using System;

using System.IO;
using System.Windows;
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
            string filename = GetSnapshotfilename(uid);

            JpegBitmapEncoder jpg = new JpegBitmapEncoder();
            jpg.Frames.Add(BitmapFrame.Create(bitmap));
            using (Stream stm = File.Create(filename))
            {

                jpg.Save(stm);
            }
        }

        public static ImageSource LoadSnapshot(this MdiWindow window)
        {
            string filename = GetSnapshotfilename(window.Uid);
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
            string filename = GetSnapshotfilename(window.Uid);

            if (File.Exists(filename)) File.Delete(filename);
        }


        private static string GetSnapshotfilename(string uid)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gb_mdi", $"snp_{uid}.snap");
        }

        //public static Bitmap ResizeImage(Image image, int width, int height)
        //{
        //    var destRect = new Rectangle(0, 0, width, height);
        //    var destImage = new Bitmap(width, height);

        //    destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

        //    using (var graphics = Graphics.FromImage(destImage))
        //    {
        //        graphics.CompositingMode = CompositingMode.SourceCopy;
        //        graphics.CompositingQuality = CompositingQuality.HighQuality;
        //        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //        graphics.SmoothingMode = SmoothingMode.HighQuality;
        //        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

        //        using (var wrapMode = new ImageAttributes())
        //        {
        //            wrapMode.SetWrapMode(WrapMode.TileFlipXY);
        //            graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
        //        }
        //    }

        //    return destImage;
        //}

    }
}
