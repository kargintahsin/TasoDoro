using Microsoft.Maui.Graphics;

namespace TasoDoro.Controls
{
    public class CircularProgressBar : GraphicsView, IDrawable
    {
        public static readonly BindableProperty ProgressProperty =
            BindableProperty.Create(nameof(Progress), typeof(double), typeof(CircularProgressBar), 0.0,

        public static readonly BindableProperty ProgressColorProperty =
            BindableProperty.Create(nameof(ProgressColor), typeof(Color), typeof(CircularProgressBar), Colors.Red, propertyChanged: Invalidate);

        public static readonly BindableProperty TrackColorProperty =
            BindableProperty.Create(nameof(TrackColor), typeof(Color), typeof(CircularProgressBar), Color.FromArgb("#333333"), propertyChanged: Invalidate);

        public static readonly BindableProperty StrokeThicknessProperty =
            BindableProperty.Create(nameof(StrokeThickness), typeof(float), typeof(CircularProgressBar), 10f, propertyChanged: Invalidate);

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

        private static void Invalidate(BindableObject bindable, object oldValue, object newValue)
        {
            ((CircularProgressBar)bindable).Invalidate();
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.SaveState();

            // Calculate dimensions
            float size = Math.Min(dirtyRect.Width, dirtyRect.Height);
            float halfSize = size / 2;
            float x = dirtyRect.Center.X - halfSize;
            float y = dirtyRect.Center.Y - halfSize;

            // Adjust for stroke thickness so it doesn't get clipped
            float thickness = StrokeThickness;
            float padding = thickness / 2;
            
            float drawX = x + padding;
            float drawY = y + padding;
            float drawSize = size - thickness;

            // Rotate -90 degrees so 0 is at Top (12 o'clock)
            canvas.Rotate(-90, dirtyRect.Center.X, dirtyRect.Center.Y);

            // Draw Track (Background Circle)
            canvas.StrokeColor = TrackColor;
            canvas.StrokeSize = thickness;
            canvas.DrawEllipse(drawX, drawY, drawSize, drawSize);

            // Draw Progress (Arc)
            if (Progress > 0)
            {
                canvas.StrokeColor = ProgressColor;
                canvas.StrokeSize = thickness;
                canvas.StrokeLineCap = LineCap.Round;

                
                float startAngle = (float)(360 * (Progress-1));
                float endAngle = -360;

                // Optimization: If full, draw full circle (sometimes Arc has artifacts at closure)
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