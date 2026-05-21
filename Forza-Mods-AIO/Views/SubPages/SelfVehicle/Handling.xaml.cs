using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Forza_Mods_AIO.Cheats.ForzaHorizon5;
using Forza_Mods_AIO.Resources.Keybinds;
using Forza_Mods_AIO.ViewModels.SubPages.SelfVehicle;
using Forza_Mods_AIO.ViewModels.Windows;
using MahApps.Metro.Controls;
using static Forza_Mods_AIO.Resources.Cheats;
using static Forza_Mods_AIO.Resources.Memory;

namespace Forza_Mods_AIO.Views.SubPages.SelfVehicle;

public partial class Handling
{
    private readonly GlobalHotkey _jumpHackHotkey = new(ModifierKeys.None, Key.None, JumpHackCallback);
    private readonly GlobalHotkey _brakeHackHotkey = new(ModifierKeys.None, Key.None, BrakeHackCallback, 1);
    private readonly GlobalHotkey _stopAllWheelsHotkey = new(ModifierKeys.None, Key.None, StopAllWheelsCallback, 1);
    private readonly GlobalHotkey _velocityHotkey = new(ModifierKeys.None, Key.None, VelocityCallback, 1);
    private readonly GlobalHotkey _wheelspeedHotkey = new(ModifierKeys.None, Key.None, WheelspeedCallback, 1);

    private static void JumpHackCallback()
    {
        if (CarCheatsFh5.LocalPlayerHookDetourAddress <= UIntPtr.Zero) return;
        GetInstance().WriteMemory(CarCheatsFh5.LocalPlayerHookDetourAddress + CarCheatsOffsets.JumpHackEnabled, (byte)1);
    }
    
    private static void BrakeHackCallback()
    {
        if (CarCheatsFh5.LocalPlayerHookDetourAddress <= UIntPtr.Zero) return;
        GetInstance().WriteMemory(CarCheatsFh5.LocalPlayerHookDetourAddress + CarCheatsOffsets.BrakeHackEnabled, (byte)1);
    }
    
    private static void StopAllWheelsCallback()
    {
        if (CarCheatsFh5.LocalPlayerHookDetourAddress <= UIntPtr.Zero) return;
        GetInstance().WriteMemory(CarCheatsFh5.LocalPlayerHookDetourAddress + CarCheatsOffsets.StopAllWheelsEnabled, (byte)1);
    }

    private static void VelocityCallback()
    {
        if (CarCheatsFh5.LocalPlayerHookDetourAddress <= UIntPtr.Zero) return;
        GetInstance().WriteMemory(CarCheatsFh5.LocalPlayerHookDetourAddress + CarCheatsOffsets.VelEnabled, (byte)1);
    }

    private static void WheelspeedCallback()
    {
        if (CarCheatsFh5.LocalPlayerHookDetourAddress <= UIntPtr.Zero) return;
        GetInstance().WriteMemory(CarCheatsFh5.LocalPlayerHookDetourAddress + CarCheatsOffsets.WheelspeedEnabled, (byte)1);
    }

    public Handling()
    {
        ViewModel = new HandlingViewModel();
        DataContext = this;
        
        InitializeComponent();
        HotkeysManager.AddHotkey(_jumpHackHotkey);
        HotkeysManager.AddHotkey(_brakeHackHotkey);
        HotkeysManager.AddHotkey(_stopAllWheelsHotkey);
        HotkeysManager.AddHotkey(_velocityHotkey);
        HotkeysManager.AddHotkey(_wheelspeedHotkey);
    }

    private static CarCheats CarCheatsFh5 => GetClass<CarCheats>();
    public HandlingViewModel ViewModel { get; }

