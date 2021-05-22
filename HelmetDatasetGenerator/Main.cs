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
        private static int num_screenshots = 0;
        private static int starting_num_screenshots = 0;
        private static int desired_num_screenshots = 5000;
        private static bool isRecording = false;
        private static HelmetLogger helmetLogger = HelmetLogger.Instance;
        private static ScenarioCreator scenarioCreator = ScenarioCreator.Instance;
        public static void RecordScene()
        {
            //Change location every 500 screenshots
            if (num_screenshots % 500 == 0)
            {
                scenarioCreator.TeleportToNextLocation();
                GameFiber.Sleep(7000);
            }
            //Regenerate peds on screen every 100 screenshots
            if (num_screenshots % 100 == 0)
            {
                scenarioCreator.ClearArea();
                scenarioCreator.GenerateRandomPeds();
                GameFiber.Sleep(4000);
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
        public static void Main()
        {
            while (true)
            {
                if (Game.IsKeyDown(Keys.O))
                {
                    Game.LogTrivial("Starting recording...");
                    isRecording = true;
                }
                if (Game.IsKeyDown(Keys.U))
                {
                    Game.LogTrivial("Stopping recording...");
                    Camera.DeleteAllCameras();
                    isRecording = false;
                }
                if (isRecording && num_screenshots - starting_num_screenshots < desired_num_screenshots)
                {
                    RecordScene();
                }
                else
                {
                    isRecording = false;
                    Game.LogTrivial("Finished Recording!");
                }
                Rage.GameFiber.Yield();
            }
        }
        [Rage.Attributes.ConsoleCommand]
        public static void Command_SetScreenshotNum(int num)
        {
            starting_num_screenshots = num;
            num_screenshots = num;
        }
        [Rage.Attributes.ConsoleCommand]
        public static void Command_SetDesiredScreenshotNum(int num)
        {
            desired_num_screenshots = num;
        }
        [Rage.Attributes.ConsoleCommand]
        public static void Command_ResetScreenshotNum()
        {
            starting_num_screenshots = 0;
            num_screenshots = 0;
        }
        [Rage.Attributes.ConsoleCommand]
        public static void Command_StartRecording()
        {
            Game.LogTrivial("Starting recording...");
            isRecording = true;
        }
        [Rage.Attributes.ConsoleCommand]
        public static void Command_StopRecording()
        {
            Game.LogTrivial("Stopping recording...");
            isRecording = false;
        }
    }
}
