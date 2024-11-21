using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Colossal.Logging.Utils;
using Colossal.PSI.Environment;
using CS2Shared.Tools;
using System;
using System.IO;

namespace CS2Shared.Extension;

public static class DataDirectory {
    public static string UserDataDirectory { get; } = EnvPath.kUserDataPath;
    public static string ModsDataDirectory { get; } = CheckedString.Combine(UserDataDirectory, "ModsData");
    public static string CurrentModDataDirectory { get; } = CheckedString.Combine(ModsDataDirectory, AssemblyTools.CurrentAssemblyName);
    public static string ModsSettingsDirectory { get; } = CheckedString.Combine(UserDataDirectory, "ModsSettings");
    public static string CurrentModSettingsDirectory { get; } = CheckedString.Combine(ModsSettingsDirectory, AssemblyTools.CurrentAssemblyName);
    public static string LocalModsDirectory { get; } = EnvPath.kLocalModsPath;
    public static string LogsDirectory { get; } = LogManager.kDefaultLogPath;
    public static string GetModPath(ExecutableAsset executableAsset) => Path.GetDirectoryName(executableAsset?.path).Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    public static string GetModDirectoryCreationTime(ExecutableAsset executableAsset) => Directory.Exists(GetModPath(executableAsset)) ? new DirectoryInfo(GetModPath(executableAsset)).CreationTime.ToString("yyyy-MM-dd HH:mm:ss") : DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

}
