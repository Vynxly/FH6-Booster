using CommunityToolkit.Mvvm.ComponentModel;
using Forza_Mods_AIO.Models;

namespace Forza_Mods_AIO.ViewModels.SubPages.SelfVehicle;

public partial class CameraViewModel : ObservableObject
{
    public bool IsFh5 => GameVerPlat.GetInstance().Type == GameVerPlat.GameType.Fh5;
    
    [ObservableProperty]
    private bool _areScanPromptLimiterUiElementsVisible = true;
    
    [ObservableProperty]
    private bool _areScanningLimiterUiElementsVisible;
    
    [ObservableProperty]
    private bool _areLimiterUiElementsVisible;
    
    [ObservableProperty]
    private bool _areCameraHookUiElementsEnabled = true;
}