using Colossal.IO.AssetDatabase.Internal;
using Colossal.Localization;
using Colossal.Logging;
using CS2Shared.Manager;
using CS2Shared.Tools;
using Game.SceneFlow;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CS2Shared.Localization;

public sealed class ModLocalizationManager : IManager<string, ILocalization> {
    public const string USE_GAME_LANGUAGE = "UGL";

    private ILog Logger { get; set; }
    public LocalizationManager GameLocalizationManager { get; private set; }
    public string GameActiveLocaleId => GameLocalizationManager.activeLocaleId;
    public string FallbackLocaleId { get; private set; }
    public bool IsLoaded { get; private set; }
    private string ModPath { get; set; }
    private ILocalization ModSetting { get; set; }
    public Dictionary<string, LocaleSource> LocaleSources { get; set; }
    public LocaleSource CurrentLocaleSource { get; private set; }
    public LocaleSource ENLocaleSource => LocaleSources[LocaleSource.EN_LOCALE_ID];
    public bool Processing { get; private set; }
    public string SerializePath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), ".json");

    public event Action<string> OnFallbackLocaleIdChanged;

    public void SerializeLocale() {
        if (ModSetting.LocaleSource is null) {
            Logger.Info("ModLocaleSource is null");
            return;
        }
        try {
            var str = JsonConvert.SerializeObject(ModSetting.LocaleSource.Source, Formatting.Indented);
            File.WriteAllText(SerializePath, str);
            Logger.Info($"Serialized localization, path: {SerializePath}");
        }
        catch (Exception e) {
            Logger.Error(e, "Serialize localization failed");
        }
        Logger.Info("Serialize locale completed");
    }

    private void InitLocale() {
        Processing = true;
        EnsureModSettingLoaceleId();
        NotifyFallbackLocaleIdChanged(false);
        var useGameLanguage = ModSetting.LocaleId == USE_GAME_LANGUAGE;
        LocaleSource source;
        if (useGameLanguage ? TryGetValueFromSources(GameActiveLocaleId, out source) : TryGetValueFromSources(ModSetting.LocaleId, out source)) {
            CurrentLocaleSource = source;
        }
        else {
            SetDefaultLocale();
        }
        AddSource();
        var str = useGameLanguage ? "use game language" : "customize language";
        Logger.Info($"InitLocale, {str}, use {CurrentLocaleSource.Id} source, game active locale: {GameActiveLocaleId}");
        GameLocalizationManager.onActiveDictionaryChanged += OnActiveDictionaryChanged;
        Processing = false;
    }

    private void OnActiveDictionaryChanged() {
        if (Processing)
            return;
        Processing = true;
        if (ModSetting.LocaleId == USE_GAME_LANGUAGE) {
            if (GameActiveLocaleId != CurrentLocaleSource.Id) {
                RemoveSource();
                NotifyFallbackLocaleIdChanged();
                if (TryGetValueFromSources(GameActiveLocaleId, out var source)) {
                    CurrentLocaleSource = source;
                }
                else {
                    SetDefaultLocale();
                }
                AddSource();
                Logger.Info($"OnActiveDictionaryChanged, use game language, use {CurrentLocaleSource.Id} source, game active locale: {GameActiveLocaleId}");
            }
#if DEBUG
            else {
                Logger.Info($"No need to change locale on active dictionary changed, use game language, use {CurrentLocaleSource.Id} source, game active locale: {GameActiveLocaleId}");
            }
#endif
        }
        else {
            if (FallbackLocaleId != GameActiveLocaleId) {       
                RemoveSource();
                NotifyFallbackLocaleIdChanged();
                if (TryGetValueFromSources(ModSetting.LocaleId, out var source)) {
                    CurrentLocaleSource = source;
                }
                else {
                    SetDefaultLocale();
                }
                AddSource();
                Logger.Info($"OnActiveDictionaryChanged, customize language, use {CurrentLocaleSource.Id} source, game active locale: {GameActiveLocaleId}");
                ModSetting.LocalizationReload();
            }
#if DEBUG
            else {
                Logger.Info($"No need to change locale on active dictionary changed, customize language, use {CurrentLocaleSource.Id} source, game active locale: {GameActiveLocaleId}");
            }
#endif
        }
        Processing = false;
    }

    public void OnLocaleOptionChanged(string localeId) {
        if (Processing)
            return;
        Processing = true;
        var useGameLanguage = localeId == USE_GAME_LANGUAGE;
        LocaleSource source;
        RemoveSource();
        NotifyFallbackLocaleIdChanged();
        if (useGameLanguage ? TryGetValueFromSources(GameActiveLocaleId, out source) : TryGetValueFromSources(localeId, out source)) {
            CurrentLocaleSource = source;
        }
        else {
            SetDefaultLocale();
        }
        AddSource();
        var str = useGameLanguage ? "use game language" : "customize language";
        Logger.Info($"OnOptionLocaleChanged, {str}, use {CurrentLocaleSource.Id} source, game active locale: {GameActiveLocaleId}");
        Processing = false;
    }

    private void EnsureModSettingLoaceleId() {
        if (string.IsNullOrEmpty(ModSetting.LocaleId)) {
            ModSetting.LocaleId = USE_GAME_LANGUAGE;
            Logger.Warn("ModSetting.LocaleId is null or empty, corrected to use game language");
        }
        if (ModSetting.LocaleId != USE_GAME_LANGUAGE && !LocaleSources.ContainsKey(ModSetting.LocaleId)) {
            ModSetting.LocaleId = USE_GAME_LANGUAGE;
            Logger.Warn("LocaleSources does not contain ModSetting.LocaleId, corrected to use game language");
        }
    }

    private void NotifyFallbackLocaleIdChanged(bool invoke = true) {
        FallbackLocaleId = GameLocalizationManager.activeLocaleId;
        if (!invoke)
            return;
        OnFallbackLocaleIdChanged?.Invoke(FallbackLocaleId);
    }

    private bool TryGetValueFromSources(string localeId, out LocaleSource localeSource) => LocaleSources.TryGetValue(localeId, out localeSource);

    private void SetDefaultLocaleAndAddSource() {
        SetDefaultLocale();
        AddSource(CurrentLocaleSource);
    }

    private void SetDefaultLocale() => CurrentLocaleSource = LocaleSources[LocaleSource.EN_LOCALE_ID];

    public void AddSource(string localeId, LocaleSource localeSource) => GameLocalizationManager.AddSource(localeId, localeSource);

    public void AddSource(LocaleSource localeSource) => GameLocalizationManager.AddSource(localeSource);

    public void AddSource() => GameLocalizationManager.AddSource(FallbackLocaleId, CurrentLocaleSource);

    private void AddENButOtherSource() => GameLocalizationManager.AddSource(LocaleSource.EN_LOCALE_ID, CurrentLocaleSource);

    public void AddENButOtherSource(LocaleSource localeSource) => GameLocalizationManager.AddSource(LocaleSource.EN_LOCALE_ID, localeSource);

    public void RemoveSource(string localeId, LocaleSource localeSource) => GameLocalizationManager.RemoveSource(localeId, localeSource);

    public void RemoveSource(LocaleSource localeSource) => GameLocalizationManager.RemoveSource(localeSource.Id, localeSource);

    private void RemoveSource() {
        GameLocalizationManager.RemoveSource(FallbackLocaleId, CurrentLocaleSource);
        Logger.Info($"Removed {CurrentLocaleSource.Id} source, game active locale: {GameActiveLocaleId}");
    }

    public Dictionary<string, string> GetLocalizedOptions() {
        var activeLocale = CurrentLocaleSource.Id;
        Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
        foreach (var item in LocaleSources) {
            if (item.Key == activeLocale) {
                keyValuePairs.Add(item.Key, item.Value.GetLocalizedName());
            }
            else {
                keyValuePairs.Add(item.Key, $"{item.Value.GetLocalizedName()} ({CurrentLocaleSource.GetLocaleIdValue(LocaleSource.GetLocaleIdKey(item.Key))})");
            }
        }
        return keyValuePairs;
    }

    public bool GameSupportsLocale(string localeId) => GameLocalizationManager.SupportsLocale(localeId);

    public string[] GetSupportedLocales() => GameLocalizationManager.GetSupportedLocales();

    public string GetTranslationProgressPercentage() => CurrentLocaleSource.IsDefault ? "100%" : CurrentLocaleSource.TranslationProgress;

    public string GetTranslationProgressPercentage(Dictionary<string, string> langDictionary) => $"{GetTranslationProgress(langDictionary)}%";

    public int GetTranslationProgress(Dictionary<string, string> langDictionary) => (int)((double)CountTranslatedItems(langDictionary) / LocaleSources[LocaleSource.EN_LOCALE_ID].Source.Count * 100);

    private int CountTranslatedItems(Dictionary<string, string> langDictionary) {
        var enDictionary = LocaleSources[LocaleSource.EN_LOCALE_ID].Source;
        return langDictionary.Count(_ => enDictionary.ContainsKey(_.Key) && !string.IsNullOrEmpty(_.Value) && _.Value != enDictionary[_.Key]);
    }

    private void LoadAllLocaleSource() {
        StringBuilder stringBuilder = new();
        stringBuilder.Append("Game supported locales: ");
        GetSupportedLocales().ForEach(_ => stringBuilder.Append($"{_} "));
        stringBuilder.AppendLine();
        stringBuilder.Append("Added Locale source: ");
        LocaleSources.Clear();
        LocaleSources.Add(LocaleSource.EN_LOCALE_ID, ModSetting.LocaleSource);
        stringBuilder.Append($"{LocaleSource.EN_LOCALE_ID} ");
        var directory = Path.Combine(Path.GetDirectoryName(ModPath), "Localization");
        if (!Directory.Exists(directory)) {
            Logger.Warn("Localization folder not found");
            return;
        }
        foreach (var file in new DirectoryInfo(directory).GetFiles("*.json")) {
            var localeID = Path.GetFileNameWithoutExtension(file.Name);
            if (!string.IsNullOrEmpty(localeID)) {
                var json = File.ReadAllText(file.FullName);
                var source = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                ENLocaleSource.Source.Where(_ => !source.ContainsKey(_.Key)).ToList().ForEach(_ => {
                    Logger.Warn($"The {localeID} source missing key [{_.Key}] is now filled");
                    source.Add(_.Key, _.Value);
                });
                var localeSource = new LocaleSource(localeID, source);
                localeSource.TranslationProgress = GetTranslationProgressPercentage(source);
                LocaleSources.Add(localeID, localeSource);
                stringBuilder.Append($"{localeID} ");
            }
        }
        Logger.Info(stringBuilder.ToString());
    }

    public void OnCreate(string t1, ILocalization t2) {
        ModPath = t1;
        ModSetting = t2;
        Logger = LogManager.GetLogger(AssemblyTools.CurrentAssemblyName);
        GameLocalizationManager = GameManager.instance.localizationManager;
        LocaleSources = new();
        LoadAllLocaleSource();
        InitLocale();
        IsLoaded = true;
    }

    public void OnCreate() { }

    public void OnUpdate() { }

    public void OnReset() { }

    public void OnDestroy() {
        GameLocalizationManager.onActiveDictionaryChanged -= OnActiveDictionaryChanged;
        IsLoaded = false;
    }
}