using Colossal.Logging;
using CS2Shared.Common;
using CS2Shared.Extension;
using CS2Shared.Localization;
using CS2Shared.Manager;
using CS2Shared.Tools;
using Game;
using Game.Modding;
using Game.SceneFlow;
using Game.Settings;
using Game.UI.Menu;
using Game.UI.Widgets;
using System;
using System.Collections.Generic;
using Unity.Entities;

namespace CS2Shared.Settings;

public abstract class ModSettingBase : ModSetting, IModSetting {
    public event Func<string, IModSetting> EventBeforeRegister;
    public event Func<string, IModSetting> EventAfterRegister;
    public event Func<string, IModSetting> EventBeforeUnregister;
    public event Func<string, IModSetting> EventAfterUnregister;

    protected const string Main = nameof(Main);
    protected const string General = nameof(General);
    protected const string Advanced = nameof(Advanced);
    protected const string Debug = nameof(Debug);
    protected const string ModInfo = nameof(ModInfo);
    protected const string KeyBindings = nameof(KeyBindings);
    protected const string About = nameof(About);
    protected const string Reset = nameof(Reset);
    protected const string Serialize = nameof(Serialize);

    [SettingsUIHidden]
    public string Id => id;
    [SettingsUIHidden]
    public bool IsRegistered { get; protected set; }
    public LocaleSource LocaleSource { get; set; }
    protected GameMode Mode => GameManager.instance.gameMode;
    public bool InGame => IsInGame();
    public bool NotInGame => !IsInGame();
    public bool InEditor => IsInEditor();
    public bool InGameOrEditor => IsInGameOrEditor();
    public bool InMainMenu => IsInMainMenu();

    protected ModSettingBase(IMod mod) : base(mod) {
        CreateLocaleSource();
        SetDefaults();
    }

    [SettingsUISection(General, ModInfo)]
    public string Version => AssemblyTools.CurrentAssemblyVersion.ToString(3);



#if DEBUG
    [SettingsUISection(General, ModInfo)]
    public string ReleaseDate => ModBase.Instance is null ? "Null" : DataDirectory.GetModDirectoryCreationTime(ModBase.Instance.ModAsset);
#else
    [SettingsUISection(General, ModInfo)]
    public string ReleaseDate => ModBase.Instance is null ? "Null" : ModBase.Instance.VersionDate.ToString("yyyy/MM/dd");
#endif

    [SettingsUISection(General, ModInfo)]
    [SettingsUIDropdown(typeof(ModSettingBase), nameof(GetLocaleIdOptions))]
    [SettingsUISetter(typeof(ModSettingBase), nameof(ChangeLocale))]
    public string LocaleId { get; set; }

    private void ChangeLocale(string localeId) {
        ManagerPool.GetOrCreateManager<ModLocalizationManager>().OnLocaleOptionChanged(localeId);
        LocalizationReload();
    }

    private DropdownItem<string>[] GetLocaleIdOptions() {
        var all = new List<DropdownItem<string>>() {
            new(){
                value = ModLocalizationManager.USE_GAME_LANGUAGE,
                displayName = GeLocaleOptionLocaleID(ModLocalizationManager.USE_GAME_LANGUAGE)
            }
        };
        //foreach (var item in ManagerPool.GetOrCreateManager<ModLocalizationManager>().LocaleSources) {
        //    all.Add(new DropdownItem<string>() { value = item.Key, displayName = item.Value.GetLocalizedName() });
        //}
        foreach (var item in ManagerPool.GetOrCreateManager<ModLocalizationManager>().GetLocalizedOptions()) {
            all.Add(new DropdownItem<string>() { value = item.Key, displayName = item.Value });
        }
        return all.ToArray();
    }

    [SettingsUISection(General, ModInfo)]
    public string TranslationProgress => ManagerPool.GetOrCreateManager<ModLocalizationManager>().GetTranslationProgressPercentage();

    public override void SetDefaults() {
        LocaleId = ModLocalizationManager.USE_GAME_LANGUAGE;
    }

    [SettingsUIButton]
    [SettingsUISection(Advanced, Reset)]
    [SettingsUIConfirmation]
    public bool ResetBindings {
        set {
            LogManager.GetLogger(AssemblyTools.CurrentAssemblyName).Info("Reset key bindings");
            ResetKeyBindings();
        }
    }

    [SettingsUIButton]
    [SettingsUISection(Advanced, Reset)]
    [SettingsUIConfirmation]
    public bool ResetDefault {
        set {
            LogManager.GetLogger(AssemblyTools.CurrentAssemblyName).Info("Reset setting");
            SetDefaults();
            ApplyAndSave();
        }
    }

#if DEBUG
    [SettingsUISection(Debug, Serialize)]
    public bool SerializeLocaleSource { set => ManagerPool.GetOrCreateManager<ModLocalizationManager>().SerializeLocale(); }
#endif

    protected bool IsInGameOrEditor() => Mode == GameMode.GameOrEditor;

    protected bool IsInEditor() => Mode == GameMode.Editor;

    protected bool IsInGame() => Mode == GameMode.Game;

    protected bool IsInMainMenu() => Mode == GameMode.MainMenu;

    public virtual void SetBindingAction() { }

    private string GeLocaleOptionLocaleID(string localeId) => "Options.Locale[" + id + "." + localeId + "]";

    public string GetOptionLocaleID(string localeId) => $"Options[{id}.{localeId}]";

    public void AddLocaleSource(string key, string value) => LocaleSource.Add(key, value);

    public void AddLocaleSource(Dictionary<string, string> pairs) => LocaleSource.AddRange(pairs);

