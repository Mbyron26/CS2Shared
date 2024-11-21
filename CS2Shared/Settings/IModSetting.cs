using CS2Shared.Localization;

namespace CS2Shared.Settings;

public interface IModSetting : ILocalization {
    bool IsRegistered { get; }
    void Register();
    void Unregister();
}