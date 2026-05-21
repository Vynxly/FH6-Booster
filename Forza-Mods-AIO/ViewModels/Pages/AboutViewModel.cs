using System.Diagnostics;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Forza_Mods_AIO.ViewModels.Pages;

public partial class AboutViewModel : ObservableObject
{
    [ObservableProperty]
    private string _version = $"Version: {Assembly.GetExecutingAssembly().GetName().Version!.ToString()}";
    
    [RelayCommand]
    private static void LaunchUrl(string param) => Process.Start("explorer.exe",$"\"{param}\"");
}