using UnityEngine;

[RequireComponent(typeof(RacketHitEmitter))]
public class RacketHitDetector : MonoBehaviour
{
    [Header("Ball Detection")]
    [SerializeField] private string ballTag = "Ball";

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private RacketHitEmitter hitEmitter;

    private void Awake()
    {
        hitEmitter = GetComponent<RacketHitEmitter>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(ballTag))
            return;

        BallController ball = collision.gameObject.GetComponent<BallController>();

        if (ball == null)
            return;

        if (ball.CurrentState != BallController.BallState.InPlay)
        {
            DebugLog("[RacketHitDetector] Ignored collision. Ball is not InPlay.");
            return;
        }

        ContactPoint contact = collision.GetContact(0);

        DebugLog("[RacketHitDetector] Rally hit detected.");

        hitEmitter.EmitRallyHit(contact.point);
    }

    private void DebugLog(string message)
    {
        if (showDebugLogs)
            Debug.Log(message, this);
    }
}