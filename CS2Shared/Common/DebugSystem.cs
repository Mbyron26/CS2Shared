using Colossal.Serialization.Entities;
using CS2Shared.Common;
using Game.Serialization;
using Game;
using UnityEngine;

namespace CS2Shared.Common;

public partial class DebugSystem : GameSystemBaseExtension, IDefaultSerializable, ISerializable, IPostDeserialize {

    protected override void OnStartRunning() {
        base.OnStartRunning();
        Logger.Info("DebugSystem.OnStartRunning");
    }

    protected override void OnStopRunning() {
        base.OnStopRunning();
        Logger.Info("DebugSystem.OnStopRunning");
    }

    protected override void OnGamePreload(Purpose purpose, GameMode mode) {
        base.OnGamePreload(purpose, mode);
        Logger.Info("DebugSystem.OnGamePreload");
    }

    protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode) {
        base.OnGameLoadingComplete(purpose, mode);
        Logger.Info("DebugSystem.OnGameLoadingComplete");
    }

    protected override void OnGameLoaded(Context serializationContext) {
        base.OnGameLoaded(serializationContext);
        Logger.Info("DebugSystem.OnGameLoaded");
    }

    public void Deserialize<TReader>(TReader reader) where TReader : IReader {
        Logger.Info("DebugSystem.Deserialize");
    }

    public void PostDeserialize(Context context) {
        Logger.Info("DebugSystem.PostDeserialize");
    }

    public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter {
        Logger.Info("DebugSystem.Serialize");
    }

    public void SetDefaults(Context context) {
        Logger.Info("DebugSystem.SetDefaults");
    }
}