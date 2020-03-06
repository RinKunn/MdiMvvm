using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MdiMvvm.ValueObjects
{
    public class SnapshotManager
    {
        private static string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GalaxyBond", "temp", "snaps");

        static SnapshotManager()
        {
            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        internal ImageSource GetSnapshot(MdiWindow window)
        {
            if (window == null) throw new ArgumentNullException("GetSnapshot has null window");
            var snapSource = LoadSnapshot(window.Uid) ?? CreateSnapshot(window);
            return snapSource;
        }

        internal bool HasSnapshot(MdiWindow window)
        {
            return File.Exists(GetSnapshotfilename(window.Uid));
        }

        internal void DeleteSnapshot(string uid)
        {
            string filename = GetSnapshotfilename(uid);
            if (File.Exists(filename)) File.Delete(filename);
        }

        private void SaveSnapshot(RenderTargetBitmap bitmap, string uid)
        {
            string filename = GetSnapshotfilename(uid);

            PngBitmapEncoder jpg = new PngBitmapEncoder();
            jpg.Frames.Add(BitmapFrame.Create(bitmap));
            using (Stream stm = File.Create(filename))
            {
                jpg.Save(stm);
            }
        }

        private ImageSource LoadSnapshot(string uid)
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
        public RenderTargetBitmap CreateSnapshot(MdiWindow window)
        {
            var bitmap = new RenderTargetBitmap((int)Math.Round(window.ActualWidth), (int)Math.Round(window.ActualHeight), 96, 96, PixelFormats.Pbgra32);
            var drawingVisual = new DrawingVisual();
            using (var context = drawingVisual.RenderOpen())
            {
                var brush = new VisualBrush(window);
                context.DrawRectangle(brush, null, new Rect(new Point(), new Size(window.ActualWidth, window.ActualHeight)));
                context.Close();
            }
            bitmap.Render(drawingVisual);

            SaveSnapshot(bitmap, window.Uid);
            return bitmap;
        }

        private string GetSnapshotfilename(string uid)
        {
            return Path.Combine(path, $"snp_{uid}.snap");
        }
    }
}
