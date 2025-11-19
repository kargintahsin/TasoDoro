using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TasoDoro.ViewModels
{
    // ObservableObject: Değişkenler değiştiğinde arayüzü (UI) otomatik uyarır.
    public partial class TimerViewModel : ObservableObject
    {
        private readonly IDispatcherTimer _timer;
        private int _totalSeconds = 25*60; //25 * 60;
        private int _remainingSeconds;
        

        // Ekranda görünecek süre (Örn: "24:59")
        [ObservableProperty]
        private string timeDisplay;
        [ObservableProperty]
        private int pomodoroCount = 0;
        // İlerleme çubuğu için (0 ile 1 arası değer)
        [ObservableProperty]
        private double progressValue;

        // Timer çalışıyor mu? (Butonları gizleyip göstermek için)
        [ObservableProperty]
        private bool isRunning;

        public TimerViewModel()
        {
            _remainingSeconds = _totalSeconds;
            UpdateDisplay();

            // MAUI'nin kendi Timer yapısını kullanıyoruz
            _timer = Application.Current.Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_remainingSeconds > 0)
            {
                _remainingSeconds--;
                UpdateDisplay();
            }
            else
            {
                Stop(); // Süre bitti
                PomodoroCount += 1;
                _remainingSeconds = _totalSeconds;
                UpdateDisplay();
                // İleride buraya "Zil Sesi Çal" kodu ekleyeceğiz
            }
        }

        // Başlat Butonu için Komut
        [RelayCommand]
        private void Start()
        {
            if (!IsRunning)
            {
                _timer.Start();
                IsRunning = true;
            }
        }

        // Durdur Butonu için Komut
        [RelayCommand]
        private void Stop()
        {
            if (IsRunning)
            {
                _timer.Stop();
                IsRunning = false;
            }
        }

        // Sıfırla Butonu için Komut
        [RelayCommand]
        private void Reset()
        {
            Stop();
            _remainingSeconds = _totalSeconds;
            //Eğer döngü sayısı da sıfırlanacaksa 
            // pomodoroCount=0;
            UpdateDisplay();
        }

        // Helper: Süreyi ekrana uygun formata çevirir
        private void UpdateDisplay()
        {
            TimeSpan time = TimeSpan.FromSeconds(_remainingSeconds);
            TimeDisplay = time.ToString(@"mm\:h");

            // Progress bar 1'den geriye doğru azalacak
            ProgressValue = (double)_remainingSeconds / _totalSeconds;
        }
    }
}