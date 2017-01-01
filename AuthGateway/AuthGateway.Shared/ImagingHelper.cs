using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Net;
using System.IO;
using System.Drawing;

namespace AuthGateway.Shared
{
    public class ImagingHelper
    {
        public static Image GetImageFromUrl(string url, out byte[] bytes)
        {
            Image image;
            using (WebClient webClient = new WebClient()) {
                byte[] data = webClient.DownloadData(url);

                using (MemoryStream mem = new MemoryStream(data)) {
                    image = Image.FromStream(mem);
                    string filename = "tmp.jpeg";
                    image.Save(filename, ImageFormat.Jpeg);
                    bytes = File.ReadAllBytes(filename);
                }                

            }
            return image;
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage)) {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes()) {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static byte[] ImageToBytes(Image image)
        {
            //ImageConverter converter = new ImageConverter();
            //return (byte[])converter.ConvertTo(img, typeof(byte[]));
            string tempFilename = "tmp.png";
            image.Save(tempFilename, ImageFormat.Png);
            return File.ReadAllBytes(tempFilename);            
        }

        public static Bitmap GetImageFromBytes(byte[] byteArray)
        {
            ImageConverter converter = new ImageConverter();
            return (Bitmap)converter.ConvertFrom(byteArray);            
        }
    }
}
