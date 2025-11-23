using TasoDoro.ViewModels;

namespace TasoDoro.Views;
public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}