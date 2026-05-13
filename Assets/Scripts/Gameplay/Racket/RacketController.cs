using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(RacketInput))]
public class RacketController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSmoothness = 15f;

    [Header("Movement Bounds")]
    [SerializeField] private Vector2 xBounds = new Vector2(-4f, 4f);
    [SerializeField] private Vector2 yBounds = new Vector2(0.5f, 4f);
    [SerializeField] private float fixedZPosition = 0f;

    private Rigidbody rb;
    private RacketInput racketInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        racketInput = GetComponent<RacketInput>();

        rb.isKinematic = true;
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        Debug.Log("[RacketController] Awake called.");
        Debug.Log("[RacketController] Rigidbody found: " + rb);
        Debug.Log("[RacketController] RacketInput found: " + racketInput);
    }

    private void FixedUpdate()
    {
        MoveRacket();
    }

    private void MoveRacket()
    {
        Vector3 rawTargetPosition = racketInput.TargetWorldPosition;

        Vector3 clampedTargetPosition = rawTargetPosition;

        clampedTargetPosition.x = Mathf.Clamp(clampedTargetPosition.x, xBounds.x, xBounds.y);
        clampedTargetPosition.y = Mathf.Clamp(clampedTargetPosition.y, yBounds.x, yBounds.y);
        clampedTargetPosition.z = fixedZPosition;

        Vector3 smoothedPosition = Vector3.Lerp(
            rb.position,
            clampedTargetPosition,
            moveSmoothness * Time.fixedDeltaTime
        );

        Debug.Log(
            "[RacketController] Raw Target: " + rawTargetPosition +
            " | Clamped Target: " + clampedTargetPosition +
            " | Current Pos: " + rb.position +
            " | Smoothed Pos: " + smoothedPosition
        );

        rb.MovePosition(smoothedPosition);
    }
}