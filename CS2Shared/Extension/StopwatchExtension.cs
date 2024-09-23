using System.Diagnostics;

namespace CS2Shared.Extension;

public static class StopwatchExtension {
    public static string GetMicrosecond(this Stopwatch stopwatch) => $"{stopwatch.Elapsed.TotalMilliseconds * 1000:n3}¦Ìs";
}

