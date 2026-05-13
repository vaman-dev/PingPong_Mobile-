using UnityEngine;

public class BallFollowTarget : MonoBehaviour
{
    [Header("Follow Target")]
    [SerializeField] private Transform followTarget;

    [Header("Follow Settings")]
    [SerializeField] private bool smoothFollow = false;
    [SerializeField] private float followSmoothness = 25f;

    public void SetFollowTarget(Transform target)
    {
        followTarget = target;
        SnapToTarget();
    }

    private void LateUpdate()
    {
        if (followTarget == null)
            return;

        if (smoothFollow)
        {
            float t = 1f - Mathf.Exp(-followSmoothness * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, followTarget.position, t);
        }
        else
        {
            transform.position = followTarget.position;
        }
    }

    public void SnapToTarget()
    {
        if (followTarget == null)
            return;

        transform.position = followTarget.position;
    }
}