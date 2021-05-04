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

[assembly: Rage.Attributes.Plugin("Player Teleport", Description = "", Author = "Pier")]

namespace PlayerTeleport
{
    public static class EntryPoint
    {
        private static void random_ped()
        {
            Vector3 player_pos = Game.LocalPlayer.Character.Position;
            Vector3 cantiere1 = new Vector3((float)-123.7665, (float)-1046.746, (float)27.27356);
            Game.LogTrivial("Posizione player " + player_pos);
            String[] models = { "G_M_M_CHEMWORK_01", "S_M_Y_FIREMAN_01", "U_M_M_FIBARCHITECT", "PLAYER_ZERO", "PLAYER_ONE", "PLAYER_TWO", "S_M_Y_CONSTRUCT_01", "S_M_Y_CONSTRUCT_02", "S_M_M_DOCKWORK_01" };


            Random r = new Random();
            int random_index = r.Next(0, models.Length - 1);
            String model_name = models[random_index];
            Game.LogTrivial("Generating " + model_name);

            Ped random_ped = new Ped(new Model(model_name), cantiere1 + new Vector3(r.Next(-20, 30), r.Next(-5, 5), 0), r.Next(0, 360));
            double prob = r.NextDouble();
            int drawableID = 0;
            if (prob <= 0.5)
            {
                int[] drawable_list;
                switch (model_name)
                {
                    case "G_M_M_CHEMWORK_01":
                    case "S_M_Y_FIREMAN_01":
                    case "U_M_M_FIBARCHITECT":
                    case "PLAYER_ZERO":
                        drawableID = 0;
                        break;
                    case "PLAYER_ONE":
                        drawable_list = new int[] { 5, 7 };
                        drawableID = drawable_list[r.Next(0, drawable_list.Length - 1)];
                        break;
                    case "PLAYER_TWO":
                        drawable_list = new int[] { 0, 4 };
                        drawableID = drawable_list[r.Next(0, drawable_list.Length - 1)];
                        break;
                    case "S_M_Y_CONSTRUCT_01":
                        drawableID = r.Next(1, 2);
                        break;
                    case "S_M_Y_CONSTRUCT_02":
                        drawableID = r.Next(0, 1);
                        break;
                    case "S_M_Y_DOCKWORK_01":
                        drawable_list = new int[] { 1, 3 };
                        drawableID = drawable_list[r.Next(0, drawable_list.Length - 1)];

                        break;
                    case "S_M_M_DOCKWORK_01":
                        drawable_list = new int[] { 0, 2 };
                        drawableID = drawable_list[r.Next(0, drawable_list.Length - 1)];
                        break;
                    default:
                        break;
                }

                double prob2 = r.NextDouble();
                if (prob2 <= 0.3)
                {
                    NativeFunction.Natives.TaskUseMobilePhoneTimed(random_ped, 40000);
                }
                else if (prob2 > 0.7)
                {
                    NativeFunction.Natives.TASK_WANDER_IN_AREA(random_ped, cantiere1 + new Vector3(r.Next(0, 10), r.Next(0, 10), 0), 80, 100, 100);
                }
                NativeFunction.Natives.SET_PED_PROP_INDEX(random_ped, 0, drawableID, 0, true);
            }


        }
        public static void Main()
        {

            
            Game.LogTrivial("Player Teleport" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " has been initialised.");
            Vector3 cantiere1 = new Vector3((float)-123.7665, (float)-1046.746, (float)27.27356);
            Vector3 cam0 = new Vector3((float)-129.7665, (float)-1036.746, (float)32.27356);
            Vector3 cam1 = new Vector3((float)-126.7665, (float)-1050.746, (float)30.27356);
            Vector3 cam2 = new Vector3((float)-115.17, (float)-1044.435, (float)27.27806);
            Vector3 cam3 = new Vector3((float)-125.7179, (float)-1046.953, (float)27.2736);
            Vector3[] camere = { cam0, cam1 , cam2, cam3};
          
            while (true)
            {
                if (Game.IsKeyDown(Keys.I))
                {
                    Vector3 player_pos = Game.LocalPlayer.Character.Position;
                    Game.LogTrivial("Posizione player " + player_pos);

                    World.TeleportLocalPlayer(cantiere1, true);
                }

                if (Game.IsKeyDown(Keys.O))
                {
                    Random r = new Random();
                    int rand = r.Next(0, camere.Length);
                    if (rand==0) {
                        Camera.DeleteAllCameras();
                        Camera cam = new Camera(true);
                        cam.Position = camere[rand];
                        Rotator rot = new Rotator(-45, 0, 180);
                        cam.Rotation = rot;

                    }else if (rand==1)
                    {
                        Camera.DeleteAllCameras();
                        Camera cam = new Camera(true);
                        cam.Position = camere[rand];
                        Rotator rot = new Rotator(-25, 0, 0);
                        cam.Rotation = rot;
                    }
                    else if (rand == 2)
                    {
                        Camera.DeleteAllCameras();
                        Camera cam = new Camera(true);
                        cam.Position = camere[rand];
                        Rotator rot = new Rotator(15, 0, 180);
                        cam.Rotation = rot;
                    }
                    else if (rand == 3)
                    {
                        Camera.DeleteAllCameras();
                        Camera cam = new Camera(true);
                        cam.Position = camere[rand];
                        Rotator rot = new Rotator(15, 0, 180);
                        cam.Rotation = rot;
                    }
                }

                if (Game.IsKeyDown(Keys.L))
                {
                    Camera.DeleteAllCameras();
                }

                if (Game.IsKeyDown(Keys.U))
                {
                   
                    Random t = new Random();
                    NativeFunction.Natives.ADD_TO_CLOCK_TIME(t.Next(6,20), 0, 0);
                }

                if (Game.IsKeyDown(Keys.Y))
                {
                   for(int i = 0;i<10;i++)
                    {

                        random_ped();
                        Thread.Sleep(200);

                    }
                }

                if (Game.IsKeyDown(Keys.T))
                {
                    Ped[] all_peds;
                    all_peds = World.GetAllPeds();
                    for(int i=0; i < all_peds.Length; i++)
                    {
                        float dist = all_peds[i].DistanceTo(Game.LocalPlayer.Character.Position);
                        if (dist < 80 && dist >0) {
                            all_peds[i].Delete();

                        }
                    }
                }
                    Rage.GameFiber.Yield();
                }
            }
        }
    }