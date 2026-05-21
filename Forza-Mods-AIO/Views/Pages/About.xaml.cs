using Forza_Mods_AIO.ViewModels.Pages;

namespace Forza_Mods_AIO.Views.Pages;

public partial class About
{
    public About()
    {
        ViewModel = new AboutViewModel();
        DataContext = this;
        
        InitializeComponent();
    }
    
    public AboutViewModel ViewModel { get; }
}