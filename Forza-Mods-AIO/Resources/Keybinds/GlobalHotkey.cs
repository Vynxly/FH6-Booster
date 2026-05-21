using System.Windows.Input;

namespace Forza_Mods_AIO.Resources.Keybinds;

// https://github.com/AngryCarrot789/KeyDownTester/blob/master/KeyDownTester/Keys/GlobalHotkey.cs
public class GlobalHotkey(ModifierKeys modifier, Key key, Action callbackMethod, int interval = 250, bool canExecute = false)
{
    public ModifierKeys Modifier { get; set; } = modifier;
    public Key Key { get; set; } = key;
    public Action Callback { get; } = callbackMethod;
    public bool CanExecute { get; set; } = canExecute;
    public bool IsPressed { get; set; }
    public int Interval { get; set; } = interval;
}