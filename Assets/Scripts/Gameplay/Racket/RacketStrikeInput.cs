using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(RacketHitEmitter))]
public class RacketStrikeInput : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference strikeAction;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private RacketHitEmitter hitEmitter;

    private void Awake()
    {
        hitEmitter = GetComponent<RacketHitEmitter>();
    }

    private void OnEnable()
    {
        if (strikeAction == null)
        {
            Debug.LogError("[RacketStrikeInput] Strike Action is missing.", this);
            return;
        }

        strikeAction.action.Enable();
        strikeAction.action.performed += OnStrikePerformed;
    }

    private void OnDisable()
    {
        if (strikeAction == null)
            return;

        strikeAction.action.performed -= OnStrikePerformed;
        strikeAction.action.Disable();
    }

    private void OnStrikePerformed(InputAction.CallbackContext context)
    {
        DebugLog("[RacketStrikeInput] Strike performed.");
        hitEmitter.EmitServeHit();
    }

    private void DebugLog(string message)
    {
        if (showDebugLogs)
            Debug.Log(message, this);
    }
}