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
    public class ScreenshotTaker
    {
        private static Size resolution = Game.Resolution;
        private static Bitmap bitmap;
        private static System.Drawing.Graphics graphics;
        public ScreenshotTaker()
        {

        }

        public void TakeScreenShot()
        {
            Game.LocalPlayer.Character.IsVisible = false;
            NativeFunction.Natives.DisplayHud(false);
            NativeFunction.Natives.DisplayRadar(false);
            bitmap = new Bitmap(resolution.Width, resolution.Height, PixelFormat.Format24bppRgb);
            graphics = System.Drawing.Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(0, 0, 0, 0, resolution);
            bitmap.Save("C:\\GTAV_helmet_data\\helmet_dataset\\images\\helmet_train\\im" + HelmetLogger.num_screenshots + ".png", ImageFormat.Png);
        }
    }
}