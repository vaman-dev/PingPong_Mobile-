using UnityEngine;

[System.Serializable]
public struct RacketShotProfile
{
    public RacketShotType ShotType;

    [Header("Ball Velocity Multipliers")]
    public float ForwardMultiplier;
    public float LiftMultiplier;
    public float RacketVelocityMultiplier;
    public float SideMultiplier;
    public float SpinMultiplier;

    [Header("Visual Tilt")]
    public Vector3 VisualTiltEuler;
}