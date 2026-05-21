using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Forza_Mods_AIO.Resources.Theme;

namespace Forza_Mods_AIO.ViewModels.Pages;

public partial class SettingsViewModel : ObservableObject
{
    private readonly Theming _theming;

    public SettingsViewModel()
    {
        _theming = Theming.GetInstance();
        _themeColor = _theming.MainColourAsColour;
    }

    [ObservableProperty]
    private Color _themeColor;    
    
    [RelayCommand]
    private void ChangeTheme()
    {
        _theming.ChangeColor(ThemeColor);
    }
    
    [RelayCommand]
    private void MonetTheme()
    {
        ThemeColor = (Color)ColorConverter.ConvertFromString("#0078D4");
        _theming.ChangeColor(ThemeColor);
    }

    [RelayCommand]
    private void ResetTheme()
    {
        _theming.ResetTheme();
    }
}