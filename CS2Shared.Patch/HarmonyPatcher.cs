using Colossal.IO.AssetDatabase.Internal;
using Colossal.Logging;
using CS2Shared.Tools;
using HarmonyLib;
using System;
using System.Reflection;
using System.Text;

namespace CS2Shared.Patch;

public sealed class HarmonyPatcher {
    private ILog Logger { get; } = LogManager.GetLogger(AssemblyTools.CurrentAssemblyName);
    public Harmony Harmony { get; private set; }

    public HarmonyPatcher(string harmonyID = null) => Harmony = harmonyID is null ? new Harmony(AssemblyTools.CurrentAssemblyName) : new Harmony(harmonyID);

    public void Reverse() {
        Harmony.UnpatchAll();
        Harmony = null;
    }

    public void PatchAll() => Harmony.PatchAll();

    public void PrefixPatching(Type originalType, string originalMethod, Type patchType, string patchMethod, Type[] targetParam = null) => PatchMethod(PatcherType.Prefix, originalType, originalMethod, patchType, patchMethod, targetParam);
    public void PrefixPatching(MethodBase originalMethodInfo, MethodInfo patchMethodInfo) => PatchMethod(PatcherType.Prefix, originalMethodInfo, patchMethodInfo);

    public void PostfixPatching(Type originalType, string originalMethod, Type patchType, string patchMethod, Type[] targetParam = null) => PatchMethod(PatcherType.Postfix, originalType, originalMethod, patchType, patchMethod, targetParam);
    public void PostfixPatching(MethodBase originalMethodInfo, MethodInfo patchMethodInfo) => PatchMethod(PatcherType.Postfix, originalMethodInfo, patchMethodInfo);

    public void TranspilerPatching(Type originalType, string originalMethod, Type patchType, string patchMethod, Type[] targetParam = null) => PatchMethod(PatcherType.Transpiler, originalType, originalMethod, patchType, patchMethod, targetParam);
    public void TranspilerPatching(MethodBase originalMethodInfo, MethodInfo patchMethodInfo) => PatchMethod(PatcherType.Transpiler, originalMethodInfo, patchMethodInfo);

    private void PatchMethod(PatcherType patcherType, MethodBase originalMethodInfo, MethodInfo patchMethodInfo) {
        if (originalMethodInfo is null) {
            Logger.Error($"Original method not found");
            return;
        }
        if (patchMethodInfo is null) {
            Logger.Error($"Patch method not found");
            return;
        }
        switch (patcherType) {
            case PatcherType.Prefix: Harmony.Patch(originalMethodInfo, prefix: new HarmonyMethod(patchMethodInfo)); break;
            case PatcherType.Postfix: Harmony.Patch(originalMethodInfo, postfix: new HarmonyMethod(patchMethodInfo)); break;
            case PatcherType.Transpiler: Harmony.Patch(originalMethodInfo, transpiler: new HarmonyMethod(patchMethodInfo)); break;
        };
        Logger.Info(PatchInfo(patcherType, originalMethodInfo, originalMethodInfo.Name, patchMethodInfo, patchMethodInfo.Name));
    }

    private void PatchMethod(PatcherType patcherType, Type originalType, string originalMethod, Type patchType, string patchMethod, Type[] targetParam = null) {
        var original = AccessTools.Method(originalType, originalMethod, targetParam);
        var patch = AccessTools.Method(patchType, patchMethod);
        if (original is null) {
            Logger.Error($"Original method [{originalMethod}] not found");
            return;
        }
        if (patch is null) {
            Logger.Error($"Patch method [{patchMethod}] not found");
            return;
        }
        switch (patcherType) {
            case PatcherType.Prefix: Harmony.Patch(original, prefix: new HarmonyMethod(patch)); break;
            case PatcherType.Postfix: Harmony.Patch(original, postfix: new HarmonyMethod(patch)); break;
            case PatcherType.Transpiler: Harmony.Patch(original, transpiler: new HarmonyMethod(patch)); break;
        };
        Logger.Info(PatchInfo(patcherType, original, originalMethod, patch, patchMethod));
    }

    public void LogPatchedMethods() => Logger.Info(GetPatchedMethods());

    public string GetPatchedMethods() {
        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine("Patched Methods:");
        Harmony.GetPatchedMethods().ForEach(_ => stringBuilder.AppendLine($"{_.DeclaringType.FullName}.{_.Name}"));
        return stringBuilder.ToString();
    }

    private string PatchInfo(PatcherType patchType, MethodBase original, string originalMethod, MethodInfo patch, string patchMethod) => $"[{patchType}] [{original.DeclaringType.FullName}.{originalMethod}] patched by [{patch.DeclaringType.FullName}.{patchMethod}]";

    public enum PatcherType {
        Prefix,
        Postfix,
        Transpiler
    }
}
