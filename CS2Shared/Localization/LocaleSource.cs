using Colossal;
using Game.SceneFlow;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CS2Shared.Localization;

public class LocaleSource : IDictionarySource, IEnumerable {
    public const string EN_LOCALE_ID = "en-US";
    public string Id { get; set; }
    public Dictionary<string, string> Source { get; set; }
    public int Count => Source.Count;
    public bool IsDefault { get; private set; }
    public string TranslationProgress { get; set; } = "100%";

    public LocaleSource(string id) {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentNullException("Locale Data Id cannot be null or empty");
        Id = id;
        Source = new Dictionary<string, string>();
    }

    public LocaleSource(string id, Dictionary<string, string> source) {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentNullException("Locale Data Id cannot be null or empty");
        Id = id;
        Source = source;
    }

    public LocaleSource(string id, Dictionary<string, string> source, bool isDefault) : this(id, source) {
        IsDefault = isDefault;
    }

    public LocaleSource(Dictionary<string, string> source) {
        Id = EN_LOCALE_ID;
        Source = source;
        IsDefault = true;
    }

    public string GetLocalizedName() {
        string localizedID = Id;
        if (Source.TryGetValue(GetLocaleIdKey(Id), out var localized)) {
            localizedID = localized;
        }
        return localizedID;
    }

    public void Add(string entry, string value) {
        if (entry is null)
            throw new ArgumentNullException("Locale entry cannot be null");
        Source.Add(entry, value);
    }

    public void AddRange(Dictionary<string, string> source) {
        if (source is null)
            return;
        foreach (var e in source) {
            if (!Source.ContainsKey(e.Key))
                Source.Add(e.Key, e.Value);
        }
    }

    public bool TryGetValue(string entry, out string value) => Source.TryGetValue(entry, out value);

    public bool ContainsKey(string entry) => Source.ContainsKey(entry);

    public void Clear() => Source.Clear();

    public bool MergeFrom(Dictionary<string, string> source) {
        if (source is null)
            return false;
        foreach (var item in source) {
            if (!Source.ContainsKey(item.Key)) {
                Source.Add(item.Key, item.Value);
            }
        }
        return true;
    }

    public bool MergeFrom(LocaleSource localeSource) {
        if (localeSource is null || string.IsNullOrEmpty(localeSource.Id) || localeSource.Source is null || !localeSource.Source.Any())
            return false;
        foreach (var item in localeSource.Source) {
            if (!Source.ContainsKey(item.Key)) {
                Source.Add(item.Key, item.Value);
            }
        }
        return true;
    }

    public string GetLocaleIdValue(string localeId) {
        if (localeId is null) {
            return string.Empty;
        }
        if (Source.TryGetValue(localeId, out var value)) {
            return value;
        }
        return string.Empty;
    }

    public static string GetLocaleIdKey(string key) => $"Options.Language[{key}]";

    public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts) => Source;

    public void Unload() { }

    IEnumerator IEnumerable.GetEnumerator() => Source.GetEnumerator();

}
