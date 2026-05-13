using UnityEngine;

public class RacketShotProfileLibrary : MonoBehaviour
{
    [SerializeField] private RacketShotProfile[] profiles;

    private void Reset()
    {
        profiles = new[]
        {
            new RacketShotProfile
            {
                ShotType = RacketShotType.NormalDrive,
                ForwardMultiplier = 1f,
                LiftMultiplier = 1f,
                RacketVelocityMultiplier = 1f,
                SideMultiplier = 0f,
                SpinMultiplier = 0.2f,
                VisualTiltEuler = new Vector3(5f, 0f, 0f)
            },

            new RacketShotProfile
            {
                ShotType = RacketShotType.PowerShot,
                ForwardMultiplier = 1.45f,
                LiftMultiplier = 0.55f,
                RacketVelocityMultiplier = 1.25f,
                SideMultiplier = 0f,
                SpinMultiplier = 0.4f,
                VisualTiltEuler = new Vector3(18f, 0f, 0f)
            },

            new RacketShotProfile
            {
                ShotType = RacketShotType.LobShot,
                ForwardMultiplier = 0.75f,
                LiftMultiplier = 1.85f,
                RacketVelocityMultiplier = 0.75f,
                SideMultiplier = 0f,
                SpinMultiplier = 0.1f,
                VisualTiltEuler = new Vector3(-20f, 0f, 0f)
            },

            new RacketShotProfile
            {
                ShotType = RacketShotType.SliceShot,
                ForwardMultiplier = 0.85f,
                LiftMultiplier = 0.65f,
                RacketVelocityMultiplier = 0.7f,
                SideMultiplier = 0f,
                SpinMultiplier = 1.6f,
                VisualTiltEuler = new Vector3(-10f, 0f, 0f)
            },

            new RacketShotProfile
            {
                ShotType = RacketShotType.FlickLeft,
                ForwardMultiplier = 0.95f,
                LiftMultiplier = 0.85f,
                RacketVelocityMultiplier = 0.9f,
                SideMultiplier = -1f,
                SpinMultiplier = 1.2f,
                VisualTiltEuler = new Vector3(5f, -18f, 14f)
            },

            new RacketShotProfile
            {
                ShotType = RacketShotType.FlickRight,
                ForwardMultiplier = 0.95f,
                LiftMultiplier = 0.85f,
                RacketVelocityMultiplier = 0.9f,
                SideMultiplier = 1f,
                SpinMultiplier = 1.2f,
                VisualTiltEuler = new Vector3(5f, 18f, -14f)
            },

            new RacketShotProfile
            {
                ShotType = RacketShotType.DefensiveBlock,
                ForwardMultiplier = 0.65f,
                LiftMultiplier = 1.1f,
                RacketVelocityMultiplier = 0.35f,
                SideMultiplier = 0f,
                SpinMultiplier = 0f,
                VisualTiltEuler = new Vector3(-4f, 0f, 0f)
            }
        };
    }

    public RacketShotProfile GetProfile(RacketShotType shotType)
    {
        if (profiles != null)
        {
            for (int i = 0; i < profiles.Length; i++)
            {
                if (profiles[i].ShotType == shotType)
                    return profiles[i];
            }
        }

        return GetFallbackProfile(shotType);
    }

    private RacketShotProfile GetFallbackProfile(RacketShotType shotType)
    {
        return new RacketShotProfile
        {
            ShotType = shotType,
            ForwardMultiplier = 1f,
            LiftMultiplier = 1f,
            RacketVelocityMultiplier = 1f,
            SideMultiplier = 0f,
            SpinMultiplier = 0f,
            VisualTiltEuler = Vector3.zero
        };
    }
}