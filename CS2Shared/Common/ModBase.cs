using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using CS2Shared.Extension;
using CS2Shared.Localization;
using CS2Shared.Manager;
using CS2Shared.Settings;
using CS2Shared.Tools;
using Game;
using Game.Modding;
using Game.SceneFlow;
using Game.Serialization;
using Game.UI.Menu;
using System;

namespace CS2Shared.Common;

public abstract class ModBase {
    public static ModBase Instance { get; private set; }
    public abstract DateTime VersionDate { get; }
    public virtual bool BetaVersion { get; }
    public ExecutableAsset ModAsset { get; private set; }
    public string ModPath => ModAsset is null ? string.Empty : ModAsset.path;
    public ILog Logger { get; private set; }
    public IModSetting Setting { get; protected set; }
    protected UpdateSystem UpdateSystem { get; private set; }

    public void OnLoad(UpdateSystem updateSystem) {
#if DEBUG
        Logger = LogManager.GetLogger(AssemblyTools.CurrentAssemblyName).SetShowsErrorsInUI(true);
#else
        Logger = LogManager.GetLogger(AssemblyTools.CurrentAssemblyName).SetShowsErrorsInUI(false);
#endif
        Instance = this;
        Logger.Info($"Mod OnLoad, version: {AssemblyTools.CurrentAssemblyVersion}");
        OnPreLoad();
        GetModAsset();
        Logger.Info($"Current mod asset at {DataDirectory.GetModPath(ModAsset)}");
        Logger.Info($"Current log at {Logger.logPath}");
        RegisterOptionsUI();
        UpdateSystem = updateSystem;
        CreateSystem(UpdateSystem);
        OnPostLoad();
    }

    protected virtual void OnPreLoad() { }
    protected virtual void OnPostLoad() { }
    protected virtual void OnPreDispose() { }
    protected virtual void OnPostDispose() { }

    public void OnDispose() {
        OnPreDispose();
        Logger.Info($"Mod Dispose");
        UnRegisterOptionsUI();
        OnPostDispose();
    }

    protected virtual void CreateSystem(UpdateSystem updateSystem) {
#if DEBUG
        updateSystem.UpdateAt<DebugSystem>(SystemUpdatePhase.MainLoop);
        updateSystem.UpdateAfter<PostDeserialize<DebugSystem>>(SystemUpdatePhase.Deserialize);
#endif
    }

    protected void LoadSetting(IModSetting defaultModSetting = null) => AssetDatabase.global.LoadSettings(AssemblyTools.GetCurrentAssemblyName(), Setting, defaultModSetting);

    protected virtual void RegisterOptionsUI() {
        CreateSetting();
        if (Setting is null)
            return;
        Logger.Info($"Register Options UI");
        if (BetaVersion) {
            BetaFilter.AddOption(Setting.Id);
            Logger.Info($"Added Beta ID");
        }
        Setting.Register();
        Logger.Info($"Load setting");
        ManagerPool.GetOrCreateManager<ModLocalizationManager, string, ILocalization>(ModPath, Setting);
    }

    protected virtual void UnRegisterOptionsUI() {
        if (Setting is null)
            return;
        Logger.Info($"UnRegister Options UI");
        Setting.Unregister();
        Setting = null;
    }

    protected abstract void CreateSetting();

    private void GetModAsset() {
        if (ModAsset is not null)
            return;
        if (GameManager.instance.modManager.TryGetExecutableAsset((IMod)this, out ExecutableAsset asset)) {
            ModAsset = asset;
            Logger.Info("Got mod asset");
        }
        else
            Logger.Error("Get mod asset failed");
    }
}