using System;
using System.IO;
using System.Reflection;

namespace CS2Shared.Tools;

public static class AssemblyTools {
    public static string CurrentAssemblyName { get; } = GetCurrentAssemblyName();
    public static Version CurrentAssemblyVersion { get; } = GetCurrentAssemblyVersion();

    public static Version GetCurrentAssemblyVersion() => Assembly.GetExecutingAssembly().GetName().Version;

    public static string GetCurrentAssemblyName() => Assembly.GetExecutingAssembly().GetName().Name;
}