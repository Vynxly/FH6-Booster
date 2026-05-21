using System.Windows;
using System.Windows.Controls;
using Forza_Mods_AIO.Cheats.ForzaHorizon5;
using Forza_Mods_AIO.Models;
using Forza_Mods_AIO.ViewModels.SubPages.SelfVehicle;
using MahApps.Metro.Controls;
using static Forza_Mods_AIO.Resources.Cheats;
using static Forza_Mods_AIO.Resources.Memory;

namespace Forza_Mods_AIO.Views.SubPages.SelfVehicle;

public partial class PhotoMode
{
    public PhotoMode()
    {
        ViewModel = new PhotoModeViewModel();
        DataContext = this;
        
        InitializeComponent();
    }

    public PhotoModeViewModel ViewModel { get; }
    private static PhotomodeCheats PhotomodeCheatsFh5 => GetClass<PhotomodeCheats>();
    private static Cheats.ForzaHorizon4.PhotomodeCheats PhotomodeCheatsFh4 =>
        GetClass<Cheats.ForzaHorizon4.PhotomodeCheats>();

    private async void NoClipSwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleSwitch toggleSwitch)
        {
            return;
        }

        switch (GameVerPlat.GetInstance().Type)
        {
            case GameVerPlat.GameType.Fh4:
            {        
                toggleSwitch.IsEnabled = false;
                if (PhotomodeCheatsFh4.NoClipDetourAddress == 0)
                {
                    await PhotomodeCheatsFh4.CheatNoClip();
                }
                toggleSwitch.IsEnabled = true;

                if (PhotomodeCheatsFh4.NoClipDetourAddress == 0) return;
                GetInstance().WriteMemory(PhotomodeCheatsFh4.NoClipDetourAddress + 0x1B, toggleSwitch.IsOn ? (byte)1 : (byte)0);
                break;
            }
            case GameVerPlat.GameType.Fh5:
            {        
                toggleSwitch.IsEnabled = false;
                if (PhotomodeCheatsFh5.NoClipDetourAddress == 0)
                {
                    await PhotomodeCheatsFh5.CheatNoClip();
                }
                toggleSwitch.IsEnabled = true;

                if (PhotomodeCheatsFh5.NoClipDetourAddress == 0) return;
                GetInstance().WriteMemory(PhotomodeCheatsFh5.NoClipDetourAddress + 0x19, toggleSwitch.IsOn ? (byte)1 : (byte)0);
                break;
            }
            case GameVerPlat.GameType.None:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void NoHeightLimitsSwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleSwitch toggleSwitch)
        {
            return;
        }

        switch (GameVerPlat.GetInstance().Type)
        {
            case GameVerPlat.GameType.Fh4:
            {
                toggleSwitch.IsEnabled = false;
                if (PhotomodeCheatsFh4.NoHeightLimitDetourAddress == 0)
                {
                    await PhotomodeCheatsFh4.CheatNoHeightLimits();
                }
                toggleSwitch.IsEnabled = true;
        
                if (PhotomodeCheatsFh4.NoHeightLimitDetourAddress == 0) return;
                GetInstance().WriteMemory(PhotomodeCheatsFh4.NoHeightLimitDetourAddress + 0x24, toggleSwitch.IsOn ? (byte)1 : (byte)0);
                break;
            }
            case GameVerPlat.GameType.Fh5:
            {
                toggleSwitch.IsEnabled = false;
                if (PhotomodeCheatsFh5.NoHeightLimitDetourAddress == 0)
                {
                    await PhotomodeCheatsFh5.CheatNoHeightLimits();
                }
                toggleSwitch.IsEnabled = true;
        
                if (PhotomodeCheatsFh5.NoHeightLimitDetourAddress == 0) return;
                GetInstance().WriteMemory(PhotomodeCheatsFh5.NoHeightLimitDetourAddress + 0x24, toggleSwitch.IsOn ? (byte)1 : (byte)0);
                break;
            }
            case GameVerPlat.GameType.None:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void IncreasedZoomSwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleSwitch toggleSwitch)
        {
            return;
        }
        
        switch (GameVerPlat.GetInstance().Type)
        {
            case GameVerPlat.GameType.Fh4:
            {
                toggleSwitch.IsEnabled = false;
                if (PhotomodeCheatsFh4.IncreasedZoomDetourAddress == 0)
                {
                    await PhotomodeCheatsFh4.CheatIncreasedZoom();
                }
                toggleSwitch.IsEnabled = true;
        
                if (PhotomodeCheatsFh4.IncreasedZoomDetourAddress == 0) return;
                GetInstance().WriteMemory(PhotomodeCheatsFh4.IncreasedZoomDetourAddress + 0x17, toggleSwitch.IsOn ? (byte)1 : (byte)0);
                break;
            }
            case GameVerPlat.GameType.Fh5:
            {
                toggleSwitch.IsEnabled = false;
                if (PhotomodeCheatsFh5.IncreasedZoomDetourAddress == 0)
                {
                    await PhotomodeCheatsFh5.CheatIncreasedZoom();
                }
                toggleSwitch.IsEnabled = true;
        
                if (PhotomodeCheatsFh5.IncreasedZoomDetourAddress == 0) return;
                GetInstance().WriteMemory(PhotomodeCheatsFh5.IncreasedZoomDetourAddress + 0x21, toggleSwitch.IsOn ? (byte)1 : (byte)0);
                break;
            }
            case GameVerPlat.GameType.None:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void ModifiersScanButton_OnClick(object sender, RoutedEventArgs e)
    {
        ViewModel.AreScanPromptLimiterUiElementsVisible = false;
        ViewModel.AreScanningLimiterUiElementsVisible = true;

        bool successful;
        switch (GameVerPlat.GetInstance().Type)
        {
            case GameVerPlat.GameType.Fh4:
            {
                if (!PhotomodeCheatsFh4.WasModifiersScanSuccessful)
                {
                    await PhotomodeCheatsFh4.CheatModifiers();
                }

                successful = PhotomodeCheatsFh4.WasModifiersScanSuccessful;
                break;
            }
            case GameVerPlat.GameType.Fh5:
            {
                if (!PhotomodeCheatsFh5.WasModifiersScanSuccessful)
                {
                    await PhotomodeCheatsFh5.CheatModifiers();
                }

                successful = PhotomodeCheatsFh5.WasModifiersScanSuccessful;
                break;
            }
            case GameVerPlat.GameType.None:
            default:
                throw new IndexOutOfRangeException();
        }

        if (!successful)
        {
            ViewModel.AreScanPromptLimiterUiElementsVisible = true;
            ViewModel.AreScanningLimiterUiElementsVisible = false;
            return;
        }
        
        var address = GameVerPlat.GetInstance().Type switch
        {
            GameVerPlat.GameType.Fh4 => PhotomodeCheatsFh4.MainModifiersAddress,
            GameVerPlat.GameType.Fh5 => PhotomodeCheatsFh5.MainModifiersAddress,
            _ => throw new IndexOutOfRangeException()
        };
        ValueBox.Value = GetInstance().ReadMemory<int>(address);
        
        ViewModel.AreScanningLimiterUiElementsVisible = false;
        ViewModel.AreModifierUiElementsVisible = true;
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var wasSuccessful = PhotomodeCheatsFh5.WasModifiersScanSuccessful ||
                            PhotomodeCheatsFh4.WasModifiersScanSuccessful;
        
        if (sender is not ComboBox comboBox || !wasSuccessful)
        {
            return;
        }

        var address = GameVerPlat.GetInstance().Type switch
        {
            GameVerPlat.GameType.Fh4 => PhotomodeCheatsFh4.MainModifiersAddress,
            GameVerPlat.GameType.Fh5 => PhotomodeCheatsFh5.MainModifiersAddress,
            _ => throw new IndexOutOfRangeException()
        };

        var speedAddress = GameVerPlat.GetInstance().Type switch
        {
            GameVerPlat.GameType.Fh4 => PhotomodeCheatsFh4.SpeedAddress,
            GameVerPlat.GameType.Fh5 => PhotomodeCheatsFh5.SpeedAddress,
            _ => throw new IndexOutOfRangeException()
        };

        ValueBox.Value = comboBox.SelectedIndex switch
        {
            0 => GetInstance().ReadMemory<int>(address),
            1 => GetInstance().ReadMemory<float>(address + 0x20),
            2 => GetInstance().ReadMemory<float>(address + 0x30),
            3 => GetInstance().ReadMemory<float>(address + 0x38),
            4 => GetInstance().ReadMemory<float>(address + 0xC),
            5 => GetInstance().ReadMemory<float>(speedAddress),
            6 => GetInstance().ReadMemory<float>(speedAddress + 0x4),
            _ => ValueBox.Value
        };
    }

    private void ValueBox_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
    {
        var address = GameVerPlat.GetInstance().Type switch
        {
            GameVerPlat.GameType.Fh4 => PhotomodeCheatsFh4.MainModifiersAddress,
            GameVerPlat.GameType.Fh5 => PhotomodeCheatsFh5.MainModifiersAddress,
            _ => throw new IndexOutOfRangeException()
        };

        var speedAddress = GameVerPlat.GetInstance().Type switch
        {
            GameVerPlat.GameType.Fh4 => PhotomodeCheatsFh4.SpeedAddress,
            GameVerPlat.GameType.Fh5 => PhotomodeCheatsFh5.SpeedAddress,
            _ => throw new IndexOutOfRangeException()
        };
        
        switch (MainComboBox.SelectedIndex)
        {
            case 0:
            {
                GetInstance().WriteMemory(address, Convert.ToInt32(e.NewValue.GetValueOrDefault()));
                break;
            }
            case 1:
            {
                GetInstance().WriteMemory(address + 0x20, Convert.ToSingle(e.NewValue.GetValueOrDefault()));
                break;
            }
            case 2:
            {
                GetInstance().WriteMemory(address + 0x30, Convert.ToSingle(e.NewValue.GetValueOrDefault()));
                break;
            }
            case 3:
            {
                GetInstance().WriteMemory(address + 0x38, Convert.ToSingle(e.NewValue.GetValueOrDefault()));
                break;
            }
            case 4:
            {
                GetInstance().WriteMemory(address + 0xC, Convert.ToSingle(e.NewValue.GetValueOrDefault()));
                break;
            }
            case 5:
            {
                GetInstance().WriteMemory(speedAddress, Convert.ToSingle(e.NewValue.GetValueOrDefault()));
                break;
            }
            case 6:
            {
                GetInstance().WriteMemory(speedAddress + 0x4, Convert.ToSingle(e.NewValue.GetValueOrDefault()));
                break;
            }
        }
    }
}