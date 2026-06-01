using System.Windows;
using System.Windows.Controls;
using Forza_Mods_AIO.Cheats.ForzaHorizon5;
using Forza_Mods_AIO.Models;
using Forza_Mods_AIO.ViewModels.SubPages.SelfVehicle;
using MahApps.Metro.Controls;
using static Forza_Mods_AIO.Resources.Cheats;
using static Forza_Mods_AIO.Resources.Memory;

namespace Forza_Mods_AIO.Views.SubPages.SelfVehicle;

public partial class Unlocks
{
    public Unlocks()
    {
        ViewModel = new UnlocksViewModel();
        DataContext = this;

        InitializeComponent();
        ValueBox.Value = ViewModel.CreditsValue;
    }

    public UnlocksViewModel ViewModel { get; }

    private static UnlocksCheats UnlocksCheatsFh5 => GetClass<UnlocksCheats>();

    private static Cheats.ForzaHorizon4.UnlocksCheats UnlocksCheatsFh4 =>
        GetClass<Cheats.ForzaHorizon4.UnlocksCheats>();

    private bool IsFh4 => GameVerPlat.GetInstance().Type == GameVerPlat.GameType.Fh4;

    private bool HasSupportedGame()
    {
        if (GameVerPlat.GetInstance().Type != GameVerPlat.GameType.None)
        {
            return true;
        }

        MessageBox.Show(
            "Launch FH4, FH5, or FH6 first.",
            "No game attached",
            MessageBoxButton.OK,
            MessageBoxImage.Information);

        return false;
    }

    private bool IsFh5OnlySelection()
    {
        return UnlockBox.SelectedIndex == 3;
    }

    private bool CanUseCurrentSelection()
    {
        if (!HasSupportedGame())
        {
            return false;
        }

        if (IsFh4 && IsFh5OnlySelection())
        {
            MessageBox.Show(
                "This module is only available for FH5/FH6.",
                "Unsupported module",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return false;
        }

        return true;
    }

    private static void WriteIntOverride(nuint enableAddress, nuint valueAddress, int value, bool enabled)
    {
        if (enabled)
        {
            GetInstance().WriteMemory(valueAddress, value);
            GetInstance().WriteMemory(enableAddress, (byte)1);
        }
        else
        {
            GetInstance().WriteMemory(enableAddress, (byte)0);
        }
    }

    private async void UnlockSwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleSwitch toggleSwitch)
        {
            return;
        }

        if (!CanUseCurrentSelection())
        {
            SetToggleWithoutRunning(false);
            return;
        }

        StoreCurrentInputValue();

        ViewModel.AreUiElementsEnabled = false;

