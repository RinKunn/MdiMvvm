using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace MdiMvvm.Extensions
{
    public static class WindowVisualExtension
    {
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

            SaveBitmap(bitmap, window.Uid);

            return bitmap;
        }



        private static void SaveBitmap(RenderTargetBitmap bitmap, string uid)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "gb_mdi");
            string filename = Path.Combine(path, $"snp_{uid}.snap");


            JpegBitmapEncoder jpg = new JpegBitmapEncoder();
            jpg.Frames.Add(BitmapFrame.Create(bitmap));
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            using (Stream stm = File.Create(filename))
            {
                jpg.Save(stm);
            }
        }

        public static ImageSource LoadSnapshot(this MdiWindow window)
        {
            string filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "gb_mdi", $"snp_{window.Uid}.snap");
            if (!File.Exists(filename)) return null;

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(filename, UriKind.Absolute);
            bitmap.EndInit();

            return bitmap;
        }

        public static void DeleteSnapshot(this MdiWindow window)
        {
            window.ImageSource = null;
            string filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "gb_mdi", $"snp_{ window.Uid}.snap");

            if (File.Exists(filename)) File.Delete(filename);
        }

    }
}
