using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MdiMvvm.Extensions
{
    public static class SnapshotManager
    {
        private static string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GalaxyBond", "temp", "snaps");

        static SnapshotManager()
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        internal static ImageSource GetSnapshot(this MdiWindow window)
        {
            if (window == null) throw new ArgumentNullException("GetSnapshot has null window");
            var snapSource = LoadSnapshot(window.Uid) ?? window.CreateSnapshot();
            return snapSource;
        }

        internal static bool HasSnapshot(this MdiWindow window)
        {
            string filename = GetSnapshotfilename(window.Uid);
            return File.Exists(filename)
                && ((new FileInfo(filename)).Length / 1024) > 5;
        }

        internal static void DeleteSnapshot(this MdiWindow window)
        {
            string filename = GetSnapshotfilename(window.Uid);
            if (File.Exists(filename)) File.Delete(filename);
        }

        internal static RenderTargetBitmap CreateSnapshot(this MdiWindow window)
        {
            double width = window.ActualWidth > 0 ? window.ActualWidth : window.Width;
            double height = window.ActualHeight > 0 ? window.ActualHeight : window.Height;
            int widthInt = (int)Math.Round(width);
            int heightInt = (int)Math.Round(height);

            var bitmap = new RenderTargetBitmap(widthInt, heightInt, 96, 96, PixelFormats.Pbgra32);
            var drawingVisual = new DrawingVisual();
            using (var context = drawingVisual.RenderOpen())
            {
                var brush = new VisualBrush(window);
                context.DrawRectangle(brush, null, new Rect(new Point(), new Size(width, height)));
                context.Close();
            }
            bitmap.Render(drawingVisual);

            SaveSnapshot(bitmap, window.Uid);
            return bitmap;
        }


        private static string GetSnapshotfilename(string uid)
        {
            return Path.Combine(path, $"snp_{uid}.snap");
        }

        private static ImageSource LoadSnapshot(string uid)
        {
            string filename = GetSnapshotfilename(uid);
            if (!File.Exists(filename)) return null;
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(filename, UriKind.Absolute);
            bitmap.EndInit();
            return bitmap;
        }

        private static void SaveSnapshot(RenderTargetBitmap bitmap, string uid)
        {
            string filename = GetSnapshotfilename(uid);

            PngBitmapEncoder jpg = new PngBitmapEncoder();
            jpg.Frames.Add(BitmapFrame.Create(bitmap));
            using (Stream stm = File.Create(filename))
            {
                jpg.Save(stm);
            }
        }
    }
}
