using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using Rage.Native;
using System.Windows.Forms;

[assembly: Rage.Attributes.Plugin("Helmet Logger", Description = "Logs coordinates of peds with helmets.", Author = "Dani")]

namespace HelmetLogger
{
    public static class EntryPoint
    {
        public static void Main()
        {
            int num_screenshots = 0;
            int num_peds;
            Game.LogTrivial("Helmet Logger " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " has been initialised.");
            while (true)
            {
                if (Game.IsKeyDown(Keys.PrintScreen))
                {
                    num_peds = 0;
                    Ped[] peds = World.GetAllPeds();
                    foreach(Ped ped in peds)
                    {
                        if(ped.IsVisible && ped.IsRendered && ped.IsWearingHelmet)
                        {
                            num_peds++;
                            Vector2 img_position = World.ConvertWorldPositionToScreenPosition(ped.Position);
                            Game.LogTrivial("Pedestrian " + num_peds + " with helmet at (" + img_position.X + "," + img_position.Y + ")");
                        }
                    }
                }
                Rage.GameFiber.Yield();
            }
        }
    }
}
