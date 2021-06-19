using System;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Hasslefree.Core.Imaging
{
    public class Transform
    {
        /// <summary>
        /// Resize the specified image and return the resized bitmap.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap Scale(Image image, int width, int height)
        {
            //Resize the image
            Bitmap result = new Bitmap(image, width, height);
            using (Graphics graphics = Graphics.FromImage(result))
            {
                //Resize at highest quality
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;

                //Draw the image into the target bitmap resizing it
                graphics.DrawImage(image, 0, 0, width, height);

                //Save the new bitmap
                using (MemoryStream stream = new MemoryStream())
                {
                    //Create the encoding parameters
                    EncoderParameters encodingParameters = new EncoderParameters(1);
                    encodingParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L); // Set the JPG Quality percentage to 90%.

                    //Encode the image as JPG
                    ImageCodecInfo jpgEncoder = null;
                    ImageCodecInfo[] encodes = ImageCodecInfo.GetImageEncoders();
                    foreach (ImageCodecInfo info in encodes)
                    {
                        if (info.MimeType.Equals("image/jpeg"))
                        {
                            jpgEncoder = info;
                            break;
                        }
						else if ( info.MimeType.Equals( "image/png" ) )
						{
							jpgEncoder = info;
							break;
						}
						else if ( info.MimeType.Equals( "image/gif" ) )
						{
							jpgEncoder = info;
							break;
						}

                    }

                    //Save the resized image to the bitmap
                    result.Save(stream, jpgEncoder, encodingParameters);
                }
            }

            //Return the resized image
            return result;
        }

        /// <summary>
		/// Resize an image maintaining aspect ratio
		/// </summary>
		/// <param name="img"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
        public static Image Resize(Image img, int width, int height)
        {
            Image resized = img;

            //Calculate the ratio of the dimensions
            Double ratioW = (Double)width / (Double)img.Width;
            Double ratioH = (Double)height / (Double)img.Height;

            //Choose the smallest ration to fit within the desired width and height
            Double ratio = Math.Min(ratioW, ratioH) > 1 ? 1 : Math.Min(ratioW, ratioH);

            //Calculate the new dimensions
            Int32 w = (Int32)Math.Floor(img.Width * ratio);
            Int32 h = (Int32)Math.Floor(img.Height * ratio);

            //Resize
            if ((w != img.Width) || (h != img.Height)) resized = Scale(resized, w, h);

            return resized;
        }


		public static byte[] ScaleWidth( System.Data.Linq.Binary data, int newWidth, int quality, ref int height, ref int width, ImageFormat format  )
        {
            try
            {
                byte[] returnData;
                byte[] bytes = data.ToArray();

                using (MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length))
                {
                    ms.Write(bytes, 0, bytes.Length);

					returnData = ScaleWidth( Image.FromStream( ms, true ), newWidth, quality, ref height, ref width, format );
                }

                return returnData;
            }
            catch
            {
                height = 0;
                width = 0;
                return null;
            }
        }

		public static byte[] ScaleWidth( Image img, int newWidth, int quality, ref int height, ref int width, ImageFormat format )
        {
            ImageCodecInfo codec = null;

            //Retrieve the codec for the jpeg format
            foreach (ImageCodecInfo c in ImageCodecInfo.GetImageEncoders())
            {
				if ( c.FormatID == format.Guid )
                {
                    codec = c;
                    break;
                }
            }

            if (codec == null)
                throw new NotSupportedException("...");

            //// Defines the parameters the encoder will use (quality factor)
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            Bitmap bmpOut = null;
            int tmpWidth = 0;
            int tmpHeight = 0;

            try
            {
                Bitmap bmpIn = new Bitmap(img);

                decimal originalWidth = img.Width;
                decimal originalHeight = img.Height;

                //Default use the ratio between widths
                tmpWidth = newWidth;
                tmpHeight = (int)(originalHeight / (originalWidth / newWidth));
                bmpOut = new Bitmap(tmpWidth, tmpHeight);


                Graphics g = Graphics.FromImage(bmpOut);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
	            g.FillRectangle(codec.FormatID == ImageFormat.Jpeg.Guid ? Brushes.White : Brushes.Transparent, 0, 0, tmpWidth, tmpHeight);
	            g.DrawImage(bmpIn, 0, 0, tmpWidth, tmpHeight);

                bmpIn.Dispose();
                g.Dispose();

            }
            catch (Exception ex)
            {
                string err = ex.ToString();
                height = tmpHeight;
                width = tmpWidth;
                return null;
            }

            MemoryStream ms = new MemoryStream();

            bmpOut.Save(ms, codec, encoderParameters);
            bmpOut.Dispose();

            byte[] data = ms.ToArray();

            ms.Dispose();

            height = tmpHeight;
            width = tmpWidth;

            return data;
        }

        /// <summary>
        /// Resize an image keeping aspect ratio, optionally placing the new image on a canvas to preserve the desired width and height
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="canvas"></param>
        /// <param name="canvasColour"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static MemoryStream Resize(Image image, Int32 width, Int32 height, Boolean canvas, Color canvasColour, ImageFormat format)
        {
            using (Bitmap bitmap = new Bitmap(width, height))
            {
                MemoryStream stream = new MemoryStream();
                using (Image resized = Resize(image, width, height))
                {
                    if (canvas && (resized.Width != width || resized.Height != height))
                    {
                        using (EncoderParameters encodingParameters = new EncoderParameters(1))
                        {
                            encodingParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L); // Set the JPG Quality percentage to 90%.

                            ImageCodecInfo jpgEncoder = null;
                            ImageCodecInfo[] encodes = ImageCodecInfo.GetImageEncoders();
                            foreach (ImageCodecInfo info in encodes)
                            {
                                if (info.MimeType.Equals("image/jpeg"))
                                {
                                    jpgEncoder = info;
                                    break;
                                }
								else if ( info.MimeType.Equals( "image/png" ) )
								{
									jpgEncoder = info;
									break;
								}
								else if ( info.MimeType.Equals( "image/gif" ) )
								{
									jpgEncoder = info;
									break;
								}
                            }

                            // Use a graphics object to draw the resized image into the bitmap
                            using (Graphics graphics = Graphics.FromImage(bitmap))
                            {
                                // Set the resize quality modes to high quality
                                graphics.CompositingQuality = CompositingQuality.HighQuality;
                                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                graphics.SmoothingMode = SmoothingMode.HighQuality;
                                graphics.FillRectangle(new SolidBrush(canvasColour), 0, 0, width, height);

                                // Draw the image into the target bitmap
                                int xOffset = (width - resized.Width) / 2;
                                int yOffset = (height - resized.Height) / 2;

                                graphics.DrawImage(resized, xOffset, yOffset);
                                bitmap.Save(stream, jpgEncoder, encodingParameters);
                            }
                        }
                    }
                    else
                    {
                        resized.Save(stream, format);
                    }
                }

                return stream;
            }
        }

        /// <summary>
        /// Resize an image keeping aspect ratio, optionally placing the new image on a canvas to preserve the desired width and height
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="canvas"></param>
        /// <param name="canvasColour"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static Byte[] Resize(Byte[] image, Int32 width, Int32 height, Boolean canvas, Color canvasColour, ImageFormat format)
        {
            using (MemoryStream stream = new MemoryStream(image))
            {
                using (MemoryStream result = Resize(Image.FromStream(stream), width, height, canvas, canvasColour, format))
                {
                    return result.ToArray();
                }
            }
        }
    }
}
