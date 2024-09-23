using CS2Shared.Common;

namespace CS2Shared.Patch;

public abstract class PatchModBase : ModBase {
    public HarmonyPatcher Patcher { get; private set; }

    protected override void OnPreLoad() {
        Logger.Info($"Harmony patches");
        Patcher = new HarmonyPatcher();
        Patcher.PatchAll();
        PatchingAction(Patcher);
    }

    protected override void OnPreDispose() {
        Logger.Info($"Reverse Harmony patches");
        Patcher.Reverse();
        Patcher = null;
    }

    protected virtual void PatchingAction(HarmonyPatcher patcher) { }
}