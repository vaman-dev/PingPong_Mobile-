using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RacketVelocityTracker : MonoBehaviour
{
    public Vector3 CurrentVelocity { get; private set; }
    public float CurrentSpeed => CurrentVelocity.magnitude;

    private Rigidbody rb;
    private Vector3 previousPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        previousPosition = rb.position;
    }

    private void FixedUpdate()
    {
        CurrentVelocity = (rb.position - previousPosition) / Time.fixedDeltaTime;
        previousPosition = rb.position;
    }
}