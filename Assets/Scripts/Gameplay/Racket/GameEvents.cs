using System;

public static class GameEvents
{
    public static event Action<RacketHitData> OnRacketHitBall;

    public static void RaiseRacketHitBall(RacketHitData hitData)
    {
        OnRacketHitBall?.Invoke(hitData);
    }

    public static void ClearAll()
    {
        OnRacketHitBall = null;
    }
}