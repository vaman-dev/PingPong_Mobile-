using UnityEngine;
using UnityEngine.InputSystem;

public class RacketShotSelector : MonoBehaviour
{
    [Header("Shot Input Actions")]
    [SerializeField] private InputActionReference powerShotAction;
    [SerializeField] private InputActionReference lobShotAction;
    [SerializeField] private InputActionReference sliceShotAction;
    [SerializeField] private InputActionReference flickLeftAction;
    [SerializeField] private InputActionReference flickRightAction;
    [SerializeField] private InputActionReference blockShotAction;

    public RacketShotType CurrentShotType => ResolveShotType();

    private void OnEnable()
    {
        EnableAction(powerShotAction);
        EnableAction(lobShotAction);
        EnableAction(sliceShotAction);
        EnableAction(flickLeftAction);
        EnableAction(flickRightAction);
        EnableAction(blockShotAction);
    }

    private void OnDisable()
    {
        DisableAction(powerShotAction);
        DisableAction(lobShotAction);
        DisableAction(sliceShotAction);
        DisableAction(flickLeftAction);
        DisableAction(flickRightAction);
        DisableAction(blockShotAction);
    }

    private RacketShotType ResolveShotType()
    {
        if (IsPressed(blockShotAction))
            return RacketShotType.DefensiveBlock;

        if (IsPressed(powerShotAction))
            return RacketShotType.PowerShot;

        if (IsPressed(lobShotAction))
            return RacketShotType.LobShot;

        if (IsPressed(sliceShotAction))
            return RacketShotType.SliceShot;

        if (IsPressed(flickLeftAction))
            return RacketShotType.FlickLeft;

        if (IsPressed(flickRightAction))
            return RacketShotType.FlickRight;

        return RacketShotType.NormalDrive;
    }

    private bool IsPressed(InputActionReference actionReference)
    {
        return actionReference != null &&
               actionReference.action != null &&
               actionReference.action.IsPressed();
    }

    private void EnableAction(InputActionReference actionReference)
    {
        if (actionReference != null && actionReference.action != null)
            actionReference.action.Enable();
    }

    private void DisableAction(InputActionReference actionReference)
    {
        if (actionReference != null && actionReference.action != null)
            actionReference.action.Disable();
    }
}