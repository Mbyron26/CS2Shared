using Game.Modding;
using Game.SceneFlow;
using System.Collections.Generic;
using System.Linq;

namespace CS2Shared.Tools;

public static class ModTools {
    public static bool IsModEnabled(string name) => GetModInfoList().Any(_ => ((_.asset.name == name) && (_.asset.isEnabled)));

    public static bool IsModInclusive(string name) => GetModInfoList().Any(_ => _.asset.name == name);

    public static string GetModPath(string name) => GetModInfoList().FirstOrDefault(_ => _.asset.name == name)?.asset.path;

    public static string GetCurrentModSubPath() => GetModSubPath(AssemblyTools.CurrentAssemblyName);

    public static string GetModSubPath(string name) => GetModInfoList().FirstOrDefault(_ => _.asset.name == name)?.asset.subPath;

    public static IEnumerable<string> GetModNameList() => GetModInfoList().Where(_ => _.asset.isMod).Select(_ => _.asset.name);

    public static IEnumerable<ModManager.ModInfo> GetModInfoList() => GameManager.instance?.modManager;
}