using UnityEngine;

[RequireComponent(typeof(RacketVelocityTracker))]
[RequireComponent(typeof(RacketShotSelector))]
[RequireComponent(typeof(RacketShotProfileLibrary))]
public class RacketTiltController : MonoBehaviour
{
    [Header("Visual Target")]
    [SerializeField] private Transform racketVisual;

    [Header("Auto Movement Tilt")]
    [SerializeField] private float velocityForMaxTilt = 7f;
    [SerializeField] private float maxAutoRoll = 12f;
    [SerializeField] private float maxAutoPitch = 8f;

    [Header("Shot Tilt")]
    [SerializeField] private float previewShotTiltWeight = 0.35f;
    [SerializeField] private float strikeTiltHoldTime = 0.18f;
    [SerializeField]
    private AnimationCurve strikeTiltCurve = new AnimationCurve(
        new Keyframe(0f, 1f),
        new Keyframe(1f, 0f)
    );

    [Header("Smoothing")]
    [SerializeField] private float tiltSmoothness = 14f;

    private RacketVelocityTracker velocityTracker;
    private RacketShotSelector shotSelector;
    private RacketShotProfileLibrary profileLibrary;

    private Quaternion baseLocalRotation;

    private float strikeTiltTimer;
    private RacketShotType strikeShotType;

    private void Awake()
    {
        velocityTracker = GetComponent<RacketVelocityTracker>();
        shotSelector = GetComponent<RacketShotSelector>();
        profileLibrary = GetComponent<RacketShotProfileLibrary>();

        if (racketVisual == null)
        {
            Debug.LogError("[RacketTiltController] Racket Visual is missing.", this);
            enabled = false;
            return;
        }

        baseLocalRotation = racketVisual.localRotation;
    }

    private void LateUpdate()
    {
        ApplyTilt();
    }

    public void PlayShotTilt(RacketShotType shotType)
    {
        strikeShotType = shotType;
        strikeTiltTimer = strikeTiltHoldTime;
    }

    private void ApplyTilt()
    {
        Vector3 autoTiltEuler = CalculateAutoTiltEuler();
        Vector3 shotTiltEuler = CalculateShotTiltEuler();

        Quaternion targetRotation =
            baseLocalRotation *
            Quaternion.Euler(autoTiltEuler + shotTiltEuler);

        float t = 1f - Mathf.Exp(-tiltSmoothness * Time.deltaTime);

        racketVisual.localRotation = Quaternion.Slerp(
            racketVisual.localRotation,
            targetRotation,
            t
        );
    }

    private Vector3 CalculateAutoTiltEuler()
    {
        Vector3 velocity = velocityTracker.CurrentVelocity;

        float sideAmount = Mathf.Clamp(
            velocity.x / velocityForMaxTilt,
            -1f,
            1f
        );

        float verticalAmount = Mathf.Clamp(
            velocity.y / velocityForMaxTilt,
            -1f,
            1f
        );

        float pitch = -verticalAmount * maxAutoPitch;
        float roll = -sideAmount * maxAutoRoll;

        return new Vector3(pitch, 0f, roll);
    }

    private Vector3 CalculateShotTiltEuler()
    {
        RacketShotType currentShotType = shotSelector.CurrentShotType;

        if (strikeTiltTimer > 0f)
        {
            strikeTiltTimer -= Time.deltaTime;

            float normalizedTime = 1f - Mathf.Clamp01(strikeTiltTimer / strikeTiltHoldTime);
            float weight = strikeTiltCurve.Evaluate(normalizedTime);

            RacketShotProfile strikeProfile = profileLibrary.GetProfile(strikeShotType);

            return strikeProfile.VisualTiltEuler * weight;
        }

        if (currentShotType == RacketShotType.NormalDrive)
            return Vector3.zero;

        RacketShotProfile previewProfile = profileLibrary.GetProfile(currentShotType);

        return previewProfile.VisualTiltEuler * previewShotTiltWeight;
    }
}