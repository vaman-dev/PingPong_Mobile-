using UnityEngine;
using UnityEngine.InputSystem;

public class RacketInput : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;

    [Header("Camera")]
    [SerializeField] private Camera mainCamera;

    [Header("Movement Plane")]
    [SerializeField] private float movementPlaneZ = -1.5f;

    [Header("Viewport Clamp")]
    [SerializeField] private float viewportXOffset = 0.03f;
    [SerializeField] private float viewportYOffsetBottom = 0.05f;
    [SerializeField] private float viewportYOffsetTop = 0.03f;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

    public Vector3 TargetWorldPosition { get; private set; }

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        if (moveAction == null)
        {
            Debug.LogError("[RacketInput] Move Action Reference is missing!", this);
            return;
        }

        moveAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null)
            moveAction.action.Disable();
    }

    private void Update()
    {
        ReadMovementInput();
    }

    private void ReadMovementInput()
    {
        if (moveAction == null || mainCamera == null)
            return;

        Vector2 screenPosition = moveAction.action.ReadValue<Vector2>();

        Vector3 viewportPosition = mainCamera.ScreenToViewportPoint(screenPosition);

        viewportPosition.x = Mathf.Clamp(
            viewportPosition.x,
            viewportXOffset,
            1f - viewportXOffset
        );

        viewportPosition.y = Mathf.Clamp(
            viewportPosition.y,
            viewportYOffsetBottom,
            1f - viewportYOffsetTop
        );

        Ray ray = mainCamera.ViewportPointToRay(viewportPosition);

        Plane movementPlane = new Plane(
            Vector3.forward,
            new Vector3(0f, 0f, movementPlaneZ)
        );

        if (movementPlane.Raycast(ray, out float enter))
        {
            TargetWorldPosition = ray.GetPoint(enter);
        }

        DebugLog(
            "[RacketInput] Screen: " + screenPosition +
            " | Viewport: " + viewportPosition +
            " | Target World: " + TargetWorldPosition
        );
    }

    private void DebugLog(string message)
    {
        if (showDebugLogs)
            Debug.Log(message, this);
    }
}