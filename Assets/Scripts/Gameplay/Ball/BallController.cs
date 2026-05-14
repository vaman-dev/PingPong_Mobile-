using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BallFollowTarget))]
public class BallController : MonoBehaviour
{
    public enum BallState
    {
        AttachedToRacket,
        InPlay,
        Dead
    }

    [Header("Shot Settings")]
    [SerializeField] private float baseForwardPower = 10f;
    [SerializeField] private float racketVelocityMultiplier = 0.45f;
    [SerializeField] private float upwardPowerMultiplier = 0.8f;
    [SerializeField] private float sideShotPower = 4f;
    [SerializeField] private float spinAngularVelocityMultiplier = 6f;
    [SerializeField] private float maxBallSpeed = 20f;

    [Header("Table Bounce Settings")]
    [SerializeField] private float tableBounceMultiplier = 0.82f;
    [SerializeField] private float minimumBounceY = 2.2f;
    [SerializeField] private float forwardRetention = 0.95f;
    [SerializeField] private float sideRetention = 0.9f;
    [SerializeField] private float maxBounceSpeed = 18f;

    [Header("Tags")]
    [SerializeField] private string tableTag = "Table";

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private Transform currentHoldPoint;

    public BallState CurrentState { get; private set; }

    private Rigidbody rb;
    private BallFollowTarget followTarget;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        followTarget = GetComponent<BallFollowTarget>();

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    private void OnEnable()
    {
        GameEvents.OnRacketHitBall += HandleRacketHitBall;
    }

    private void OnDisable()
    {
        GameEvents.OnRacketHitBall -= HandleRacketHitBall;
    }

    public void AttachToRacket(Transform holdPoint)
    {
        CurrentState = BallState.AttachedToRacket;

        currentHoldPoint = holdPoint;

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.isKinematic = true;
        rb.useGravity = false;

        followTarget.enabled = true;
        followTarget.SetFollowTarget(holdPoint);
        followTarget.SnapToTarget();

        transform.position = holdPoint.position;

        DebugLog("[BallController] Ball attached to racket.");
    }

    private void HandleRacketHitBall(RacketHitData hitData)
    {
        if (CurrentState == BallState.Dead)
        {
            DebugLog("[BallController] Hit ignored. Ball is Dead.");
            return;
        }

        if (hitData.HitType == RacketHitType.Serve &&
            CurrentState != BallState.AttachedToRacket)
        {
            DebugLog("[BallController] Serve ignored. Ball is not attached.");
            return;
        }

        if (hitData.HitType == RacketHitType.Rally &&
            CurrentState != BallState.InPlay)
        {
            DebugLog("[BallController] Rally ignored. Ball is not InPlay.");
            return;
        }

        ShootBall(hitData);
    }

    private void ShootBall(RacketHitData hitData)
    {
        CurrentState = BallState.InPlay;

        if (hitData.HitType == RacketHitType.Serve && currentHoldPoint != null)
        {
            transform.position = currentHoldPoint.position;
        }

        followTarget.enabled = false;

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 forwardDirection = hitData.ForwardDirection.normalized;
        Vector3 sideDirection = Vector3.Cross(Vector3.up, forwardDirection).normalized;

        Vector3 forwardVelocity =
            forwardDirection *
            baseForwardPower *
            hitData.ForwardMultiplier;

        Vector3 racketVelocityInfluence =
            hitData.RacketVelocity *
            racketVelocityMultiplier *
            hitData.RacketVelocityMultiplier;

        Vector3 liftVelocity =
            Vector3.up *
            hitData.LiftPower *
            upwardPowerMultiplier *
            hitData.LiftMultiplier;

        Vector3 sideVelocity =
            sideDirection *
            sideShotPower *
            hitData.SideMultiplier;

        Vector3 finalVelocity =
            forwardVelocity +
            racketVelocityInfluence +
            liftVelocity +
            sideVelocity;

        finalVelocity = Vector3.ClampMagnitude(finalVelocity, maxBallSpeed);

        rb.linearVelocity = finalVelocity;

        ApplySpin(hitData, sideDirection);

        DebugLog(
            "[BallController] Ball shot." +
            " | Hit Type: " + hitData.HitType +
            " | Shot Type: " + hitData.ShotType +
            " | Final Velocity: " + finalVelocity +
            " | Speed: " + finalVelocity.magnitude
        );
    }

    private void ApplySpin(RacketHitData hitData, Vector3 sideDirection)
    {
        if (Mathf.Abs(hitData.SpinMultiplier) < 0.001f)
            return;

        Vector3 spinAxis = sideDirection;

        rb.angularVelocity =
            spinAxis *
            hitData.SpinPower *
            hitData.SpinMultiplier *
            spinAngularVelocityMultiplier;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (CurrentState != BallState.InPlay)
            return;

        if (!collision.gameObject.CompareTag(tableTag))
            return;

        HandleTableBounce(collision);
    }

    private void HandleTableBounce(Collision collision)
    {
        ContactPoint contact = collision.GetContact(0);

        if (contact.normal.y < 0.4f)
        {
            DebugLog("[BallController] Table collision ignored. Not top surface.");
            return;
        }

        Vector3 currentVelocity = rb.linearVelocity;

        Vector3 newVelocity = new Vector3(
            currentVelocity.x * sideRetention,
            Mathf.Abs(currentVelocity.y) * tableBounceMultiplier,
            currentVelocity.z * forwardRetention
        );

        if (newVelocity.y < minimumBounceY)
            newVelocity.y = minimumBounceY;

        newVelocity = Vector3.ClampMagnitude(newVelocity, maxBounceSpeed);

        rb.linearVelocity = newVelocity;

        GameEvents.RaiseBallHitTable(contact.point);

        DebugLog(
            "[BallController] Table bounce." +
            " | Old Velocity: " + currentVelocity +
            " | New Velocity: " + newVelocity +
            " | Speed: " + newVelocity.magnitude
        );
    }

    public void MarkDead()
    {
        CurrentState = BallState.Dead;

        followTarget.enabled = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;

        DebugLog("[BallController] Ball marked Dead.");
    }

    private void DebugLog(string message)
    {
        if (showDebugLogs)
            Debug.Log(message, this);
    }
}