using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Rigidbody))]
public class BallDecalProjector : MonoBehaviour
{
    [Header("Decal Prefab")]
    [SerializeField] private List<DecalProjector> decalPrefab;

    [Header("Table Detection")]
    [SerializeField] private LayerMask tableLayer;

    [Header("Placement")]
    [SerializeField] private float surfaceOffset = 0.01f;
    [SerializeField] private bool projectAlongNegativeNormal = true;

    [Header("Decal Size")]
    [SerializeField] private Vector3 decalSize = new Vector3(0.35f, 0.35f, 0.08f);
    [SerializeField] private bool overrideDecalSize = true;

    [Header("Lifetime")]
    [SerializeField] private float decalLifetime = 1.5f;
    [SerializeField] private int maxActiveDecals = 8;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private readonly Queue<DecalProjector> activeDecals = new Queue<DecalProjector>();
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (decalPrefab == null || decalPrefab.Count == 0)
        {
            Debug.LogWarning("[BallDecalProjector] Decal prefab is missing.", this);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsInLayerMask(collision.gameObject.layer, tableLayer))
            return;

        if (collision.contactCount <= 0)
            return;

        ContactPoint contact = collision.GetContact(0);

        SpawnDecal(contact);

        DebugLog(
            "[BallDecalProjector] Table impact decal spawned." +
            " | Point: " + contact.point +
            " | Normal: " + contact.normal
        );
    }

    private void SpawnDecal(ContactPoint contact)
    {
        if (decalPrefab == null || decalPrefab.Count == 0)
            return;

        Vector3 contactPoint = contact.point;
        Vector3 contactNormal = contact.normal.normalized;

        Vector3 decalPosition = contactPoint + contactNormal * surfaceOffset;
        Quaternion decalRotation = GetDecalRotation(contactNormal);

        DecalProjector decalInstance = Instantiate(
            decalPrefab[Random.Range(0, decalPrefab.Count)],
            decalPosition,
            decalRotation
        );

        if (overrideDecalSize)
        {
            decalInstance.size = decalSize;
        }

        activeDecals.Enqueue(decalInstance);

        Destroy(decalInstance.gameObject, decalLifetime);

        while (activeDecals.Count > maxActiveDecals)
        {
            DecalProjector oldDecal = activeDecals.Dequeue();

            if (oldDecal != null)
            {
                Destroy(oldDecal.gameObject);
            }
        }
    }

    private Quaternion GetDecalRotation(Vector3 surfaceNormal)
    {
        Vector3 projectionDirection = projectAlongNegativeNormal
            ? -surfaceNormal
            : surfaceNormal;

        Vector3 decalUp = GetStableDecalUp(surfaceNormal);

        return Quaternion.LookRotation(projectionDirection, decalUp);
    }

    private Vector3 GetStableDecalUp(Vector3 surfaceNormal)
    {
        Vector3 velocityDirection = rb.linearVelocity;

        Vector3 tangent = Vector3.ProjectOnPlane(
            velocityDirection,
            surfaceNormal
        );

        if (tangent.sqrMagnitude > 0.001f)
        {
            return tangent.normalized;
        }

        tangent = Vector3.ProjectOnPlane(
            transform.forward,
            surfaceNormal
        );

        if (tangent.sqrMagnitude > 0.001f)
        {
            return tangent.normalized;
        }

        return Vector3.forward;
    }

    private bool IsInLayerMask(int objectLayer, LayerMask layerMask)
    {
        return (layerMask.value & (1 << objectLayer)) != 0;
    }

    private void DebugLog(string message)
    {
        if (showDebugLogs)
        {
            Debug.Log(message, this);
        }
    }
}