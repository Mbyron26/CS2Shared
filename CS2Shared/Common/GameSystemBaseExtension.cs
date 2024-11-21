using Colossal.Logging;
using Colossal.Serialization.Entities;
using CS2Shared.Tools;
using Game;
using System;

namespace CS2Shared.Common;

public partial class GameSystemBaseExtension : GameSystemBase {
    protected ILog Logger { get; } = LogManager.GetLogger(AssemblyTools.CurrentAssemblyName);
    protected Type SystemType { get; private set; }
    protected string SystemName => SystemType is null ? string.Empty : SystemType.Name;
    protected GameMode Mode { get; set; }
    protected bool InMainMenu => Mode == GameMode.MainMenu;
    protected bool InGame => Mode == GameMode.Game;
    protected bool NotInGame => !InGame;
    protected bool InEditor => Mode == GameMode.Editor;
    protected bool InGameOrEditor => Mode == GameMode.GameOrEditor;

    protected override void OnCreate() {
        base.OnCreate();
        SystemType = GetType();
        Logger.Info($"System created: {SystemName}");
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        Logger.Info($"System destroyed: {SystemName}");
    }

    protected override void OnUpdate() { }

    protected override void OnGamePreload(Purpose purpose, GameMode mode) => Mode = mode;
}