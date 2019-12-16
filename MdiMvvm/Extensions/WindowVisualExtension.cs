using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MdiMvvm.Extensions
{
    public static class WindowVisualExtension
    {
        //private static void ModifyPosition(FrameworkElement fe)
        //{
        //    /// get the size of the visual with margin
        //    Size fs = new Size(
        //        fe.ActualWidth +
        //        fe.Margin.Left + fe.Margin.Right,
        //        fe.ActualHeight +
        //        fe.Margin.Top + fe.Margin.Bottom);

        //    /// measure the visual with new size
        //    fe.Measure(fs);

        //    /// arrange the visual to align parent with (0,0)
        //    fe.Arrange(new Rect(
        //        -fe.Margin.Left, -fe.Margin.Top,
        //        fs.Width, fs.Height));
        //}
        //private static void ModifyPositionBack(FrameworkElement fe)
        //{
        //    /// remeasure a size smaller than need, wpf will
        //    /// rearrange it to the original position
        //    fe.Measure(new Size());
        //}

        public static RenderTargetBitmap CreateSnapshot(this MdiWindow window)
        {
            var bitmap = new RenderTargetBitmap((int)Math.Round(window.Width), (int)Math.Round(window.Height), 96, 96, PixelFormats.Default);
            var drawingVisual = new DrawingVisual();
            using (var context = drawingVisual.RenderOpen())
            {
                var brush = new VisualBrush(window);
                context.DrawRectangle(brush, null, new Rect(new Point(), new Size(window.Width, window.Height)));
                context.Close();
            }

            bitmap.Render(drawingVisual);

            //string f = $"./screenshoot_{window.Title}.jpg";
            //ModifyPosition(window as FrameworkElement);
            //bitmap.Render(window);
            //JpegBitmapEncoder e = new JpegBitmapEncoder();
            //e.Frames.Add(BitmapFrame.Create(bitmap));

            ///// new a FileStream to write the image file
            //FileStream s = new FileStream(f,
            //    FileMode.OpenOrCreate, FileAccess.Write);
            //e.Save(s);
            //s.Close();
            //ModifyPositionBack(window as FrameworkElement);

            return bitmap;
        }      
    }
}
