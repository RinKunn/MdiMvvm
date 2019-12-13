using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MdiMvvm.Extensions
{
    public static class WindowVisualExtension
    {
        public static RenderTargetBitmap CreateSnapshot(this MdiWindow window)
        {
            Visibility oldVisibility = window.Visibility;

            if (window.Visibility != Visibility.Visible) window.Visibility = Visibility.Visible;

            var bitmap = new RenderTargetBitmap((int)Math.Round(window.Width), (int)Math.Round(window.Height), 96, 96, PixelFormats.Default);
            var drawingVisual = new DrawingVisual();
            using (var context = drawingVisual.RenderOpen())
            {
                var brush = new VisualBrush(window);
                context.DrawRectangle(brush, null, new Rect(new Point(), new Size(window.Width, window.Height)));
                context.Close();
            }

            bitmap.Render(drawingVisual);

            window.Visibility = oldVisibility;
            return bitmap;
        }      
    }
}
