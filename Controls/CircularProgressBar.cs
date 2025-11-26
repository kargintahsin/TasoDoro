using Microsoft.Maui.Graphics;

namespace TasoDoro.Controls
{
    public class CircularProgressBar : GraphicsView, IDrawable
    {
        // BindableProperty tanımları (XAML Binding için zorunludur)
        public static readonly BindableProperty ProgressProperty =
            BindableProperty.Create(nameof(Progress), typeof(double), typeof(CircularProgressBar), 0.0,
                propertyChanged: (b, o, n) => ((CircularProgressBar)b).Invalidate());

        public static readonly BindableProperty ProgressColorProperty =
            BindableProperty.Create(nameof(ProgressColor), typeof(Color), typeof(CircularProgressBar), Colors.Red,
                propertyChanged: (b, o, n) => ((CircularProgressBar)b).Invalidate());

        public static readonly BindableProperty TrackColorProperty =
            BindableProperty.Create(nameof(TrackColor), typeof(Color), typeof(CircularProgressBar), Color.FromArgb("#333333"),
                propertyChanged: (b, o, n) => ((CircularProgressBar)b).Invalidate());

        public static readonly BindableProperty StrokeThicknessProperty =
            BindableProperty.Create(nameof(StrokeThickness), typeof(float), typeof(CircularProgressBar), 10f,
                propertyChanged: (b, o, n) => ((CircularProgressBar)b).Invalidate());

        // Property Wrapper'ları
        public double Progress
        {
            get => (double)GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }

        public Color ProgressColor
        {
            get => (Color)GetValue(ProgressColorProperty);
            set => SetValue(ProgressColorProperty, value);
        }

        public Color TrackColor
        {
            get => (Color)GetValue(TrackColorProperty);
            set => SetValue(TrackColorProperty, value);
        }

        public float StrokeThickness
        {
            get => (float)GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        public CircularProgressBar()
        {
            Drawable = this;
            BackgroundColor = Colors.Transparent;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.SaveState();

            // Boyut ve Merkez Hesaplamaları
            float size = Math.Min(dirtyRect.Width, dirtyRect.Height);
            float halfSize = size / 2;
            float x = dirtyRect.Center.X - halfSize;
            float y = dirtyRect.Center.Y - halfSize;

            // Kalınlık Ayarı
            float thickness = StrokeThickness;
            float padding = thickness / 2;
            
            float drawX = x + padding;
            float drawY = y + padding;
            float drawSize = size - thickness;

            // Canvas'ı -90 derece döndür (0 noktası Tepe/12 yönü olur)
            canvas.Rotate(-90, dirtyRect.Center.X, dirtyRect.Center.Y);

            // Arka Plan Çemberi
            canvas.StrokeColor = TrackColor;
            canvas.StrokeSize = thickness;
            canvas.DrawEllipse(drawX, drawY, drawSize, drawSize);

            // İlerleme Yayı
            if (Progress > 0)
            {
                canvas.StrokeColor = ProgressColor;
                canvas.StrokeSize = thickness;
                canvas.StrokeLineCap = LineCap.Round;

                // Dolu Başla -> Azalarak Git (Saat Yönünde Azalma)
                // Başlangıç Açısı: 360 * (1 - Progress) -> Boşluk arttıkça başlangıç noktası ilerler
                // Bitiş Açısı: 360 (Sabit)
                
                float startAngle = (float)(360 * (Progress - 1));
                float endAngle = -360;

                // Tam doluysa elips çiz (çizim hatası olmaması için)
                if (Progress == 1)
                {
                     canvas.DrawEllipse(drawX, drawY, drawSize, drawSize);
                }
                else
                {
                     canvas.DrawArc(drawX, drawY, drawSize, drawSize, startAngle, endAngle, true, false);
                }
            }

            canvas.RestoreState();
        }
    }
}
