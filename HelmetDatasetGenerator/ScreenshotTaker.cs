using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Rage;
using Rage.Native;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

using System.Threading;


namespace HelmetDatasetGenerator
{
    public sealed class ScreenshotTaker
    {
        private static ScreenshotTaker instance = null;

        private static Size resolution = Game.Resolution;
        private static Bitmap bitmap;
        private static System.Drawing.Graphics graphics;
        private static string img_dir;

        public static ScreenshotTaker Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ScreenshotTaker();
                }
                return instance;
            }
        }

        private ScreenshotTaker()
        {

        }

        public bool SaveScreenshot(int num_screenshots)
        {
            Game.LocalPlayer.Character.IsVisible = false;
            NativeFunction.Natives.DisplayHud(false);
            NativeFunction.Natives.DisplayRadar(false);
            bitmap = new Bitmap(resolution.Width, resolution.Height, PixelFormat.Format24bppRgb);
            graphics = System.Drawing.Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(0, 0, 0, 0, resolution);
            img_dir = "C:\\GTAV_helmet_data\\helmet_dataset\\images\\helmet_train\\im" + num_screenshots + ".png";
            File.Delete(@img_dir);
            bitmap.Save(img_dir, ImageFormat.Png);
            return File.Exists(@img_dir);
        }
    }
}