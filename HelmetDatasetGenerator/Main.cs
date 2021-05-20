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
        public static void Main()
        {
            HelmetLogger helmetLogger = new HelmetLogger();
            ScenarioCreator scenarioCreator = new ScenarioCreator();
            ScreenshotTaker screenshotTaker = new ScreenshotTaker();
            bool log_successful;
            while (true)
            {
                if (Game.IsKeyDown(Keys.O))
                {
                    //Change location every 250 screenshots
                    Game.LogTrivial("Num screenshots: " + HelmetLogger.num_screenshots);
                    if (HelmetLogger.num_screenshots % 250 == 0)
                    {
                        scenarioCreator.TeleportToNextLocation();
                        Thread.Sleep(5000);
                    }
                    //Change time of day every 50 screenshots
                    if (HelmetLogger.num_screenshots % 50 == 0)
                    {
                        scenarioCreator.ClearArea();
                        scenarioCreator.GenerateRandomPeds();
                        Thread.Sleep(3000);
                    }
                    if (HelmetLogger.num_screenshots % 100 == 0)
                    {
                        scenarioCreator.RandomWeather();
                        scenarioCreator.RandomTimeOfDay();
                        Thread.Sleep(1000);
                    }
                    scenarioCreator.GenerateRandomCamera();
                    Thread.Sleep(500);
                    //Game.IsPaused = true;
                    log_successful = helmetLogger.LogInformationOnScreen();
                    if (log_successful)
                    {
                        screenshotTaker.TakeScreenShot();
                        helmetLogger.IncrementScreenshots();
                    }
                    //Game.IsPaused = false;
                    Thread.Sleep(3000);
                }
                if (Game.IsKeyDown(Keys.Y))
                {
                    helmetLogger.SetNumScreenshots(20000);
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
