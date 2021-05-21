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
using System.Threading;

[assembly: Rage.Attributes.Plugin("Helmet Dataset Generator", Description = "Plugin to generate images for ML purposes", Author = "Daniele e Pierpaolo")]

namespace HelmetDatasetGenerator
{
    public static class EntryPoint
    {
        private static int num_screenshots;
        public static void Main()
        {
            HelmetLogger helmetLogger = HelmetLogger.Instance;
            ScenarioCreator scenarioCreator = ScenarioCreator.Instance;
            while (true)
            {
                if (Game.IsKeyDown(Keys.O))
                {
                    //Change location every 250 screenshots
                    Game.LogTrivial("Num screenshots: " + num_screenshots);
                    if (num_screenshots % 250 == 0)
                    {
                        scenarioCreator.TeleportToNextLocation();
                        Thread.Sleep(5000);
                    }
                    //Change time of day every 50 screenshots
                    if (num_screenshots % 50 == 0)
                    {
                        scenarioCreator.ClearArea();
                        scenarioCreator.GenerateRandomPeds();
                        Thread.Sleep(3000);
                    }
                    //Change weather and time of day every 100 screenshots
                    if (num_screenshots % 100 == 0)
                    {
                        scenarioCreator.RandomWeather();
                        scenarioCreator.RandomTimeOfDay();
                        Thread.Sleep(1000);
                    }
                    scenarioCreator.GenerateRandomCamera();
                    Thread.Sleep(500);
                    //Game.IsPaused = true;
                    if (helmetLogger.LogInformationOnScreen(num_screenshots))
                    {
                        num_screenshots++;
                    }
                    //Game.IsPaused = false;
                    Thread.Sleep(3000);
                }
                if (Game.IsKeyDown(Keys.Y))
                {
                    num_screenshots = 50000;
                }
                if (Game.IsKeyDown(Keys.U))
                {
                    Camera.DeleteAllCameras();
                }
                Rage.GameFiber.Yield();
            }
        }
    }
}
