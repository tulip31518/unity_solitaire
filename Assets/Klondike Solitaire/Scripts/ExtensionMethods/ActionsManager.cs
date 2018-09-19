using System;
using UnityEngine;

public static class ActionsManager
{
    public static void RunAction(this Action action)
    {
        if (action != null)
        {
            action();
        }
    }

    public static void RunAction<T>(this Action<T> action, T argument)
    {
        if (action != null)
        {
            action(argument);
        }
    }

    public static void RunAction<T,U>(this Action<T,U> action, T argument1, U argument2)
    {
        if (action != null)
        {
            action(argument1,argument2);
        }
    }

    public static T ToEnum<T>(this string value, T defaultValue, bool ignoreCase = true)
    {
        if (string.IsNullOrEmpty(value))
        {
            return defaultValue;
        }

        return (T)Enum.Parse(typeof(T), value, ignoreCase);
    }

   
}
