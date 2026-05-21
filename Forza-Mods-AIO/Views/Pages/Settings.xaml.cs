using System.Windows;
using System.Windows.Controls;
using Forza_Mods_AIO.Resources.Theme;
using Forza_Mods_AIO.ViewModels.Pages;
using Forza_Mods_AIO.Controls.TranslationComboboxItem;
using Forza_Mods_AIO.Helpers;


namespace Forza_Mods_AIO.Views.Pages;

public partial class Settings
{
    private bool _isInitializing = true;
    
    public Settings()
    {
        ViewModel = new SettingsViewModel();
        DataContext = this;
        
        InitializeComponent();
        
        // Debug: Print out all items and their enabled states
        foreach (TranslationComboboxItem item in LanguageBox.Items)
        {
            System.Diagnostics.Debug.WriteLine($"Language: {item.Content}, IsEnabled: {item.IsEnabled}, LanguageCode: {item.LanguageCode}");
        }
        
        // Filter out disabled items
        var enabledItems = LanguageBox.Items.Cast<TranslationComboboxItem>()
            .Where(x => x.IsEnabled)
            .ToList();
            
        LanguageBox.Items.Clear();
        foreach (var item in enabledItems)
        {
            LanguageBox.Items.Add(item);
        }
        
        LoadSavedLanguage();
    }
    
    public SettingsViewModel ViewModel { get; }
    public Theming Theming => Theming.GetInstance();

    private void LoadSavedLanguage()
    {
        string savedLanguage = Helpers.Settings.LoadLanguage();
        
        // Find and select the saved language in the ComboBox after initialization
        Dispatcher.BeginInvoke(new Action(() =>
        {
            var comboBox = this.FindName("LanguageBox") as ComboBox;
            if (comboBox != null)
            {
                var item = comboBox.Items.Cast<TranslationComboboxItem>()
                    .FirstOrDefault(x => x.LanguageCode == savedLanguage && x.IsEnabled);
                
                if (item != null)
                {
                    comboBox.SelectedItem = item;
                    ApplyLanguage(savedLanguage);
                }
            }
            _isInitializing = false;
        }));
    }

    private void ApplyLanguage(string languageCode)
    {
        try
        {
            // Create the resource dictionary URI for the selected language
            var uri = new Uri($"/Resources/Translations/{languageCode}.xaml", UriKind.Relative);
            
            // Create the new ResourceDictionary
            ResourceDictionary langDict = new ResourceDictionary();
            
            try
            {
                langDict.Source = uri;
            }
            catch (Exception)
            {
                MessageBox.Show($"Failed to load language file for {languageCode}.", "Language Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Get the app's current resource dictionaries
            var resources = Application.Current.Resources.MergedDictionaries;

            // Find and remove the current language dictionary if it exists
            var langDictToRemove = resources.FirstOrDefault(dict => 
                dict.Source?.OriginalString.Contains("/Resources/Translations/") == true);
                
            if (langDictToRemove != null)
            {
                resources.Remove(langDictToRemove);
            }

            // Add the new language dictionary
            resources.Add(langDict);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error switching language: {ex.Message}", "Language Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private void LanguageBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox comboBox || comboBox.SelectedItem is not TranslationComboboxItem selectedItem)
        {
            return;
        }

        string? languageCode = selectedItem.LanguageCode;
        if (string.IsNullOrEmpty(languageCode))
        {
            return;
        }

        // Don't save during initial loading
        if (!_isInitializing)
        {
            Helpers.Settings.SaveLanguage(languageCode);
        }

        ApplyLanguage(languageCode);
    }

    private void ResetLanguage_OnClick(object sender, RoutedEventArgs e)
    {
        var englishItem = LanguageBox.Items.Cast<TranslationComboboxItem>()
            .FirstOrDefault(x => x.LanguageCode == "English" && x.IsEnabled);
            
        if (englishItem != null)
        {
            LanguageBox.SelectedItem = englishItem;
        }
    }
}