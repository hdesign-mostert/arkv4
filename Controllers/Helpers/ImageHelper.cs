using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Data.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Ark.Controllers.Helpers
{
    public class ImageHelper
    {
        public static byte[] ScaleWidth(System.Data.Linq.Binary data, int newWidth, int quality)
        {
            try
            {
                System.Data.Linq.Binary binary = null;
                byte[] bytes = data.ToArray();

                using (MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length))
                {
                    ms.Write(bytes, 0, bytes.Length);

                    binary = ScaleWidth(Image.FromStream(ms, true), newWidth, quality);
                }

                return binary.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public static System.Data.Linq.Binary ScaleWidth(System.Drawing.Image img, int newWidth, int quality)
        {
            ImageCodecInfo jpegCodec = null;

            //Retrieve the codec for the jpeg format
            foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageEncoders())
            {
                if (codec.FormatID == ImageFormat.Jpeg.Guid)
                {
                    jpegCodec = codec;
                    break;
                }
            }

            if (jpegCodec == null)
                throw new NotSupportedException("...");

            //// Defines the parameters the encoder will use (quality factor)
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);

            Bitmap bmpOut = null;

            try
            {
                Bitmap bmpIn = new Bitmap(img);

                decimal originalWidth = img.Width;
                decimal originalHeight = img.Height;

                //Default use the ratio between widths
                int tmpWidth = newWidth;
                int tmpHeight = (int)(originalHeight / (originalWidth / newWidth));
                bmpOut = new Bitmap(tmpWidth, tmpHeight);


                Graphics g = Graphics.FromImage(bmpOut);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.FillRectangle(Brushes.White, 0, 0, tmpWidth, tmpHeight);
                g.DrawImage(bmpIn, 0, 0, tmpWidth, tmpHeight);

                bmpIn.Dispose();
                g.Dispose();

            }
            catch (Exception ex)
            {
                string err = ex.ToString();
                return null;
            }

            MemoryStream ms = new MemoryStream();

            bmpOut.Save(ms, jpegCodec, encoderParameters);
            bmpOut.Dispose();

            System.Data.Linq.Binary binary = new System.Data.Linq.Binary(ms.GetBuffer());
            ms.Dispose();

            return binary;
        }

        public static byte[] ScaleLogo(byte[] data, int newWidth, int newHeight, int quality)
        {
            if (data == null)
                return null;

            System.Data.Linq.Binary binary = null;
            byte[] bytes = data;

            using (MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length))
            {
                ms.Write(bytes, 0, bytes.Length);

                binary = ScaleLogo(Image.FromStream(ms, true), newWidth, newHeight, quality);
            }

            return binary.ToArray();
        }

        public static byte[] ScaleLogo(System.Drawing.Image img, int newWidth, int newHeight, int quality)
        {
            ImageCodecInfo jpegCodec = null;

            //Retrieve the codec for the jpeg format
            foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageEncoders())
            {
                if (codec.FormatID == ImageFormat.Jpeg.Guid)
                {
                    jpegCodec = codec;
                    break;
                }
            }

            if (jpegCodec == null)
                throw new NotSupportedException("...");

            //// Defines the parameters the encoder will use (quality factor)
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);

            Bitmap bmpOut = null;
            Bitmap cropped = null;

            try
            {
                Bitmap bmpIn = new Bitmap(img);

                decimal originalWidth = img.Width;
                decimal originalHeight = img.Height;
                bool useHeightRatio = false;

                if ((originalWidth / newWidth) > (originalHeight / newHeight))
                    useHeightRatio = true;

                //Default use the ratio between widths
                int tmpWidth = newWidth;
                int tmpHeight = (int)(originalHeight / (originalWidth / newWidth));

                if (useHeightRatio)
                {
                    tmpHeight = newHeight;
                    tmpWidth = (int)(originalWidth / (originalHeight / newHeight));
                }

                bmpOut = new Bitmap(tmpWidth, tmpHeight);

                Graphics g = Graphics.FromImage(bmpOut);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                g.FillRectangle(Brushes.White, 0, 0, tmpWidth, tmpHeight);
                g.DrawImage(bmpIn, 0, 0, tmpWidth, tmpHeight);

                bmpIn.Dispose();
                g.Dispose();

                cropped = new Bitmap(newWidth, newHeight);

                //Width is too large
                if (useHeightRatio)
                {
                    int centerX = tmpWidth / 2;
                    int widthX = newWidth / 2;
                    int startX = centerX - widthX;

                    for (int x = 0; x < cropped.Width; x++)
                    {
                        for (int y = 0; y < cropped.Height; y++)
                        {
                            cropped.SetPixel(x, y, bmpOut.GetPixel(x + startX, y));
                        }
                    }
                } //Height is too large
                else
                {
                    int centerY = tmpHeight / 2;
                    int widthY = newHeight / 2;
                    int startY = centerY - widthY;

                    for (int y = 0; y < cropped.Height; y++)
                    {
                        for (int x = 0; x < cropped.Width; x++)
                        {
                            cropped.SetPixel(x, y, bmpOut.GetPixel(x, y + startY));
                        }
                    }
                }

                bmpOut.Dispose();
            }
            catch (Exception ex)
            {


                return null;
            }

            MemoryStream ms = new MemoryStream();

            cropped.Save(ms, jpegCodec, encoderParameters);
            cropped.Dispose();

            System.Data.Linq.Binary binary = new System.Data.Linq.Binary(ms.GetBuffer());
            ms.Dispose();

            return binary.ToArray();
        }
    }
}