    private async void ModifierToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleSwitch toggleSwitch)
        {
            return;
        }

        ViewModel.AreUiElementsEnabled = false;
        
        if (ModifierModeBox.SelectedIndex == 0)
        {
            await Accel(toggleSwitch.IsOn);
        }
        else
        {
            await Gravity(toggleSwitch.IsOn);
        }

        ViewModel.AreUiElementsEnabled = true;
    }

    private async Task Accel(bool toggled)
    {
        if (CarCheatsFh5.AccelDetourAddress == 0)
        {
            await CarCheatsFh5.CheatAccel();
        }

        if (CarCheatsFh5.AccelDetourAddress <= 0) return;
        GetInstance().WriteMemory(CarCheatsFh5.AccelDetourAddress + 0x58, toggled ? (byte)1 : (byte)0);
        GetInstance().WriteMemory(CarCheatsFh5.AccelDetourAddress + 0x59, Convert.ToSingle(ModifierValueBox.Value));  
        ViewModel.IsAccelEnabled = toggled;
    }
    
    private async Task Gravity(bool toggled)
    {
        if (CarCheatsFh5.GravityDetourAddress == 0)
        {
            await CarCheatsFh5.CheatGravity();
        }
            
        if (CarCheatsFh5.GravityDetourAddress <= 0) return;
        GetInstance().WriteMemory(CarCheatsFh5.GravityDetourAddress + 0x59, toggled ? (byte)1 : (byte)0);
        GetInstance().WriteMemory(CarCheatsFh5.GravityDetourAddress + 0x5A,Convert.ToSingle(ModifierValueBox.Value));   
        ViewModel.IsGravityEnabled = toggled;
    }

    private void ModifierModeBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ModifierToggleSwitch == null)
        {
            return;
        }
        
        ModifierToggleSwitch.Toggled -= ModifierToggleSwitch_OnToggled;
        
        if (ModifierModeBox.SelectedIndex == 0)
        {
            ModifierValueBox.Value = ViewModel.AccelValue;
            ModifierToggleSwitch.IsOn = ViewModel.IsAccelEnabled;
        }
        else
        {
            ModifierValueBox.Value = ViewModel.GravityValue;
            ModifierToggleSwitch.IsOn = ViewModel.IsGravityEnabled;
        }
        
        ModifierToggleSwitch.Toggled += ModifierToggleSwitch_OnToggled;
    }

    private void ModifierValueBox_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
    {
        if (ModifierModeBox.SelectedIndex == 0)
        {
            ViewModel.AccelValue = Convert.ToDouble(e.NewValue);
            if (CarCheatsFh5.AccelDetourAddress <= UIntPtr.Zero) return;
            GetInstance().WriteMemory(CarCheatsFh5.AccelDetourAddress + 0x59, Convert.ToSingle(e.NewValue));  
        }
        else
        {
            ViewModel.GravityValue = Convert.ToDouble(e.NewValue);
            if (CarCheatsFh5.GravityDetourAddress <= UIntPtr.Zero) return;
            GetInstance().WriteMemory(CarCheatsFh5.GravityDetourAddress + 0x5A,Convert.ToSingle(e.NewValue));   
        }
    }

    private async void PullButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (ModifierModeBox.SelectedIndex == 0)
        {
            ViewModel.AreUiElementsEnabled = false;
            if (CarCheatsFh5.AccelDetourAddress == 0)
            {
                await CarCheatsFh5.CheatAccel();
            }
            ViewModel.AreUiElementsEnabled = true;
            
            if (CarCheatsFh5.AccelDetourAddress == 0) return;
            ViewModel.AccelValue = Convert.ToDouble(GetInstance().ReadMemory<float>(CarCheatsFh5.AccelDetourAddress + 0x5D));
            ModifierValueBox.Value = ViewModel.AccelValue;
        }
        else
        {
            ViewModel.AreUiElementsEnabled = false;
            if (CarCheatsFh5.GravityDetourAddress == 0)
            {
                await CarCheatsFh5.CheatGravity();
            }
            ViewModel.AreUiElementsEnabled = true;

            if (CarCheatsFh5.GravityDetourAddress == 0) return;
            ViewModel.GravityValue = Convert.ToDouble(GetInstance().ReadMemory<float>(CarCheatsFh5.GravityDetourAddress + 0x5E));
            ModifierValueBox.Value = ViewModel.GravityValue;
        }
        
    }

    private async void VelocitySwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleSwitch toggleSwitch)
        {
            return;
        }

        ViewModel.AreUiElementsEnabled = false;

        if (CarCheatsFh5.LocalPlayerHookDetourAddress == 0)
        {
            await CarCheatsFh5.CheatLocalPlayer();
        }
        ViewModel.AreUiElementsEnabled = true;
        
        if (CarCheatsFh5.LocalPlayerHookDetourAddress <= UIntPtr.Zero) return;
        GetInstance().WriteMemory(CarCheatsFh5.LocalPlayerHookDetourAddress + CarCheatsOffsets.VelBoost, 1f + Convert.ToSingle(VelocityStrength.Value / 1000));  
        GetInstance().WriteMemory(CarCheatsFh5.LocalPlayerHookDetourAddress + CarCheatsOffsets.VelLimit, Convert.ToSingle(VelocityLimit.Value));
        _velocityHotkey.CanExecute = toggleSwitch.IsOn;
    }

    private void VelocityStrength_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
    {
        if (CarCheatsFh5.LocalPlayerHookDetourAddress <= UIntPtr.Zero) return;
        GetInstance().WriteMemory(CarCheatsFh5.LocalPlayerHookDetourAddress + CarCheatsOffsets.VelBoost, 1f + Convert.ToSingle(VelocityStrength.Value / 1000));  
    }

    private void VelocityLimit_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
    {
        if (CarCheatsFh5.LocalPlayerHookDetourAddress <= UIntPtr.Zero) return;
        GetInstance().WriteMemory(CarCheatsFh5.LocalPlayerHookDetourAddress + CarCheatsOffsets.VelLimit, Convert.ToSingle(VelocityLimit.Value));  
    }

    private async void WheelspeedSwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleSwitch toggleSwitch)
        {
            return;
        }

        ViewModel.AreUiElementsEnabled = false;

        if (CarCheatsFh5.LocalPlayerHookDetourAddress == 0)
        {
            await CarCheatsFh5.CheatLocalPlayer();
        }
        ViewModel.AreUiElementsEnabled = true;
        
        if (CarCheatsFh5.LocalPlayerHookDetourAddress <= UIntPtr.Zero) return;
        GetInstance().WriteMemory(CarCheatsFh5.LocalPlayerHookDetourAddress + CarCheatsOffsets.WheelspeedMode, (byte)WheelspeedModeBox.SelectedIndex);  
        GetInstance().WriteMemory(CarCheatsFh5.LocalPlayerHookDetourAddress + CarCheatsOffsets.WheelspeedBoost, Convert.ToSingle(WheelspeedValueBox.Value));
        GetInstance().WriteMemory(CarCheatsFh5.LocalPlayerHookDetourAddress + CarCheatsOffsets.WheelspeedLimit, Convert.ToSingle(WheelspeedLimit.Value));  
        _wheelspeedHotkey.CanExecute = toggleSwitch.IsOn;
    }

    private void WheelspeedNum_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
    {
        if (CarCheatsFh5.LocalPlayerHookDetourAddress <= UIntPtr.Zero) return;
        GetInstance().WriteMemory(CarCheatsFh5.LocalPlayerHookDetourAddress + CarCheatsOffsets.WheelspeedBoost, Convert.ToSingle(WheelspeedValueBox.Value));  
    }

    private void WheelspeedModeBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CarCheatsFh5.LocalPlayerHookDetourAddress <= UIntPtr.Zero) return;
        GetInstance().WriteMemory(CarCheatsFh5.LocalPlayerHookDetourAddress + CarCheatsOffsets.WheelspeedMode, (byte)WheelspeedModeBox.SelectedIndex);   
    }

    private void JumpSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (CarCheatsFh5.LocalPlayerHookDetourAddress <= UIntPtr.Zero) return;
        GetInstance().WriteMemory(CarCheatsFh5.LocalPlayerHookDetourAddress + CarCheatsOffsets.JumpHackBoost, Convert.ToSingle(e.NewValue));   
    }

    private async void JumpSwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleSwitch toggleSwitch)
        {
            return;
        }
        
        ViewModel.AreUiElementsEnabled = false;

        if (CarCheatsFh5.LocalPlayerHookDetourAddress == 0)
        {
            await CarCheatsFh5.CheatLocalPlayer();
        }
        ViewModel.AreUiElementsEnabled = true;
        
        if (CarCheatsFh5.LocalPlayerHookDetourAddress <= UIntPtr.Zero) return; 
        GetInstance().WriteMemory(CarCheatsFh5.LocalPlayerHookDetourAddress + CarCheatsOffsets.JumpHackBoost, Convert.ToSingle(JumpSlider.Value));
        _jumpHackHotkey.CanExecute = toggleSwitch.IsOn;
    }

    private void StopSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (CarCheatsFh5.LocalPlayerHookDetourAddress <= UIntPtr.Zero) return;
        GetInstance().WriteMemory(CarCheatsFh5.LocalPlayerHookDetourAddress + CarCheatsOffsets.BrakeHackBoost, Convert.ToSingle(e.NewValue));   
    }

    private async void StopSwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleSwitch toggleSwitch)
        {
            return;
        }
        
        ViewModel.AreUiElementsEnabled = false;
        if (CarCheatsFh5.LocalPlayerHookDetourAddress == 0)
        {
            await CarCheatsFh5.CheatLocalPlayer();
        }
        ViewModel.AreUiElementsEnabled = true;
        
        if (CarCheatsFh5.LocalPlayerHookDetourAddress <= UIntPtr.Zero) return;
        GetInstance().WriteMemory(CarCheatsFh5.LocalPlayerHookDetourAddress + CarCheatsOffsets.BrakeHackBoost, Convert.ToSingle(StopSlider.Value));
        _brakeHackHotkey.CanExecute = toggleSwitch.IsOn;
    }

    private async void StopAllSwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleSwitch toggleSwitch)
        {
            return;
        }

        ViewModel.AreUiElementsEnabled = false;

        if (CarCheatsFh5.LocalPlayerHookDetourAddress == 0)
        {
            await CarCheatsFh5.CheatLocalPlayer();
        }
        
        ViewModel.AreUiElementsEnabled = true;
        _stopAllWheelsHotkey.CanExecute = toggleSwitch.IsOn;
    }

    private async void JumpHackMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        var mainWindowViewModel = App.GetRequiredService<MainWindowViewModel>();
        await mainWindowViewModel.GetHotkey(_jumpHackHotkey);
    }

    private async void NoWaterDragSwitch_OnToggled(object sender, RoutedEventArgs e)
    {        
        if (sender is not ToggleSwitch toggleSwitch)
        {
            return;
        }

        toggleSwitch.IsEnabled = false;
        if (CarCheatsFh5.NoWaterDragDetourAddress == 0)
        {
            await CarCheatsFh5.CheatNoWaterDrag();
        }
        toggleSwitch.IsEnabled = true;
        
        if (CarCheatsFh5.NoWaterDragDetourAddress == 0) return;
        GetInstance().WriteMemory(CarCheatsFh5.NoWaterDragDetourAddress + 0x17, toggleSwitch.IsOn ? (byte)1 : (byte)0);
    }

    private async void NoClipSwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleSwitch toggleSwitch)
        {
            return;
        }

        ViewModel.AreUiElementsEnabled = false;
        if (CarCheatsFh5.NoClipDetourAddress == 0)
        {
            await CarCheatsFh5.CheatNoClip();
        }
        ViewModel.AreUiElementsEnabled = true;
        
        if (CarCheatsFh5.NoClipDetourAddress == 0) return;
        GetInstance().WriteMemory(CarCheatsFh5.NoClipDetourAddress + 0x31, toggleSwitch.IsOn ? (byte)1 : (byte)0);
    }

    private async void VelocityMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        var mainWindowViewModel = App.GetRequiredService<MainWindowViewModel>();
        await mainWindowViewModel.GetHotkey(_velocityHotkey);
    }

    private async void WheelspeedMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        var mainWindowViewModel = App.GetRequiredService<MainWindowViewModel>();
        await mainWindowViewModel.GetHotkey(_wheelspeedHotkey);
    }

    private async void BrakeHackMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        var mainWindowViewModel = App.GetRequiredService<MainWindowViewModel>();
        await mainWindowViewModel.GetHotkey(_brakeHackHotkey);
    }

    private async void StopAllWheelsMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        var mainWindowViewModel = App.GetRequiredService<MainWindowViewModel>();
        await mainWindowViewModel.GetHotkey(_stopAllWheelsHotkey);
    }

    private void WheelspeedLimit_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
    {
        if (CarCheatsFh5.LocalPlayerHookDetourAddress <= UIntPtr.Zero) return;
        GetInstance().WriteMemory(CarCheatsFh5.LocalPlayerHookDetourAddress + CarCheatsOffsets.WheelspeedLimit, Convert.ToSingle(e.NewValue));  
    }
}