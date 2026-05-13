using UnityEngine;

[RequireComponent(typeof(RacketVelocityTracker))]
[RequireComponent(typeof(RacketShotDirection))]
[RequireComponent(typeof(RacketShotSelector))]
[RequireComponent(typeof(RacketShotProfileLibrary))]
public class RacketHitEmitter : MonoBehaviour
{
    [Header("Serve Hit Settings")]
    [SerializeField] private float serveLiftPower = 2.5f;
    [SerializeField] private float serveSpinPower = 0f;

    [Header("Rally Hit Settings")]
    [SerializeField] private float rallyLiftPower = 2f;
    [SerializeField] private float rallySpinPower = 1f;

    [Header("Visual Feedback")]
    [SerializeField] private RacketImpulseVisual impulseVisual;
    [SerializeField] private RacketTiltController tiltController;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private RacketVelocityTracker velocityTracker;
    private RacketShotDirection shotDirection;
    private RacketShotSelector shotSelector;
    private RacketShotProfileLibrary shotProfileLibrary;

    private void Awake()
    {
        velocityTracker = GetComponent<RacketVelocityTracker>();
        shotDirection = GetComponent<RacketShotDirection>();
        shotSelector = GetComponent<RacketShotSelector>();
        shotProfileLibrary = GetComponent<RacketShotProfileLibrary>();

        if (impulseVisual == null)
            impulseVisual = GetComponent<RacketImpulseVisual>();

        if (tiltController == null)
            tiltController = GetComponent<RacketTiltController>();
    }

    public void EmitServeHit()
    {
        EmitHit(
            RacketHitType.Serve,
            transform.position,
            serveLiftPower,
            serveSpinPower
        );
    }

    public void EmitRallyHit(Vector3 contactPoint)
    {
        EmitHit(
            RacketHitType.Rally,
            contactPoint,
            rallyLiftPower,
            rallySpinPower
        );
    }

    private void EmitHit(
        RacketHitType hitType,
        Vector3 contactPoint,
        float liftPower,
        float spinPower)
    {
        RacketShotType shotType = shotSelector.CurrentShotType;
        RacketShotProfile profile = shotProfileLibrary.GetProfile(shotType);

        Vector3 direction = shotDirection.GetDirection();

        RacketHitData hitData = new RacketHitData(
            hitType,
            shotType,
            direction,
            velocityTracker.CurrentVelocity,
            contactPoint,
            liftPower,
            spinPower,
            profile
        );

        if (impulseVisual != null)
            impulseVisual.PlayImpulse();

        if (tiltController != null)
            tiltController.PlayShotTilt(shotType);

        DebugLog(
            "[RacketHitEmitter] Hit emitted." +
            " | Hit Type: " + hitType +
            " | Shot Type: " + shotType +
            " | Direction: " + direction +
            " | Velocity: " + velocityTracker.CurrentVelocity
        );

        GameEvents.RaiseRacketHitBall(hitData);
    }

    private void DebugLog(string message)
    {
        if (showDebugLogs)
            Debug.Log(message, this);
    }
}