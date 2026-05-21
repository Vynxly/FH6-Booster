using System.Configuration;
using System.Windows.Media;

namespace Forza_Mods_AIO.Helpers;

public static class Settings
{
    public static void SaveThemeColor(Color color)
    {
        var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        
        // Remove existing setting if it exists
        if (config.AppSettings.Settings["ThemeColor"] != null)
        {
            config.AppSettings.Settings.Remove("ThemeColor");
        }
        
        // Add the new setting
        config.AppSettings.Settings.Add("ThemeColor", color.ToString());
        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("appSettings");
    }

    public static Color? LoadThemeColor()
    {
        var colorString = ConfigurationManager.AppSettings["ThemeColor"];
        if (string.IsNullOrEmpty(colorString))
            return null;
            
        try
        {
            return (Color)ColorConverter.ConvertFromString(colorString);
        }
        catch
        {
            return null;
        }
    }

    public static void SaveLanguage(string language)
    {
        var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        
        // Remove existing setting if it exists
        if (config.AppSettings.Settings["Language"] != null)
        {
            config.AppSettings.Settings.Remove("Language");
        }
        
        // Add the new setting
        config.AppSettings.Settings.Add("Language", language);
        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("appSettings");
    }

    public static string LoadLanguage()
    {
        return ConfigurationManager.AppSettings["Language"] ?? "English";
    }
} 