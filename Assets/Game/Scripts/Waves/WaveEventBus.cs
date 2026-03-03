using System;

public static class WaveEventBus
{
    public static event Action<int> OnWaveStarted;
    public static event Action<int> OnWaveCleared;
    public static event Action OnVictory;
    public static event Action<string> OnGameOver;
    public static event Action<int, int> OnCountdownTick;

    public static void RaiseWaveStarted(int n) => OnWaveStarted?.Invoke(n);
    public static void RaiseWaveCleared(int n) => OnWaveCleared?.Invoke(n);
    public static void RaiseVictory() => OnVictory?.Invoke();
    public static void RaiseGameOver(string reason = "") => OnGameOver?.Invoke(reason);
    public static void RaiseCountdownTick(int s, int n) => OnCountdownTick?.Invoke(s, n);
}