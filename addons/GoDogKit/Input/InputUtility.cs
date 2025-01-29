using System;
using System.Collections.Generic;
using Godot;
using Array = Godot.Collections.Array;

namespace GoDogKit;

/// <summary>
/// Provides additional input detect methods base on the Godot.
/// </summary>
public static class InputUtility
{
    private static readonly Dictionary<string, ulong> m_pressed_caches;
    private static readonly Dictionary<string, (int, ulong)> m_pressed_count_caches;
    private static readonly Dictionary<string, (int, ulong)> m_released_count_caches;

    static InputUtility()
    {
        m_pressed_caches = [];
        m_pressed_count_caches = [];
        m_released_count_caches = [];
    }

    // Returns in seconds.
    private static double GetElapseFrom(ulong ticks)
        => ticks <= 0 ? 0 : TimeSpan.FromMilliseconds(Time.GetTicksMsec() - ticks).TotalSeconds;

    #region PressedForSeconds

    private static void PressedStart(string action)
    {
        var ticks = Time.GetTicksMsec();

        if (Input.IsActionJustPressed(action))
        {
            if (!m_pressed_caches.TryAdd(action, ticks))
            {
                m_pressed_caches[action] = ticks;
            }
        }
    }

    private static bool PressedEnd(string action, float seconds, Func<bool> signal, Action callback)
    {
        if (signal())
        {
            if (m_pressed_caches.TryGetValue(action, out var ticks))
            {
                if (GetElapseFrom(ticks) >= seconds)
                {
                    callback?.Invoke();
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Return true if an action was pressed and held for *at least* a given duration in seconds,
    /// and right after the action released. It's considered as a delay version of 
    /// IsActionJustReleased().
    /// return false if not.
    /// </summary>
    public static bool IsActionPressedForSeconds(string action, float seconds)
    {
        PressedStart(action);

        return PressedEnd(action, seconds,
        () => Input.IsActionJustReleased(action), null);
    }

    /// <summary>
    /// Return true if an action was just pressed and held for *at least* a given duration in seconds,
    /// and right after duration reached the end. It's considered as a delay version of 
    /// IsActionJustPressed().
    /// return false if not.
    /// </summary>
    public static bool IsActionJustPressedForSeconds(string action, float seconds)
    {
        PressedStart(action);

        return PressedEnd(action, seconds,
        () => Input.IsActionPressed(action),
         delegate { m_pressed_caches[action] = ulong.MinValue; });
    }

    #endregion

    #region Continuous

    private static bool Continuous
    (
        Dictionary<string, (int, ulong)> caches,
        string action,
        int count,
        float interval
    )
    {
        var ticks = Time.GetTicksMsec();

        if (caches.TryGetValue(action, out (int, ulong) value))
        {
            // Don't use the 'Seconds' property in TimeSpan directly, which is wrong!
            // Use the 'Total' instead.
            var elapse = GetElapseFrom(ticks);

            // Indicate that elapse time within the interval.
            if (elapse <= interval)
            {
                var newValue = (++value.Item1, ticks);

                if (newValue.Item1 >= count)
                {
                    // Reset the count and ticks.
                    caches[action] = (1, ticks);

                    return true;
                }
                else caches[action] = newValue;
            }
            // Reset
            else
            {
                // Reset the count and ticks.
                caches[action] = (1, ticks);
            }
        }
        // If the action haven't been cached, cache it
        else
        {
            caches.Add(action, (1, ticks));
            if (1 >= count) return true;
        }

        return false;
    }

    /// <summary>
    /// Return true if an action has pressed continuously 'count' times, the later press
    /// must happen within a given interval, which was well tested to be 0.2 sec.
    /// It's considered as requesting IsActionJustPressed() mutliply times.
    /// Return false if not.
    /// </summary>    
    /// <param name="count"> The number of presses needed. </param>
    /// <param name="interval"> The minim interval between two presses. </param>
    public static bool IsActionJustPressedContinuous
    (string action, int count, float interval = 0.2f)
    {
        if (!Input.IsActionJustPressed(action)) return false;

        return Continuous(m_pressed_count_caches, action, count, interval);
    }

    /// <summary>
    /// Return true if an action has released continuously 'count' times, the later release
    /// must happen within a given interval, which was well tested to be 0.2 sec.
    /// It's considered as requesting IsActionJustReleased() mutliply times.
    /// Return false if not.
    /// </summary>
    /// <param name="count"> The number of releases needed. </param>
    /// <param name="interval"> The minim interval between two releases. </param>
    public static bool IsActionJustReleasedContinuous
    (string action, int count, float interval = 0.2f)
    {
        if (!Input.IsActionJustReleased(action)) return false;

        return Continuous(m_released_count_caches, action, count, interval);
    }

    #endregion

    #region InputMap

    /// <summary>
    /// Get all actions in the input map defined by user, ignores engine default 'ui_*' actions.
    /// </summary>
    public static List<string> GetDefinedActions()
    {
        List<string> actions = [];

        foreach (string action in InputMap.GetActions())
        {
            // Ignores the internal project settings.
            if (action.StartsWith("ui_")) continue;

            actions.Add(action);
        }

        return actions;
    }

    /// <summary>
    /// Save current input map into a given path as a config files.
    /// </summary>    
    public static void SaveInputMapConfig(string path)
    {
        ArchiveUtility.EnsurePathExists(path);

        using ConfigFile file = new();

        foreach (string action in GetDefinedActions())
        {
            // No needs to save the deadzone values.
            file.SetValue("input", action, InputMap.ActionGetEvents(action));
        }

        var error = file.Save(path);

        if (error is not Error.Ok) GD.Print(error);
    }

    /// <summary>
    /// Override current input map with a given path from a config files.
    /// </summary> 
    public static void LoadInputMapConfig(string path)
    {
        ArchiveUtility.EnsurePathExists(path);

        using ConfigFile file = new();

        var error = file.Load(path);

        if (error is not Error.Ok) GD.Print(error);

        foreach (string action in GetDefinedActions())
        {
            InputMap.ActionEraseEvents(action);

            foreach (InputEvent @event in (Array)file.GetValue("input", action))
            {
                InputMap.ActionAddEvent(action, @event);
                // GD.Print(@event.AsText());
            }
        }
    }

    #endregion
}