    protected virtual void CreateLocaleSource() {
        LocaleSource = new LocaleSource(new Dictionary<string, string>() {
            { GetOptionTabLocaleID(Main), "Main" },
            { GetOptionTabLocaleID(General), "General" },
            { GetOptionTabLocaleID(KeyBindings), "Key Bindings" },
            { GetOptionTabLocaleID(Advanced), "Advanced" },
            { GetOptionTabLocaleID(Debug), "Debug" },

            { GetOptionGroupLocaleID(About), "About" },
            { GetOptionGroupLocaleID(Reset), "Reset" },
            { GetOptionGroupLocaleID(Serialize), "Serialize" },
            { GetOptionGroupLocaleID(ModInfo), "Mod Info" },

            { GetOptionLabelLocaleIDExtension(nameof(Version)), "Version" },
            { GetOptionDescLocaleIDExtension(nameof(Version)), "Current mod version" },
            { GetOptionLabelLocaleIDExtension(nameof(ReleaseDate)), "Release Date" },
            { GetOptionDescLocaleIDExtension(nameof(ReleaseDate)), "Mod Release Date" },
            { GetOptionLabelLocaleIDExtension(nameof(LocaleId)),"Language"},
            { GetOptionDescLocaleIDExtension(nameof(LocaleId)),"Select the mod language from the drop-down list."},
            { LocaleSource.GetLocaleIdKey("de-DE"), "German" },
            { LocaleSource.GetLocaleIdKey("en-US"), "English" },
            { LocaleSource.GetLocaleIdKey("es-ES"), "Spanish" },
            { LocaleSource.GetLocaleIdKey("fr-FR"), "French" },
            { LocaleSource.GetLocaleIdKey("it-IT"), "Italian" },
            { LocaleSource.GetLocaleIdKey("ja-JP"), "Japanese" },
            { LocaleSource.GetLocaleIdKey("ko-KR"), "Korean" },
            { LocaleSource.GetLocaleIdKey("pl-PL"), "Polish" },
            { LocaleSource.GetLocaleIdKey("pt-BR"), "Brazilian Portuguese" },
            { LocaleSource.GetLocaleIdKey("ru-RU"), "Russian" },
            { LocaleSource.GetLocaleIdKey("zh-HANS"), "Simplified Chinese" },
            { LocaleSource.GetLocaleIdKey("zh-HANT"), "Traditional Chinese" },
            { LocaleSource.GetLocaleIdKey("ar"), "Arabic" },
            { LocaleSource.GetLocaleIdKey("nl-NL"), "Dutch" },
            { LocaleSource.GetLocaleIdKey("ms-MY"), "Malay" },
            { LocaleSource.GetLocaleIdKey("th-TH"), "Thai" },
            { LocaleSource.GetLocaleIdKey("tr-TR"), "Turkish" },
            { LocaleSource.GetLocaleIdKey("sk-SK"), "Slovak" },
            { LocaleSource.GetLocaleIdKey("pt-PT"), "Portuguese" },
            { GeLocaleOptionLocaleID(ModLocalizationManager.USE_GAME_LANGUAGE),"Use game language" },
            { GetOptionLabelLocaleIDExtension(nameof(TranslationProgress)), "Translation Progress" },
            { GetOptionDescLocaleIDExtension(nameof(TranslationProgress)), "Percentage of the current language translated, if you want to add, improve or correct translations please go to Crowdin." },

            { GetOptionLabelLocaleIDExtension("SerializeLocaleSource"), "Serialize locale" },
            { GetOptionDescLocaleIDExtension("SerializeLocaleSource"), "" },

            { GetOptionLabelLocaleIDExtension(nameof(ResetDefault)), "Reset default" },
            { GetOptionDescLocaleIDExtension(nameof(ResetDefault)), "Reset all configurations of the mod." },
            { GetOptionWarningLocaleIDExtension(nameof(ResetDefault)), "Are you sure you want to reset all config? This operation is not reversible!" },

            { GetOptionLabelLocaleIDExtension(nameof(ResetBindings)), "Reset key bindings" },
            { GetOptionDescLocaleIDExtension(nameof(ResetBindings)), "Reset all key bindings of the mod." },
            { GetOptionWarningLocaleIDExtension(nameof(ResetBindings)),"Are you sure you want to reset all key bindings? This operation is not reversible!" }
            });
    }

    public string GetOptionLabelLocaleIDExtension(string optionName) => "Options.OPTION[" + id + "." + nameof(ModSettingBase) + "." + optionName + "]";
    public string GetOptionDescLocaleIDExtension(string optionName) => "Options.OPTION_DESCRIPTION[" + id + "." + nameof(ModSettingBase) + "." + optionName + "]";
    public string GetOptionWarningLocaleIDExtension(string optionName) => "Options.WARNING[" + id + "." + nameof(ModSettingBase) + "." + optionName + "]";
    public static string GetOptionLanguageLocaleIDExtension(string id) => LocaleSource.GetLocaleIdKey(id);

    public string GetUILocaleID(string id) => $"{AssemblyTools.CurrentAssemblyName}.UI[{id}]";

    public virtual void Register() {
        if (IsRegistered)
            return;
        EventBeforeRegister?.Invoke(Id);
        RegisterInOptionsUI();
        RegisterKeyBindings();
        SetBindingAction();
        IsRegistered = true;
        EventAfterRegister?.Invoke(Id);
    }

    public void LocalizationReload() {
        UnregisterInOptionsUI();
        RegisterInOptionsUI();
        World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<OptionsUISystem>().OpenPage(Id, Id, false);
    }

    public virtual void Unregister() {
        if (!IsRegistered)
            return;
        EventBeforeUnregister?.Invoke(Id);
        UnregisterInOptionsUI();
        IsRegistered = false;
        EventAfterUnregister?.Invoke(Id);
    }

}