using System;
using System.Diagnostics;

namespace BEAM.Profiling;

public delegate void TimerEndEvent(TimerEventArgs timerEventArgs);

public class TimerEventArgs(string name, Stopwatch watch)
{
    public string Name => name;
    public Stopwatch Watch => watch;
}

/// <summary>
/// Timer token.
/// Invokes OnTimerEnd on dispose
/// </summary>
/// <param name="name"></param>
/// <param name="watch"></param>
public class TimerScopeToken(string name, Stopwatch watch) : IDisposable
{
    public event TimerEndEvent OnTimerEnd = delegate { };

    public void Dispose()
    {
        OnTimerEnd?.Invoke(new TimerEventArgs(name, watch));
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Utility class for timing methods.
/// Usage:
/// <code>
/// using var _ = Timer.Start();
/// </code>
/// </summary>
public static class Timer
{
    public static event TimerEndEvent TimerEnd = delegate { };

    /// <summary>
    /// Starts a new timer which automatically invokes TimerEnd when it's stack closes.
    /// Usage:
    /// <code>
    /// using var _ = Timer.Start();
    /// </code>
    /// </summary>
    /// <param name="name">The name of the timer. If null, the name of the current method name is being used</param>
    /// <returns>A token that automatically disposes or can be disposed manually</returns>
    public static TimerScopeToken Start(string? name = null)
    {
        var method = new StackFrame(1).GetMethod()!;
        var className = method.DeclaringType!.Name;
        var ns = method.DeclaringType.Namespace;

        var token = new TimerScopeToken(name ?? $"{ns}.{className}.{method.Name}()", Stopwatch.StartNew());
        token.OnTimerEnd += TimerEnd;
        return token;
    }
}