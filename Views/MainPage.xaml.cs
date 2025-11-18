using TasoDoro.ViewModels;

namespace TasoDoro.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage(TimerViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}