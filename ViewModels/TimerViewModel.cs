using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Graphics;

namespace TasoDoro.ViewModels
{
    // Pomodoro döngüsünün anlık durumunu belirtir.
    public enum PomodoroState
    {
        Stopped,
        Working,
        ShortBreak,
        LongBreak
    }

    public partial class TimerViewModel : ObservableObject
    {
        private readonly IDispatcherTimer _timer;
        private readonly SettingsViewModel _settings;

        // Ayarlardan gelen süreler (saniye cinsinden)
        private int _workDuration;
        private int _shortBreakDuration;
        private int _longBreakDuration;

        // Mevcut periyodun toplam ve kalan süresi
        private int _totalSecondsInCurrentPeriod;
        private double _remainingSeconds; // Hassas hesaplama için double yapıldı
        private DateTime _endTime; // Bitiş zamanını tutar

        // Kaçıncı çalışma seansında olduğumuzu sayar (uzun mola için)
        private int _workSessionCount = 0;

        // Gözlemlenebilir (Observable) Propertie'ler
        [ObservableProperty]
        private string _timeDisplay;

        [ObservableProperty]
        private double _progressValue;

        [ObservableProperty]
        private bool _isRunning;

        [ObservableProperty]
        private int _pomodoroCount = 0;

        [ObservableProperty]
        private PomodoroState _currentState = PomodoroState.Stopped;

        [ObservableProperty]
        private string _currentStateDisplay = "Başlamak için Oynat'a bas!";

        [ObservableProperty]
        private Color _progressColor = Colors.Red; // Varsayılan renk

        public TimerViewModel(SettingsViewModel settings)
        {
            _settings = settings;

            // Ayarlar değiştiğinde haberdar olmak için event'e abone ol
            _settings.PropertyChanged += (s, e) => LoadSettingsAndReset();

            LoadSettingsAndReset();

            _timer = Application.Current.Dispatcher.CreateTimer();
            // Akıcı animasyon için timer aralığını düşürdük (örneğin 33ms ~ 30 FPS)
            _timer.Interval = TimeSpan.FromMilliseconds(10);
            _timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            var remaining = _endTime - DateTime.Now;
            
            if (remaining.TotalSeconds > 0)
            {
                _remainingSeconds = remaining.TotalSeconds;
                UpdateDisplay();
            }
            else
            {
                _remainingSeconds = 0;
                UpdateDisplay();
                TransitionToNextState();
            }
        }

        private void TransitionToNextState()
        {
            Stop(); // Mevcut durumu bitir

            if (CurrentState == PomodoroState.Working)
            {
                _workSessionCount++;
                PomodoroCount++; // Tamamlanan pomodoro sayısını artır

                // 4 çalışma seansı tamamlandıysa uzun mola, değilse kısa mola
                var nextState = _workSessionCount % 4 == 0 ? PomodoroState.LongBreak : PomodoroState.ShortBreak;
                SetupPeriod(nextState);
            }
            else // Mola bittiyse yeni çalışma seansına geç
            {
                SetupPeriod(PomodoroState.Working);
            }

            Start(); // Yeni durumu başlat
        }

        [RelayCommand]
        private void Start()
        {
            if (IsRunning) return;

            // Eğer durmuş durumdaysa, çalışma periyodu ile başla
            if (CurrentState == PomodoroState.Stopped)
            {
                SetupPeriod(PomodoroState.Working);
            }

            // Bitiş zamanını şimdiki zamana kalan saniyeyi ekleyerek hesapla
            _endTime = DateTime.Now.AddSeconds(_remainingSeconds);

            _timer.Start();
            IsRunning = true;
        }

        [RelayCommand]
        private void Stop()
        {
            if (!IsRunning) return;

            _timer.Stop();
            IsRunning = false;
            
            // Durduğumuz anı kaydetmek için remainingSeconds'ı güncelle
            // (Tekrar başladığında bu değer üzerinden yeni _endTime hesaplanacak)
            var diff = _endTime - DateTime.Now;
            _remainingSeconds = diff.TotalSeconds > 0 ? diff.TotalSeconds : 0;
            UpdateDisplay();
        }

        [RelayCommand]
        private void Reset()
        {
            Stop();
            LoadSettingsAndReset();
            PomodoroCount = 0;
            _workSessionCount = 0;
        }

        // Belirtilen duruma göre zamanlayıcıyı ve arayüzü ayarlar
        private void SetupPeriod(PomodoroState state)
        {
            CurrentState = state;
            switch (state)
            {
                case PomodoroState.Working:
                    _totalSecondsInCurrentPeriod = _workDuration;
                    CurrentStateDisplay = "Çalışma";
                    ProgressColor = Colors.Red;
                    break;
                case PomodoroState.ShortBreak:
                    _totalSecondsInCurrentPeriod = _shortBreakDuration;
                    CurrentStateDisplay = "Kısa Mola";
                    ProgressColor = Colors.Green;
                    break;
                case PomodoroState.LongBreak:
                    _totalSecondsInCurrentPeriod = _longBreakDuration;
                    CurrentStateDisplay = "Uzun Mola";
                    ProgressColor = Colors.Green;
                    break;
                case PomodoroState.Stopped:
                    CurrentStateDisplay = "Başlamak için Oynat'a bas!";
                    ProgressColor = Colors.Red;
                    _totalSecondsInCurrentPeriod = _workDuration; // Reset'te başa dön
                    break;
            }
            _remainingSeconds = _totalSecondsInCurrentPeriod;
            UpdateDisplay();
        }
        // Ayarları yükler ve zamanlayıcıyı sıfırlar
        private void LoadSettingsAndReset()
        {
            if (_settings.IsClassicMode)
            {
                _workDuration = 25 * 60;
                _shortBreakDuration = 5 * 60;
                _longBreakDuration = 15 * 60;
            }
            else
            {
                _workDuration = _settings.WorkDuration * 60;
                _shortBreakDuration = _settings.ShortBreakDuration * 60;
                _longBreakDuration = _settings.LongBreakDuration * 60;
            }
            SetupPeriod(PomodoroState.Stopped);
        }

        private void UpdateDisplay()
        {
            // Ekranda saniye atlamaması için Ceiling (yukarı yuvarlama) kullanıyoruz.
            // Böylece 0.9 saniye kaldığında hala 00:01 görünür, 0 olunca 00:00 olur.
            TimeSpan time = TimeSpan.FromSeconds(Math.Ceiling(_remainingSeconds));
            TimeDisplay = time.ToString(@"mm\:ss");

            if (_totalSecondsInCurrentPeriod > 0)
            {
                // Hassas double değerini kullanarak smooth progress sağlıyoruz
                ProgressValue = _remainingSeconds / _totalSecondsInCurrentPeriod;
            }
            else
            {
                ProgressValue = 1;
            }
        }
    }
}
