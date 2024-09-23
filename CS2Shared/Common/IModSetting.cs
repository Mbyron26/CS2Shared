using CS2Shared.Localization;

namespace CS2Shared.Common;

public interface IModSetting : ILocalization {
    string Id { get; }
    bool IsRegistered { get; }
    void Register();
    void Unregister();
}