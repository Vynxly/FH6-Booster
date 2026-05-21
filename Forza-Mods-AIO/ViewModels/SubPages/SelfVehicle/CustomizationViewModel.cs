using CommunityToolkit.Mvvm.ComponentModel;
using Forza_Mods_AIO.Models;

namespace Forza_Mods_AIO.ViewModels.SubPages.SelfVehicle;

public partial class CustomizationViewModel : ObservableObject
{
    public bool IsFh5 => GameVerPlat.GetInstance().Type == GameVerPlat.GameType.Fh5;
    
    [ObservableProperty]
    private bool _areMainUiElementsEnabled = true;
    
    [ObservableProperty]
    private bool _areHeadlightUiElementsEnabled = true;
    
    [ObservableProperty]
    private bool _areBackfireUiElementsEnabled = true;
    
    [ObservableProperty]
    private bool _dirtEnabled;
    
    [ObservableProperty]
    private float _dirtValue;

    [ObservableProperty]
    private bool _mudEnabled;
    
    [ObservableProperty]
    private float _mudValue;

    [ObservableProperty]
    private bool _glowingPaintEnabled;
    
    [ObservableProperty]
    private float _glowingPaintValue;

    [ObservableProperty]
    private bool _forceLodEnabled;
    
    [ObservableProperty]
    private int _forceLodValue;
}