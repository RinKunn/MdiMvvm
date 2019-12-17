using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MdiMvvm.ValueObjects
{
    public class SnapshotManager
    {
        public SnapshotManager()
        {
            string snapsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gb_mdi");
            if (!Directory.Exists(snapsPath)) Directory.CreateDirectory(snapsPath);
        }

        public ImageSource GetSnapshot(MdiWindow window)
        {
            if (window == null) throw new ArgumentNullException("GetSnapshot has null window");
            string uid = window.Uid;
            window.Closing += (o, e) => Window_Closing(uid);
            window.WindowStateChanged += (o, e) => { if (e.NewValue != WindowState.Minimized) Window_Closing(uid); };
            var snapSource = LoadSnapshot(window.Uid) ?? CreateSnapshot(window);
            return snapSource;
        }

        private void Window_Closing(string uid)
        {
            Console.WriteLine($"win: {uid}  is closing");
            DeleteSnapshot(uid);
        }

        public bool HasSnapshot(MdiWindow window)
        {
            return File.Exists(GetSnapshotfilename(window.Uid));
        }

        private void DeleteSnapshot(string uid)
        {
            //window.ImageSource = null;
            string filename = GetSnapshotfilename(uid);
            if (File.Exists(filename)) File.Delete(filename);
        }

        private void SaveSnapshot(RenderTargetBitmap bitmap, string uid)
        {
            string filename = GetSnapshotfilename(uid);

            JpegBitmapEncoder jpg = new JpegBitmapEncoder();
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
            var bitmap = new RenderTargetBitmap((int)Math.Round(window.Width), (int)Math.Round(window.Height), 96, 96, PixelFormats.Default);
            var drawingVisual = new DrawingVisual();
            using (var context = drawingVisual.RenderOpen())
            {
                var brush = new VisualBrush(window);
                context.DrawRectangle(brush, null, new Rect(new Point(), new Size(window.Width, window.Height)));
                context.Close();
            }
            bitmap.Render(drawingVisual);

            SaveSnapshot(bitmap, window.Uid);
            return bitmap;
        }

        private string GetSnapshotfilename(string uid)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gb_mdi", $"snp_{uid}.snap");
        }
    }
}
