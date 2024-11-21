using Colossal.Annotations;
using Colossal.UI.Binding;
using CS2Shared.Tools;
using Game.UI;
using System;
using static Game.UI.NameSystem;

namespace CS2Shared.Common;

public abstract partial class UISystemBaseExtension : UISystemBase {
    public virtual string ModId { get; set; } = AssemblyTools.CurrentAssemblyName;

    public BoolBinding AddBoolBindingAndTriggerBinding(string name, bool initialValue, Action<bool> callback) {
        var boolBinding = AddBoolBinding(name, initialValue);
        AddBoolTriggerBinding(name, callback);
        return boolBinding;
    }

    public BoolBinding AddBoolBinding(string name, bool initialValue) {
        var boolBinding = new BoolBinding(ModId, name, initialValue);
        AddBinding(boolBinding.ValueBinding);
        return boolBinding;
    }

    public void AddBoolTriggerBinding(string name, [NotNull] Action<bool> callback) => AddTriggerBinding(name, callback);

    public TriggerBinding<T> AddTriggerBinding<T>(string name, [NotNull] Action<T> callback) {
        var triggerBinding = new TriggerBinding<T>(ModId, name, callback);
        AddBinding(triggerBinding);
        return triggerBinding;
    }

    public ValueBinding<T> AddValueBinding<T>(string name, T initialValue) {
        var valueBinding = new ValueBinding<T>(ModId, name, initialValue);
        AddBinding(valueBinding);
        return valueBinding;
    }

}


public class BoolBinding {
    public ValueBinding<bool> ValueBinding { get; private set; }

    public Action<bool> OnValueChanged;

    public bool Value {
        get => ValueBinding.value;
        set {
            if (value != ValueBinding.value) {
                Update(value);
                OnValueChanged?.Invoke(value);
            }
        }
    }

    public BoolBinding(string group, string name, bool initialValue) => ValueBinding = new(group, name, initialValue);

    public void Update() => Update(!ValueBinding.value);

    public void Update(bool value) => ValueBinding.Update(value);

}