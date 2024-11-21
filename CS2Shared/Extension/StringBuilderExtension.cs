using System;
using System.Collections.Generic;
using System.Text;

namespace CS2Shared.Extension;

public static class StringBuilderExtension {
    public static string ToString(this StringBuilder stringBuilder, Action<StringBuilder> action, bool clear = true) {
        if (clear)
            stringBuilder.Clear();
        action(stringBuilder);
        return stringBuilder.ToString();
    }

    public static string ToString(this StringBuilder stringBuilder, List<Action<StringBuilder>> actions) {
        stringBuilder.Clear();
        foreach (var action in actions) {
            action(stringBuilder);
        }
        return stringBuilder.ToString();
    }
}