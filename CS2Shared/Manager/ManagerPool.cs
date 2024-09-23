using Colossal.Logging;
using CS2Shared.Tools;
using System;
using System.Collections.Generic;

namespace CS2Shared.Manager;

public static class ManagerPool {
    private static readonly Dictionary<Type, IManager> buffer = new();

    public static event Action<Type, IManager> OnManagerCreated;
    public static event Action<Type, IManager> OnManagerReset;
    public static event Action<Type, IManager> OnManagerReleased;

    private static ILog Logger { get; } = LogManager.GetLogger(AssemblyTools.CurrentAssemblyName);

    public static bool HasManager<T>() => buffer.ContainsKey(typeof(T));
    public static IEnumerable<Type> GetAllManagerTypes() => buffer.Keys;
    public static IEnumerable<IManager> GetAllManagers() => buffer.Values;
    public static int GetManagerCount() => buffer.Count;

    public static T GetOrCreateManager<T>() where T : IManager, new() {
        if (buffer.TryGetValue(typeof(T), out IManager manager))
            return (T)manager;
        var newManager = new T();
        Logger.Info($"ManagerPool created manager: {typeof(T)}");
        newManager.OnCreate();
        OnManagerCreated?.Invoke(typeof(T), newManager);
        buffer[typeof(T)] = newManager;
        return newManager;
    }

    public static T GetOrCreateManager<T, TParam>(TParam param) where T : IManager<TParam>, new() {
        if (buffer.TryGetValue(typeof(T), out IManager manager))
            return (T)manager;
        var newManager = new T();
        Logger.Info($"ManagerPool created manager: {typeof(T)}");
        newManager.OnCreate(param);
        OnManagerCreated?.Invoke(typeof(T), newManager);
        buffer[typeof(T)] = newManager;
        return newManager;
    }

    public static T GetOrCreateManager<T, TParam1, TParam2>(TParam1 param1, TParam2 param2) where T : IManager<TParam1, TParam2>, new() {
        if (buffer.TryGetValue(typeof(T), out IManager manager))
            return (T)manager;
        var newManager = new T();
        Logger.Info($"ManagerPool created manager: {typeof(T)}");
        newManager.OnCreate(param1, param2);
        OnManagerCreated?.Invoke(typeof(T), newManager);
        buffer[typeof(T)] = newManager;
        return newManager;
    }

    public static T GetOrCreateManager<T, TParam1, TParam2, TParam3>(TParam1 param1, TParam2 param2, TParam3 param3) where T : IManager<TParam1, TParam2, TParam3>, new() {
        if (buffer.TryGetValue(typeof(T), out IManager manager))
            return (T)manager;
        var newManager = new T();
        Logger.Info($"ManagerPool created manager: {typeof(T)}");
        newManager.OnCreate(param1, param2, param3);
        OnManagerCreated?.Invoke(typeof(T), newManager);
        buffer[typeof(T)] = newManager;
        return newManager;
    }

    public static T GetOrCreateManager<T, TParam1, TParam2, TParam3, TParam4>(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4) where T : IManager<TParam1, TParam2, TParam3, TParam4>, new() {
        if (buffer.TryGetValue(typeof(T), out IManager manager))
            return (T)manager;
        var newManager = new T();
        Logger.Info($"ManagerPool created manager: {typeof(T)}");
        newManager.OnCreate(param1, param2, param3, param4);
        OnManagerCreated?.Invoke(typeof(T), newManager);
        buffer[typeof(T)] = newManager;
        return newManager;
    }

    public static bool ResetManager<T>() where T : IManager {
        if (buffer.TryGetValue(typeof(T), out IManager manager)) {
            manager.OnReset();
            OnManagerReset?.Invoke(typeof(T), manager);
            return true;
        }
        return false;
    }

    public static void ResetAllManagers() {
        foreach (var manager in buffer.Values) {
            manager.OnReset();
        }
    }

    public static void DestroyManager<T>() where T : IManager {
        if (buffer.TryGetValue(typeof(T), out IManager manager)) {
            manager.OnDestroy();
            OnManagerReleased?.Invoke(typeof(T), manager);
            buffer.Remove(typeof(T));
        }
    }

    public static void DestroyAllManager() {
        foreach (var manager in buffer.Values) {
            manager.OnDestroy();
        }
        buffer.Clear();
    }

    public static T GetManagerOrNull<T>() where T : IManager {
        if (buffer.TryGetValue(typeof(T), out var manager))
            return (T)manager;
        return default;
    }

    public static void UpdateManager<T>() where T : IManager {
        if (buffer.TryGetValue(typeof(T), out var manager))
            manager.OnUpdate();
    }

    public static void UpdateManagers() {
        foreach (var manager in buffer.Values) {
            manager.OnUpdate();
        }
    }

    public static void UpdateManager<T>(Action<T> updateAction) where T : IManager {
        if (buffer.TryGetValue(typeof(T), out IManager manager)) {
            updateAction?.Invoke((T)manager);
        }
    }

    public static IEnumerable<T> FilterManagers<T>(Func<T, bool> predicate) where T : IManager {
        foreach (var manager in buffer.Values) {
            if (manager is T typedManager && predicate(typedManager)) {
                yield return typedManager;
            }
        }
    }

}