using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    [Header("Ball Prefab")]
    [SerializeField] private BallController ballPrefab;

    [Header("Spawn / Hold Point")]
    [SerializeField] private Transform ballHoldPoint;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private BallController currentBall;

    private void Start()
    {
        SpawnBallOnce();
    }

    private void SpawnBallOnce()
    {
        if (ballPrefab == null)
        {
            Debug.LogError("[BallSpawner] Ball prefab is missing.", this);
            return;
        }

        if (ballHoldPoint == null)
        {
            Debug.LogError("[BallSpawner] Ball hold point is missing.", this);
            return;
        }

        currentBall = Instantiate(
            ballPrefab,
            ballHoldPoint.position,
            Quaternion.identity
        );

        currentBall.AttachToRacket(ballHoldPoint);

        DebugLog("[BallSpawner] Ball spawned and attached.");
    }

    public void ResetBallToRacket()
    {
        if (currentBall == null)
        {
            SpawnBallOnce();
            return;
        }

        currentBall.AttachToRacket(ballHoldPoint);

        DebugLog("[BallSpawner] Ball reset to racket.");
    }

    private void DebugLog(string message)
    {
        if (showDebugLogs)
            Debug.Log(message, this);
    }
}