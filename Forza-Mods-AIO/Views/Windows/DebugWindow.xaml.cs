using System.ComponentModel;
using System.Windows.Controls;
using Forza_Mods_AIO.ViewModels.Windows;

namespace Forza_Mods_AIO.Views.Windows;

public partial class DebugWindow
{
    public DebugWindowViewModel ViewModel { get; }

    public DebugWindow(DebugWindowViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
        Closing += OnClosing;
    }

    private void OnClosing(object? sender, CancelEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }

    private void DebugList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ViewModel.CurrentDebugSession = ViewModel.DebugSessions[DebugList.SelectedIndex];
    }
}