        try
        {
            switch (UnlockBox.SelectedIndex)
            {
                case 0:
                {
                    if (IsFh4)
                    {
                        await CreditsFh4(toggleSwitch.IsOn);
                    }
                    else
                    {
                        await Credits(toggleSwitch.IsOn);
                    }

                    break;
                }
                case 1:
                {
                    if (IsFh4)
                    {
                        await WheelspinsFh4(toggleSwitch.IsOn);
                    }
                    else
                    {
                        await Wheelspins(toggleSwitch.IsOn);
                    }

                    break;
                }
                case 2:
                {
                    if (IsFh4)
                    {
                        await SkillPointsFh4(toggleSwitch.IsOn);
                    }
                    else
                    {
                        await SkillPoints(toggleSwitch.IsOn);
                    }

                    break;
                }
                case 3:
                {
                    await Series(toggleSwitch.IsOn);
                    break;
                }
            }
        }
        finally
        {
            ViewModel.AreUiElementsEnabled = true;
        }
    }


    private async void OneClickBooster_OnClick(object sender, RoutedEventArgs e)
    {
        if (!HasSupportedGame())
        {
            return;
        }

        ViewModel.CreditsValue = 20_000_000;
        UnlockBox.SelectedIndex = 0;
        ValueBox.Value = ViewModel.CreditsValue;
        ViewModel.AreUiElementsEnabled = false;

        try
        {
            if (IsFh4)
            {
                await CreditsFh4(true);
            }
            else
            {
                await Credits(true);
            }

            SetToggleWithoutRunning(ViewModel.IsCreditsEnabled);
        }
        finally
        {
            ViewModel.AreUiElementsEnabled = true;
        }
    }

    private async void MaxAll_OnClick(object sender, RoutedEventArgs e)
    {
        if (!HasSupportedGame())
        {
            return;
        }

        ViewModel.CreditsValue = 20_000_000;
        ViewModel.WheelspinsValue = 9_999;
        ViewModel.SkillPointsValue = 9_999;
        ViewModel.SeriesValue = 9_999;
        UnlockBox.SelectedIndex = 0;
        ValueBox.Value = ViewModel.CreditsValue;
        ViewModel.AreUiElementsEnabled = false;

        try
        {
            if (IsFh4)
            {
                await CreditsFh4(true);
                await WheelspinsFh4(true);
                await SkillPointsFh4(true);
            }
            else
            {
                await Credits(true);
                await Wheelspins(true);
                await SkillPoints(true);
                await Series(true);
            }

            SetToggleWithoutRunning(ViewModel.IsCreditsEnabled);
        }
        finally
        {
            ViewModel.AreUiElementsEnabled = true;
        }
    }

    private async Task Credits(bool enabled)
    {
        if (enabled && UnlocksCheatsFh5.CreditsDetourAddress == 0)
        {
            await UnlocksCheatsFh5.CheatCredits();
        }

        if (UnlocksCheatsFh5.CreditsDetourAddress <= 0)
        {
            ViewModel.IsCreditsEnabled = false;
            SetToggleWithoutRunning(false);
            return;
        }

        WriteIntOverride(
            UnlocksCheatsFh5.CreditsDetourAddress + 0x31,
            UnlocksCheatsFh5.CreditsDetourAddress + 0x32,
            ViewModel.CreditsValue,
            enabled);

        ViewModel.IsCreditsEnabled = enabled;
    }

    private async Task CreditsFh4(bool enabled)
    {
        if (enabled && UnlocksCheatsFh4.CreditsDetourAddress == 0)
        {
            await UnlocksCheatsFh4.CheatCredits();
        }

        if (UnlocksCheatsFh4.CreditsDetourAddress <= 0)
        {
            ViewModel.IsCreditsEnabled = false;
            SetToggleWithoutRunning(false);
            return;
        }

        WriteIntOverride(
            UnlocksCheatsFh4.CreditsDetourAddress + 0x24,
            UnlocksCheatsFh4.CreditsDetourAddress + 0x25,
            ViewModel.CreditsValue,
            enabled);

        ViewModel.IsCreditsEnabled = enabled;
    }

    private async Task Xp(bool enabled)
    {
        if (enabled && UnlocksCheatsFh5.XpDetourAddress == 0)
        {
            await UnlocksCheatsFh5.CheatXp();
        }

        if (UnlocksCheatsFh5.XpDetourAddress <= 0)
        {
            ViewModel.IsXpEnabled = false;
            SetToggleWithoutRunning(false);
            return;
        }

        if (enabled)
        {
            GetInstance().WriteMemory(UnlocksCheatsFh5.XpDetourAddress + 0x1C, ViewModel.XpValue);
            GetInstance().WriteMemory(UnlocksCheatsFh5.XpPointsDetourAddress + 0x1B, (byte)1);
            GetInstance().WriteMemory(UnlocksCheatsFh5.XpDetourAddress + 0x1B, (byte)1);
        }
        else
        {
            GetInstance().WriteMemory(UnlocksCheatsFh5.XpPointsDetourAddress + 0x1B, (byte)0);
            GetInstance().WriteMemory(UnlocksCheatsFh5.XpDetourAddress + 0x1B, (byte)0);
        }

        ViewModel.IsXpEnabled = enabled;
    }

    private async Task XpFh4(bool enabled)
    {
        if (enabled && UnlocksCheatsFh4.XpDetourAddress == 0)
        {
            await UnlocksCheatsFh4.CheatXp();
        }

        if (UnlocksCheatsFh4.XpDetourAddress <= 0)
        {
            ViewModel.IsXpEnabled = false;
            SetToggleWithoutRunning(false);
            return;
        }

        if (enabled)
        {
            GetInstance().WriteMemory(UnlocksCheatsFh4.XpDetourAddress + 0x1C, ViewModel.XpValue);
            GetInstance().WriteMemory(UnlocksCheatsFh4.XpPointsDetourAddress + 0x1B, (byte)1);
            GetInstance().WriteMemory(UnlocksCheatsFh4.XpDetourAddress + 0x1B, (byte)1);
        }
        else
        {
            GetInstance().WriteMemory(UnlocksCheatsFh4.XpPointsDetourAddress + 0x1B, (byte)0);
            GetInstance().WriteMemory(UnlocksCheatsFh4.XpDetourAddress + 0x1B, (byte)0);
        }

        ViewModel.IsXpEnabled = enabled;
    }

    private async Task Wheelspins(bool enabled)
    {
        if (enabled && UnlocksCheatsFh5.SpinsDetourAddress == 0)
        {
            await UnlocksCheatsFh5.CheatSpins();
        }

        if (UnlocksCheatsFh5.SpinsDetourAddress <= 0)
        {
            ViewModel.IsWheelspinsEnabled = false;
            SetToggleWithoutRunning(false);
            return;
        }

        WriteIntOverride(
            UnlocksCheatsFh5.SpinsDetourAddress + 0x1C,
            UnlocksCheatsFh5.SpinsDetourAddress + 0x1D,
            ViewModel.WheelspinsValue,
            enabled);

        ViewModel.IsWheelspinsEnabled = enabled;
    }

    private async Task WheelspinsFh4(bool enabled)
    {
        if (enabled && UnlocksCheatsFh4.SpinsDetourAddress == 0)
        {
            await UnlocksCheatsFh4.CheatSpins();
        }

        if (UnlocksCheatsFh4.SpinsDetourAddress <= 0)
        {
            ViewModel.IsWheelspinsEnabled = false;
            SetToggleWithoutRunning(false);
            return;
        }

        WriteIntOverride(
            UnlocksCheatsFh4.SpinsDetourAddress + 0x1A,
            UnlocksCheatsFh4.SpinsDetourAddress + 0x1B,
            ViewModel.WheelspinsValue,
            enabled);

        ViewModel.IsWheelspinsEnabled = enabled;
    }

    private async Task SkillPoints(bool enabled)
    {
        if (enabled && UnlocksCheatsFh5.SkillPointsDetourAddress == 0)
        {
            await UnlocksCheatsFh5.CheatSkillPoints();
        }

        if (UnlocksCheatsFh5.SkillPointsDetourAddress <= 0)
        {
            ViewModel.IsSkillPointsEnabled = false;
            SetToggleWithoutRunning(false);
            return;
        }

        WriteIntOverride(
            UnlocksCheatsFh5.SkillPointsDetourAddress + 0x19,
            UnlocksCheatsFh5.SkillPointsDetourAddress + 0x1A,
            ViewModel.SkillPointsValue,
            enabled);

        ViewModel.IsSkillPointsEnabled = enabled;
    }

    private async Task SkillPointsFh4(bool enabled)
    {
        if (enabled && UnlocksCheatsFh4.SkillPointsDetourAddress == 0)
        {
            await UnlocksCheatsFh4.CheatSkillPoints();
        }

        if (UnlocksCheatsFh4.SkillPointsDetourAddress <= 0)
        {
            ViewModel.IsSkillPointsEnabled = false;
            SetToggleWithoutRunning(false);
            return;
        }

        WriteIntOverride(
            UnlocksCheatsFh4.SkillPointsDetourAddress + 0x1C,
            UnlocksCheatsFh4.SkillPointsDetourAddress + 0x1D,
            ViewModel.SkillPointsValue,
            enabled);

        ViewModel.IsSkillPointsEnabled = enabled;
    }

    private async Task Kudos(bool enabled)
    {
        if (enabled && UnlocksCheatsFh5.BxmlEncryptionDetourAddress == 0)
        {
            await UnlocksCheatsFh5.CheatBxmlEncryption();
        }

        if (UnlocksCheatsFh5.BxmlEncryptionDetourAddress <= 0)
        {
            ViewModel.IsKudosEnabled = false;
            SetToggleWithoutRunning(false);
            return;
        }

        WriteIntOverride(
            UnlocksCheatsFh5.BxmlEncryptionDetourAddress + 0x54,
            UnlocksCheatsFh5.BxmlEncryptionDetourAddress + 0x55,
            ViewModel.KudosValue,
            enabled);

        ViewModel.IsKudosEnabled = enabled;
    }

    private async Task Accolades(bool enabled)
    {
        if (enabled && UnlocksCheatsFh5.BxmlEncryptionDetourAddress == 0)
        {
            await UnlocksCheatsFh5.CheatBxmlEncryption();
        }

        if (UnlocksCheatsFh5.BxmlEncryptionDetourAddress <= 0)
        {
            ViewModel.IsAccoladesEnabled = false;
            SetToggleWithoutRunning(false);
            return;
        }

        WriteIntOverride(
            UnlocksCheatsFh5.BxmlEncryptionDetourAddress + 0x59,
            UnlocksCheatsFh5.BxmlEncryptionDetourAddress + 0x5A,
            ViewModel.AccoladesValue,
            enabled);

        ViewModel.IsAccoladesEnabled = enabled;
    }

    private async Task Forzathon(bool enabled)
    {
        if (enabled && UnlocksCheatsFh5.BxmlEncryptionDetourAddress == 0)
        {
            await UnlocksCheatsFh5.CheatBxmlEncryption();
        }

        if (UnlocksCheatsFh5.BxmlEncryptionDetourAddress <= 0)
        {
            ViewModel.IsForzathonEnabled = false;
            SetToggleWithoutRunning(false);
            return;
        }

        WriteIntOverride(
            UnlocksCheatsFh5.BxmlEncryptionDetourAddress + 0x5E,
            UnlocksCheatsFh5.BxmlEncryptionDetourAddress + 0x5F,
            ViewModel.ForzathonValue,
            enabled);

        ViewModel.IsForzathonEnabled = enabled;
    }

    private async Task Seasonal(bool enabled)
    {
        if (enabled && UnlocksCheatsFh5.SeasonalDetourAddress == 0)
        {
            await UnlocksCheatsFh5.CheatSeasonal();
        }

        if (UnlocksCheatsFh5.SeasonalDetourAddress <= 0)
        {
            ViewModel.IsSeasonalEnabled = false;
            SetToggleWithoutRunning(false);
            return;
        }

        WriteIntOverride(
            UnlocksCheatsFh5.SeasonalDetourAddress + 0x23,
            UnlocksCheatsFh5.SeasonalDetourAddress + 0x24,
            ViewModel.SeasonalValue,
            enabled);

        ViewModel.IsSeasonalEnabled = enabled;
    }

    private async Task Series(bool enabled)
    {
        if (enabled && UnlocksCheatsFh5.SeriesDetourAddress == 0)
        {
            await UnlocksCheatsFh5.CheatSeries();
        }

        if (UnlocksCheatsFh5.SeriesDetourAddress <= 0)
        {
            ViewModel.IsSeriesEnabled = false;
            SetToggleWithoutRunning(false);
            return;
        }

        WriteIntOverride(
            UnlocksCheatsFh5.SeriesDetourAddress + 0x1B,
            UnlocksCheatsFh5.SeriesDetourAddress + 0x1C,
            ViewModel.SeriesValue,
            enabled);

        ViewModel.IsSeriesEnabled = enabled;
    }

    private void UnlockBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ValueBox == null || UnlockBox == null || UnlockSwitch == null)
        {
            return;
        }

        ValueBox.ValueChanged -= ValueBox_OnValueChanged;

        ValueBox.Value = UnlockBox.SelectedIndex switch
        {
            0 => ViewModel.CreditsValue,
            1 => ViewModel.WheelspinsValue,
            2 => ViewModel.SkillPointsValue,
            3 => ViewModel.SeriesValue,
            _ => 0
        };

        ValueBox.ValueChanged += ValueBox_OnValueChanged;
        SetToggleWithoutRunning(GetSelectedToggleState());
    }

    private bool GetSelectedToggleState()
    {
        return UnlockBox.SelectedIndex switch
        {
            0 => ViewModel.IsCreditsEnabled,
            1 => ViewModel.IsWheelspinsEnabled,
            2 => ViewModel.IsSkillPointsEnabled,
            3 => ViewModel.IsSeriesEnabled,
            _ => false
        };
    }

    private void SetToggleWithoutRunning(bool isOn)
    {
        if (UnlockSwitch == null)
        {
            return;
        }

        UnlockSwitch.Toggled -= UnlockSwitch_OnToggled;
        UnlockSwitch.IsOn = isOn;
        UnlockSwitch.Toggled += UnlockSwitch_OnToggled;
    }

    private void ValueBox_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
    {
        StoreCurrentInputValue();
    }

    private void StoreCurrentInputValue()
    {
        var value = Convert.ToInt32(ValueBox.Value ?? 0);

        switch (UnlockBox.SelectedIndex)
        {
            case 0:
            {
                ViewModel.CreditsValue = value;
                break;
            }
            case 1:
            {
                ViewModel.WheelspinsValue = value;
                break;
            }
            case 2:
            {
                ViewModel.SkillPointsValue = value;
                break;
            }
            case 3:
            {
                ViewModel.SeriesValue = value;
                break;
            }
        }
    }
}
