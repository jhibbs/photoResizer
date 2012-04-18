using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;

namespace photoReisze
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                switch (args.Length)
                {
                    case 2:
                        // should be filename and percentage
                        string filename = args[0];
                        double percentage = double.Parse(args[1]);

                        if (System.IO.File.Exists(filename) == false)
                            throw new Exception("File doesn't exist.");

                        Image img;
                        img = new Bitmap(filename);

                        Image newImg = ResizeImage(img, (int)(img.Width * (percentage / 100)));
                        
                        string ret = SaveJpeg(GetNewFileName(filename), newImg, 100);

                        break;
                    default:
                        // show help
                        // ...
                        break;

                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("An error occured:\n" + ex.Message);   
            }
        }

        public static string GetNewFileName(string filename)
        {
            int cur = 1;
            
            string newName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filename), System.IO.Path.GetFileNameWithoutExtension(filename));
            newName = newName + "-resized";

            string ext = "." + System.IO.Path.GetExtension(filename);

            string testName = newName + ext;
            while (System.IO.File.Exists(testName))
            {
                testName = newName + "-" + cur++.ToString() + ext;
            }

            return testName;
        }

        public static string SaveJpeg(string sFileName, System.Drawing.Image img, long nQuality)
        {
            try
            {
                // if the filename is blank then we need to create one.
                while (sFileName.Length == 0)
                {
                    sFileName = System.IO.Path.GetTempPath() + "VN" + DateTime.Now.Millisecond * DateTime.Now.Minute + ".jpg";
                    if (System.IO.File.Exists(sFileName))
                    {
                        sFileName = "";
                    }
                }

                // Build image encoder details
                ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
                System.Drawing.Imaging.EncoderParameters ec = new EncoderParameters(1);
                ec.Param[0] = new EncoderParameter(Encoder.Quality, nQuality);

                // Get jpg format ID
                string sOut = "";

                foreach (ImageCodecInfo en in encoders)
                {
                    if (en.MimeType == "image/jpeg")
                    {
                        // Save
                        sFileName = sFileName.Substring(0, sFileName.LastIndexOf(".")) + ".jpg";
                        img.Save(sFileName, en, ec);
                        sOut = sFileName;
                        break; // TODO: might not be correct. Was : Exit For
                    }
                    else
                    {
                        sOut = "Suitable JPEG encoder format not found";
                    }
                }

                return sOut;

            }
            catch (Exception ex)
            {
                //MessageBox.Show("There was an error saving the image: " & vbCrLf & ex.Message)
                return "";
            }

        }

        public static System.Drawing.Image ResizeImage(System.Drawing.Image img, int newWidth = 800)
        {
            if (img.Width <= newWidth)
            {
                return img;
            }
            float ratio = (float)newWidth / img.Width;
            // gets the ratio
            int newHeight = (int)(img.Height * ratio);

            System.Drawing.Bitmap bmPhoto = new System.Drawing.Bitmap(newWidth, newHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(180, 180);

            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmPhoto);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, newWidth, newHeight);

            g.DrawImage(img, rect, 0, 0, img.Width, img.Height, System.Drawing.GraphicsUnit.Pixel);

            img = bmPhoto;
            g.Dispose();

            return img;
        }

    }
}
