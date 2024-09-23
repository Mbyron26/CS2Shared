using Colossal.Localization;

namespace CS2Shared.Localization;

public static class LocalizationManagerExtension {
    public static void AddSource(this LocalizationManager manager, LocaleSource source) {
        if (source is not null)
            manager.AddSource(source.Id, source);
    }

    public static void RemoveSource(this LocalizationManager manager, LocaleSource source) {
        if (source is not null)
            manager.RemoveSource(source.Id, source);
    }

}