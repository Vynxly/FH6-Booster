using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Forza_Mods_AIO.Cheats.ForzaHorizon4;
using Forza_Mods_AIO.Models;
using MahApps.Metro.Controls;
using static Forza_Mods_AIO.Resources.Cheats;

namespace Forza_Mods_AIO.ViewModels.Windows;

public partial class DebugWindowViewModel : ObservableObject
{
    private bool _isInitialized;

    public ObservableCollection<DebugSession> DebugSessions => Resources.DebugSessions.GetInstance().EveryDebugSession;
    
    [ObservableProperty]
    private DebugSession _currentDebugSession = null!;

    [ObservableProperty]
    private bool _areAnyBreakpointsAvailable;

    [ObservableProperty]
    private string _windowTitle = string.Empty;
    
    public bool IsFh4 => GameVerPlat.GetInstance().Type == GameVerPlat.GameType.Fh4;
    public bool IsFh5 => GameVerPlat.GetInstance().Type == GameVerPlat.GameType.Fh5;

    [RelayCommand]
    private static void DisableCrt()
    {
        GetClass<Bypass>().DisableCreateRemoteThreadChecks();
    }
    
    [RelayCommand]
    private static async Task DisableCrc()
    {
        await GetClass<Cheats.ForzaHorizon5.Bypass>().DisableCrcChecks();
    }
    
    [RelayCommand]
    private static async Task DisableEncryption()
    {
        await GetClass<Cheats.ForzaHorizon5.ValueEncryption>().CheatDisableValueEncryption();
    }
    
    public DebugWindowViewModel()
    {
        if (_isInitialized) return;
        InitializeViewModel();
    }

    private void InitializeViewModel()
    {
        WindowTitle = $"{App.GetRequiredService<MetroWindow>().Title} - Debug Window";
        _isInitialized = true;
    }
}