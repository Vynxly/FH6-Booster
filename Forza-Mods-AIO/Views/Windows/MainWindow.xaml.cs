using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Forza_Mods_AIO.Resources.Theme;
using Forza_Mods_AIO.ViewModels.Windows;

namespace Forza_Mods_AIO.Views.Windows;

public partial class MainWindow
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    public MainWindowViewModel ViewModel { get; }
    public Theming Theming => Theming.GetInstance();

    private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (WindowState != WindowState.Normal) return;

        var isLeftButton = e.ChangedButton == MouseButton.Left;
        if (!isLeftButton) return;

        var position = e.GetPosition(this);
        var isWithinTopArea = position.Y < 58;
        if (!isWithinTopArea) return;

        DragMove();
    }

    private void WindowStateAction_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;

        switch (button.Tag)
        {
            case "1":
            {
                SystemCommands.MinimizeWindow(this);
                break;
            }
            case "2":
            {
                if (WindowState == WindowState.Maximized)
                {
                    SystemCommands.RestoreWindow(this);
                }
                else
                {
                    SystemCommands.MaximizeWindow(this);
                }

                ViewModel.HandleMaximizeMinimize(this);
                break;
            }
            case "3":
            {
                SystemCommands.CloseWindow(this);
                break;
            }
        }
    }

    private void VynxFooter_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://vynx.dev",
            UseShellExecute = true
        });
    }
}
