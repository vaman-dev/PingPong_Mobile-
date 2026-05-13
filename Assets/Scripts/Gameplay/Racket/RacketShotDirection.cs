using UnityEngine;

public class RacketShotDirection : MonoBehaviour
{
    [Header("Shot Direction")]
    [SerializeField] private Transform directionSource;
    [SerializeField] private Vector3 fallbackDirection = Vector3.forward;
    [SerializeField] private bool flattenY = true;

    public Vector3 GetDirection()
    {
        Vector3 direction = directionSource != null
            ? directionSource.forward
            : fallbackDirection;

        if (flattenY)
            direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            direction = fallbackDirection;

        return direction.normalized;
    }
}