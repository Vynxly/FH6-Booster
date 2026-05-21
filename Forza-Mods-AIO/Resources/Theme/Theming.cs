using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using ControlzEx.Theming;
using static System.Windows.Media.ColorConverter;
using Forza_Mods_AIO.Helpers;

namespace Forza_Mods_AIO.Resources.Theme;

public sealed class Theming : INotifyPropertyChanged
{
    private static readonly Color DefaultThemeColor = (Color)ConvertFromString("#4C566A");
    
    private static Theming _instance = null!;
    public static Theming GetInstance()
    {
        if (_instance != null!) return _instance;
        _instance = new Theming();
        return _instance;
    }
    
    private Brush _mainColour = new SolidColorBrush((Color)ConvertFromString("#4C566A"));
    public Brush MainColour
    {
        get => _mainColour;
        private set => SetField(ref _mainColour, value);
    }
    
    private Brush _darkishColour = new SolidColorBrush((Color)ConvertFromString("#434C5E"));
    public Brush DarkishColour
    {
        get => _darkishColour;
        private set => SetField(ref _darkishColour, value);
    }
    
    private Brush _darkColour = new SolidColorBrush((Color)ConvertFromString("#3B4252"));
    public Brush DarkColour
    {
        get => _darkColour;
        private set => SetField(ref _darkColour, value);
    }
    
    private Brush _darkerColour = new SolidColorBrush((Color)ConvertFromString("#2E3440"));
    public Brush DarkerColour
    {
        get => _darkerColour;
        private set => SetField(ref _darkerColour, value);
    }

    public Color MainColourAsColour { get; private set; } = (Color)ConvertFromString("#2E3440");

    public void ChangeColor(Color color = default)
    {
        if (color == default)
            return;
        
        // Store the main color
        MainColourAsColour = color;
        
        // Save the color to settings
        Settings.SaveThemeColor(color);
        
        // Create a darker shade for each level
        var darkish = Color.FromRgb(
            (byte)(color.R * 0.9),
            (byte)(color.G * 0.9),
            (byte)(color.B * 0.9));
        
        var dark = Color.FromRgb(
            (byte)(color.R * 0.8),
            (byte)(color.G * 0.8),
            (byte)(color.B * 0.8));
        
        var darker = Color.FromRgb(
            (byte)(color.R * 0.7),
            (byte)(color.G * 0.7),
            (byte)(color.B * 0.7));

        // Update the brushes
        MainColour = new SolidColorBrush(color);
        DarkishColour = new SolidColorBrush(darkish);
        DarkColour = new SolidColorBrush(dark);
        DarkerColour = new SolidColorBrush(darker);

        // Create and apply the theme
        var themeName = $"CustomTheme_{color.ToString()}";
        ThemeManager.Current.AddTheme(new ControlzEx.Theming.Theme(themeName,
            themeName,
            "Dark",
            "Red",
            color,
            new SolidColorBrush(color),
            false,
            false));
        ThemeManager.Current.ChangeTheme(Application.Current, themeName);
    }

    // Add a method to initialize the theme
    public void InitializeTheme()
    {
        var savedColor = Settings.LoadThemeColor();
        if (savedColor.HasValue)
        {
            ChangeColor(savedColor.Value);
        }
    }

    public void ResetTheme()
    {
        ChangeColor(DefaultThemeColor);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        OnPropertyChanged(propertyName);
    }
}