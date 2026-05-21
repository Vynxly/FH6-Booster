using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Forza_Mods_AIO.Resources.Theme;
using Forza_Mods_AIO.Services;
using Forza_Mods_AIO.Views.Windows;

namespace Forza_Mods_AIO.ViewModels.Pages;

public partial class AioInfoViewModel : ObservableObject
{
    [RelayCommand]
    private static void LaunchUrl(string param) => Process.Start("explorer.exe",$"\"{param}\"");

    [RelayCommand]
    private static void ChangeMonet() => Theming.GetInstance().ChangeColor();

    [RelayCommand]
    private static void ShowDebugWindow() => WindowsProviderService.Show<DebugWindow>();
}