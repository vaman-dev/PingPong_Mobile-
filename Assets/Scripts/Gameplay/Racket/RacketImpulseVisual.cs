using System.Collections;
using UnityEngine;

public class RacketImpulseVisual : MonoBehaviour
{
    [Header("Visual Target")]
    [SerializeField] private Transform racketVisual;

    [Header("Impulse Motion")]
    [SerializeField] private float impulseDistance = 0.35f;
    [SerializeField] private float impulseDuration = 0.16f;

    [Header("Direction")]
    [SerializeField] private Vector3 localImpulseDirection = Vector3.forward;

    [Header("Curve")]
    [SerializeField]
    private AnimationCurve impulseCurve = new AnimationCurve(
        new Keyframe(0f, 0f),
        new Keyframe(0.25f, 1f),
        new Keyframe(1f, 0f)
    );

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

    private Vector3 defaultLocalPosition;
    private Coroutine impulseRoutine;

    private void Awake()
    {
        if (racketVisual == null)
        {
            Debug.LogError("[RacketImpulseVisual] Racket Visual is missing.", this);
            enabled = false;
            return;
        }

        defaultLocalPosition = racketVisual.localPosition;
    }

    public void PlayImpulse()
    {
        if (!enabled || racketVisual == null)
            return;

        if (impulseRoutine != null)
            StopCoroutine(impulseRoutine);

        impulseRoutine = StartCoroutine(ImpulseRoutine());

        DebugLog("[RacketImpulseVisual] Impulse started.");
    }

    private IEnumerator ImpulseRoutine()
    {
        float elapsedTime = 0f;
        Vector3 direction = localImpulseDirection.sqrMagnitude > 0.001f
            ? localImpulseDirection.normalized
            : Vector3.forward;

        while (elapsedTime < impulseDuration)
        {
            elapsedTime += Time.deltaTime;

            float normalizedTime = Mathf.Clamp01(elapsedTime / impulseDuration);
            float curveValue = impulseCurve.Evaluate(normalizedTime);

            Vector3 offset = direction * impulseDistance * curveValue;
            racketVisual.localPosition = defaultLocalPosition + offset;

            yield return null;
        }

        racketVisual.localPosition = defaultLocalPosition;
        impulseRoutine = null;

        DebugLog("[RacketImpulseVisual] Impulse finished.");
    }

    private void DebugLog(string message)
    {
        if (showDebugLogs)
            Debug.Log(message, this);
    }
}