﻿using Avalonia;
using System;
using BEAM.Log;
using BEAM.Profiling;

namespace BEAM;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
#if DEBUG
        Logger.Init("../../../../BEAM.Tests/loggerTests/testLogs/testLog.log");

        Timer.TimerEnd += (e) =>
        {
            Console.WriteLine($"[TIMER]: {e.Name} took {e.Watch.Elapsed.Microseconds}µs.");
        };
#else
        Logger.Init();
#endif

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}