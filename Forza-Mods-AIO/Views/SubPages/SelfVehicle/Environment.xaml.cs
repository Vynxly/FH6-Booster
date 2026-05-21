using System.Numerics;
using System.Windows;
using System.Windows.Media;
using Forza_Mods_AIO.Cheats.ForzaHorizon5;
using Forza_Mods_AIO.Models;
using Forza_Mods_AIO.ViewModels.SubPages.SelfVehicle;
using MahApps.Metro.Controls;
using static Forza_Mods_AIO.Resources.Cheats;
using static Forza_Mods_AIO.Resources.Memory;

namespace Forza_Mods_AIO.Views.SubPages.SelfVehicle;

public partial class Environment
{
    public Environment()
    {
        ViewModel = new EnvironmentViewModel();
        DataContext = this;
        
        InitializeComponent();
    }

    public EnvironmentViewModel ViewModel { get; }
    private static EnvironmentCheats EnvironmentCheatsFh5 => GetClass<EnvironmentCheats>();
    private static Cheats.ForzaHorizon4.EnvironmentCheats EnvironmentCheatsFh4 =>
        GetClass<Cheats.ForzaHorizon4.EnvironmentCheats>();
    private static CarCheats CarCheatsFh5 => GetClass<CarCheats>();
    
    private static Vector4 ConvertUiColorToGameValues(Color uiColor, double intensity)
    {
        var fIntensity = Convert.ToSingle(intensity);
        var alpha = uiColor.A / 255f;
        var red = uiColor.R / 255f * alpha * fIntensity;
        var green = uiColor.G / 255f * alpha * fIntensity;
        var blue = uiColor.B / 255f * alpha * fIntensity;
        return new Vector4(1 + red, 1 + green, 1 + blue, 1);        
    }

