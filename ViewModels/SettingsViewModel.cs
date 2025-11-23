using CommunityToolkit.Mvvm.ComponentModel;

namespace TasoDoro.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        // Default Değerler
        private const bool IsClassicModeDefault = true;
        private const int WorkDurationDefault = 25;
        private const int ShortBreakDurationDefault = 5;
        private const int LongBreakDurationDefault = 15;

        [ObservableProperty]
        private bool _isClassicMode;

        [ObservableProperty]
        private int _workDuration;

        [ObservableProperty]
        private int _shortBreakDuration;

        [ObservableProperty]
        private int _longBreakDuration;

        public SettingsViewModel()
        {
            // Kayıtlı ayarları yükle veya varsayılanları kullan
            _isClassicMode = Preferences.Get(nameof(IsClassicMode), IsClassicModeDefault);
            _workDuration = Preferences.Get(nameof(WorkDuration), WorkDurationDefault);
            _shortBreakDuration = Preferences.Get(nameof(ShortBreakDuration), ShortBreakDurationDefault);
            _longBreakDuration = Preferences.Get(nameof(LongBreakDuration), LongBreakDurationDefault);
        }

        // IsClassicMode değiştiğinde bu metot otomatik olarak çağrılır.
        partial void OnIsClassicModeChanged(bool value)
        {
            Preferences.Set(nameof(IsClassicMode), value);
            // IsCustomMode'un da değiştiğini UI'a bildirir.
            OnPropertyChanged(nameof(IsCustomMode));
        }

        // Diğer property'ler değiştiğinde çağrılacak metotlar
        partial void OnWorkDurationChanged(int value) => Preferences.Set(nameof(WorkDuration), value);
        partial void OnShortBreakDurationChanged(int value) => Preferences.Set(nameof(ShortBreakDuration), value);
        partial void OnLongBreakDurationChanged(int value) => Preferences.Set(nameof(LongBreakDuration), value);

        // Bu property, IsClassicMode'a bağlı olarak dinamik bir şekilde hesaplanır.
        public bool IsCustomMode => !IsClassicMode;
    }
}
