using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace Forza_Mods_AIO.Resources.Keybinds;

// https://github.com/AngryCarrot789/KeyDownTester/blob/master/KeyDownTester/Keys/HotkeysManager.cs
public static partial class HotkeysManager
{
    private static List<GlobalHotkey> Hotkeys { get; }
    private static readonly LowLevelKeyboardProc LowLevelProc = HookCallback;
    private static IntPtr _hookId = IntPtr.Zero;
    
    static HotkeysManager()
    {
        Hotkeys = new List<GlobalHotkey>();
    }

    public static void SetupSystemHook()
    {
        _hookId = SetHook(LowLevelProc);
        if (_hookId > 0) return;
        MessageBox.Show("Couldn't setup the keybinding hook.","Information", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public static void ShutdownSystemHook()
    {
        if (_hookId <= 0) return;
        UnhookWindowsHookEx(_hookId);
    }

    public static void AddHotkey(GlobalHotkey hotkey)
    {
        Hotkeys.Add(hotkey);
    }

    public static void RemoveHotkey(GlobalHotkey hotkey)
    {
        Hotkeys.Remove(hotkey);
    }

    public static bool CheckIfTheSameHotkeyExists(Key key, ModifierKeys modifierKeys)
    {
        return Hotkeys.Any(globalHotkey => globalHotkey.Key == key && globalHotkey.Modifier == modifierKeys);
    }

    private static async void CheckHotkeys()
    {
        await Application.Current.Dispatcher.InvokeAsync(async () =>
        {
            foreach (var hotkey in Hotkeys
                         .Where(hotkey => Keyboard.Modifiers == hotkey.Modifier && hotkey.Key != Key.None)
                         .Where(hotkey => hotkey.CanExecute))
            {
                if (hotkey.IsPressed)
                {
                    continue;
                }

                hotkey.IsPressed = true;
                while (Keyboard.IsKeyDown(hotkey.Key))
                {
                    hotkey.Callback();
                    await Task.Delay(hotkey.Interval);
                }
                hotkey.IsPressed = false;
            }
        });
    }

    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using var curProcess = Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule;
        return curModule == null ? 0 : SetWindowsHookEx(13, proc, Imports.GetModuleHandle(curModule.ModuleName), 0);
    }

    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            Task.Run(CheckHotkeys);
        }
        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    #region Native Methods

    [LibraryImport("user32.dll", EntryPoint = "SetWindowsHookExA", SetLastError = true)]
    private static partial IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial void UnhookWindowsHookEx(IntPtr hhk);

    [LibraryImport("user32.dll", SetLastError = true)]
    private static partial IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    #endregion
}