    private async void RgbSwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleSwitch toggleSwitch)
        {
            return;
        }

        ViewModel.AreSunRgbUiElementsEnabled = false;
        switch (GameVerPlat.GetInstance().Type)
        {
            case GameVerPlat.GameType.Fh4:
            {
                if (EnvironmentCheatsFh4.SunRgbDetourAddress == 0)
                {
                    await EnvironmentCheatsFh4.CheatSunRgb();
                }
                break;
            }
            case GameVerPlat.GameType.Fh5:
            {
                
                if (EnvironmentCheatsFh5.SunRgbDetourAddress == 0)
                {
                    await EnvironmentCheatsFh5.CheatSunRgb();
                }
                break;
            }
            case GameVerPlat.GameType.None:
            default:
                throw new IndexOutOfRangeException();
        }
        ViewModel.AreSunRgbUiElementsEnabled = true;
        
        var detourAddress = GameVerPlat.GetInstance().Type switch
        {
            GameVerPlat.GameType.Fh4 => EnvironmentCheatsFh4.SunRgbDetourAddress,
            GameVerPlat.GameType.Fh5 => EnvironmentCheatsFh5.SunRgbDetourAddress,
            _ => throw new IndexOutOfRangeException()
        };
        
        if (detourAddress == 0) return;
        GetInstance().WriteMemory(detourAddress + 0x32, toggleSwitch.IsOn ? (byte)1 : (byte)0);
        GetInstance().WriteMemory(detourAddress + 0x33, ConvertUiColorToGameValues(Picker.SelectedColor.GetValueOrDefault(), IntensityBox.Value.GetValueOrDefault()));
    }

    private void Picker_OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
    {        
        var detourAddress = GameVerPlat.GetInstance().Type switch
        {
            GameVerPlat.GameType.Fh4 => EnvironmentCheatsFh4.SunRgbDetourAddress,
            GameVerPlat.GameType.Fh5 => EnvironmentCheatsFh5.SunRgbDetourAddress,
            _ => throw new IndexOutOfRangeException()
        };
        
        if (detourAddress == 0) return;
        GetInstance().WriteMemory(detourAddress + 0x33, ConvertUiColorToGameValues(Picker.SelectedColor.GetValueOrDefault(), IntensityBox.Value.GetValueOrDefault()));
    }

    private async void PullButton_OnClick(object sender, RoutedEventArgs e)
    {
        ViewModel.AreManualTimeUiElementsEnabled = false;
        switch (GameVerPlat.GetInstance().Type)
        {
            case GameVerPlat.GameType.Fh4:
            {
                if (EnvironmentCheatsFh4.TimeDetourAddress == 0)
                {
                    await EnvironmentCheatsFh4.CheatTime();
                }
                break;
            }
            case GameVerPlat.GameType.Fh5:
            {
                if (EnvironmentCheatsFh5.TimeDetourAddress == 0)
                {
                    await EnvironmentCheatsFh5.CheatTime();
                }
                break;
            }
            case GameVerPlat.GameType.None:
            default:
                throw new IndexOutOfRangeException();
        }
        ViewModel.AreManualTimeUiElementsEnabled = true;
        
        var detourAddress = GameVerPlat.GetInstance().Type switch
        {
            GameVerPlat.GameType.Fh4 => EnvironmentCheatsFh4.TimeDetourAddress,
            GameVerPlat.GameType.Fh5 => EnvironmentCheatsFh5.TimeDetourAddress,
            _ => throw new IndexOutOfRangeException()
        };
        
        if (detourAddress == 0) return;
        TimeBox.Value = GetInstance().ReadMemory<double>(detourAddress + 0x2C);
    }

    private void TimeBox_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
    {
        var detourAddress = GameVerPlat.GetInstance().Type switch
        {
            GameVerPlat.GameType.Fh4 => EnvironmentCheatsFh4.TimeDetourAddress,
            GameVerPlat.GameType.Fh5 => EnvironmentCheatsFh5.TimeDetourAddress,
            _ => throw new IndexOutOfRangeException()
        };
        
        if (detourAddress == 0) return;
        GetInstance().WriteMemory(detourAddress + 0x24, e.NewValue.GetValueOrDefault());
    }

    private async void TimeSwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleSwitch toggleSwitch)
        {
            return;
        }
        
        ViewModel.AreManualTimeUiElementsEnabled = false;
        switch (GameVerPlat.GetInstance().Type)
        {
            case GameVerPlat.GameType.Fh4:
            {
                if (EnvironmentCheatsFh4.TimeDetourAddress == 0)
                {
                    await EnvironmentCheatsFh4.CheatTime();
                }
                break;
            }
            case GameVerPlat.GameType.Fh5:
            {
                
                if (EnvironmentCheatsFh5.TimeDetourAddress == 0)
                {
                    await EnvironmentCheatsFh5.CheatTime();
                }
                break;
            }
            case GameVerPlat.GameType.None:
            default:
                throw new IndexOutOfRangeException();
        }
        ViewModel.AreManualTimeUiElementsEnabled = true;
        
        var detourAddress = GameVerPlat.GetInstance().Type switch
        {
            GameVerPlat.GameType.Fh4 => EnvironmentCheatsFh4.TimeDetourAddress,
            GameVerPlat.GameType.Fh5 => EnvironmentCheatsFh5.TimeDetourAddress,
            _ => throw new IndexOutOfRangeException()
        };
        
        if (detourAddress == 0) return;
        GetInstance().WriteMemory(detourAddress + 0x23, toggleSwitch.IsOn ? (byte)1 : (byte)0);
        GetInstance().WriteMemory(detourAddress + 0x24, TimeBox.Value.GetValueOrDefault());
    }

    private async void FreezeAiSwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleSwitch toggleSwitch)
        {
            return;
        }

        toggleSwitch.IsEnabled = false;
        if (CarCheatsFh5.FreezeAiDetourAddress == 0)
        {
            await CarCheatsFh5.CheatFreezeAi();
        }
        toggleSwitch.IsEnabled = true;

        if (CarCheatsFh5.FreezeAiDetourAddress == 0) return;
        GetInstance().WriteMemory(CarCheatsFh5.FreezeAiDetourAddress + 0x4F, toggleSwitch.IsOn ? (byte)1 : (byte)0);
    }

    private void NumericUpDown_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
    {
        var detourAddress = GameVerPlat.GetInstance().Type switch
        {
            GameVerPlat.GameType.Fh4 => EnvironmentCheatsFh4.SunRgbDetourAddress,
            GameVerPlat.GameType.Fh5 => EnvironmentCheatsFh5.SunRgbDetourAddress,
            _ => throw new IndexOutOfRangeException()
        };
        
        if (detourAddress == 0) return;
        GetInstance().WriteMemory(detourAddress + 0x33, ConvertUiColorToGameValues(Picker.SelectedColor.GetValueOrDefault(), IntensityBox.Value.GetValueOrDefault()));
    }
}