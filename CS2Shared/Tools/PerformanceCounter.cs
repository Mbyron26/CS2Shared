using Colossal.Logging;
using System.Diagnostics;
using System;

namespace CS2Shared.Tools;

public class PerformanceCounter : IDisposable {
    protected Stopwatch stopwatch = new();
    protected Action<TimeSpan> callback;
    public static ILog Logger { get; } = LogManager.GetLogger(AssemblyTools.CurrentAssemblyName);
    public TimeSpan Result => stopwatch.Elapsed;
    public string ReportSeconds => string.Format("{0:F3}s", Result.TotalSeconds);
    public TimeSpan ResultAndRestart {
        get {
            TimeSpan elapsed = stopwatch.Elapsed;
            stopwatch.Restart();
            return elapsed;
        }
    }

    public PerformanceCounter() {
        stopwatch.Start();
    }

    public PerformanceCounter(Action<TimeSpan> callback) : this() {
        this.callback = callback;
    }

    public static PerformanceCounter Start(Action<TimeSpan> callback) => new(callback);

    public static PerformanceCounter StartAndLog(string logPrefix = "") => new(_ => Logger.Info($"Performance Counter: {logPrefix} {_.TotalMilliseconds}ms"));


    public void Report(Action<TimeSpan> callback) => callback(Result);

    public void Dispose() {
        stopwatch.Stop();
        callback?.Invoke(Result);
    }

}