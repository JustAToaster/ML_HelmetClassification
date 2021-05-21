﻿using System;
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
        private static bool isRecording;
        public static void Main()
        {
            HelmetLogger helmetLogger = HelmetLogger.Instance;
            ScenarioCreator scenarioCreator = ScenarioCreator.Instance;
            num_screenshots = 0;
            isRecording = false;
            while (true)
            {
                if (Game.IsKeyDown(Keys.O))
                {
                    isRecording = true;
                }
                if (Game.IsKeyDown(Keys.U))
                {
                    Camera.DeleteAllCameras();
                    isRecording = false;
                }
                if (isRecording)
                {
                    //Change location every 500 screenshots
                    if (num_screenshots % 500 == 0)
                    {
                        scenarioCreator.TeleportToNextLocation();
                        GameFiber.Sleep(5000);
                    }
                    //Regenerate peds on screen every 100 screenshots
                    if (num_screenshots % 100 == 0)
                    {
                        scenarioCreator.ClearArea();
                        scenarioCreator.GenerateRandomPeds();
                        GameFiber.Sleep(3000);
                    }
                    //Change weather and time of day every 250 screenshots
                    if (num_screenshots % 250 == 0)
                    {
                        scenarioCreator.RandomWeather();
                        scenarioCreator.RandomTimeOfDay();
                        GameFiber.Sleep(1000);
                    }
                    scenarioCreator.GenerateRandomCamera();
                    GameFiber.Sleep(500);

                    //Game.IsPaused = true;
                    Game.TimeScale = 0.0f;
                    if (helmetLogger.LogInformationOnScreen(num_screenshots))
                    {
                        num_screenshots++;
                    }
                    Game.TimeScale = 1.0f;
                    //Game.IsPaused = false;
                    GameFiber.Sleep(3000);
                }
                Rage.GameFiber.Yield();
            }
        }
        [Rage.Attributes.ConsoleCommand]
        public static void Command_SetScreenshotNum(int num)
        {
            num_screenshots = num;
        }
        [Rage.Attributes.ConsoleCommand]
        public static void Command_ResetScreenshotNum()
        {
            num_screenshots = 0;
        }
    }
}
