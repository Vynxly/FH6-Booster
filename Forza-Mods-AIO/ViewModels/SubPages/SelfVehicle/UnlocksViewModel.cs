using CommunityToolkit.Mvvm.ComponentModel;
using Forza_Mods_AIO.Models;

namespace Forza_Mods_AIO.ViewModels.SubPages.SelfVehicle;

public partial class UnlocksViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _areUiElementsEnabled = true;

    [ObservableProperty]
    private bool _wasComboBoxLoaded;

    [ObservableProperty]
    private bool _isXpEnabled;
    
    [ObservableProperty]
    private bool _isCreditsEnabled;
    
    [ObservableProperty]
    private bool _isSkillPointsEnabled;
    
    [ObservableProperty]
    private bool _isWheelspinsEnabled;
    
    [ObservableProperty]
    private bool _isAccoladesEnabled;
    
    [ObservableProperty]
    private bool _isKudosEnabled;
    
    [ObservableProperty]
    private bool _isForzathonEnabled;
    
    [ObservableProperty]
    private bool _isSeriesEnabled;
    
    [ObservableProperty]
    private bool _isSeasonalEnabled;
    
    [ObservableProperty]
    private int _xpValue;
    
    [ObservableProperty]
    private int _creditsValue;
    
    [ObservableProperty]
    private int _skillPointsValue;
    
    [ObservableProperty]
    private int _wheelspinsValue;
    
    [ObservableProperty]
    private int _accoladesValue;
    
    [ObservableProperty]
    private int _kudosValue;
    
    [ObservableProperty]
    private int _forzathonValue;
    
    [ObservableProperty]
    private int _seriesValue;
    
    [ObservableProperty]
    private int _seasonalValue;
    
    public bool IsFh5 => GameVerPlat.GetInstance().Type == GameVerPlat.GameType.Fh5;
    
    public bool IsHorizon => GameVerPlat.GetInstance().Type is GameVerPlat.GameType.Fh4 or GameVerPlat.GameType.Fh5;
}