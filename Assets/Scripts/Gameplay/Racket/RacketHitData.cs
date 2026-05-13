using UnityEngine;

public readonly struct RacketHitData
{
    public readonly RacketHitType HitType;
    public readonly RacketShotType ShotType;

    public readonly Vector3 ForwardDirection;
    public readonly Vector3 RacketVelocity;
    public readonly Vector3 ContactPoint;

    public readonly float RacketSpeed;
    public readonly float LiftPower;
    public readonly float SpinPower;

    public readonly float ForwardMultiplier;
    public readonly float LiftMultiplier;
    public readonly float RacketVelocityMultiplier;
    public readonly float SideMultiplier;
    public readonly float SpinMultiplier;

    public RacketHitData(
        RacketHitType hitType,
        RacketShotType shotType,
        Vector3 forwardDirection,
        Vector3 racketVelocity,
        Vector3 contactPoint,
        float liftPower,
        float spinPower,
        RacketShotProfile shotProfile)
    {
        HitType = hitType;
        ShotType = shotType;

        ForwardDirection = forwardDirection.sqrMagnitude > 0.001f
            ? forwardDirection.normalized
            : Vector3.forward;

        RacketVelocity = racketVelocity;
        ContactPoint = contactPoint;

        RacketSpeed = racketVelocity.magnitude;
        LiftPower = liftPower;
        SpinPower = spinPower;

        ForwardMultiplier = shotProfile.ForwardMultiplier;
        LiftMultiplier = shotProfile.LiftMultiplier;
        RacketVelocityMultiplier = shotProfile.RacketVelocityMultiplier;
        SideMultiplier = shotProfile.SideMultiplier;
        SpinMultiplier = shotProfile.SpinMultiplier;
    }
}