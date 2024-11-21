namespace CS2Shared.Localization;

public interface ILocalization {
    LocaleSource LocaleSource { get; set; }
    string Id { get; }
    string LocaleId { get; set; }
    string TranslationProgress { get; }
    void LocalizationReload();
}