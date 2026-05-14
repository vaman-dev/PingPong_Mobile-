using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action<RacketHitData> OnRacketHitBall;
    public static event Action<bool> OnBallPlayingStateChanged;

    public static event Action<Vector3> OnBallHitTable;

    public static void RaiseRacketHitBall(RacketHitData hitData)
    {
        OnRacketHitBall?.Invoke(hitData);
    }

    public static void RaiseBallPlayingStateChanged(bool isPlaying)
    {
        OnBallPlayingStateChanged?.Invoke(isPlaying);
    }

    public static void RaiseBallHitTable(Vector3 contactPoint)
    {
        OnBallHitTable?.Invoke(contactPoint);
    }

    public static void ClearAll()
    {
        OnRacketHitBall = null;
        OnBallPlayingStateChanged = null;
        OnBallHitTable = null;
    }
}