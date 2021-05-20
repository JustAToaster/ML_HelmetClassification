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
            Random r = new Random();
            Vector3 player_pos = Game.LocalPlayer.Character.Position;
            Vector3 spawn = Game.LocalPlayer.Character.Position+new Vector3(r.Next(-10, 10), r.Next(-10, 10),0);
            Game.LogTrivial("Posizione player " + player_pos);
            String[] models = { "G_M_M_CHEMWORK_01", "S_M_Y_FIREMAN_01", "U_M_M_FIBARCHITECT", "PLAYER_ZERO", "PLAYER_ONE", "PLAYER_TWO", "S_M_Y_CONSTRUCT_01", "S_M_Y_CONSTRUCT_02", "S_M_M_DOCKWORK_01" };

            int random_index = r.Next(0, models.Length - 1);
            String model_name = models[random_index];
            Game.LogTrivial("Generating " + model_name);

            Ped random_ped = new Ped(new Model(model_name), spawn + new Vector3(r.Next(-10, 10), r.Next(-10, 10), 0), r.Next(0, 360));
         
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
                    NativeFunction.Natives.TASK_WANDER_IN_AREA(random_ped, spawn + new Vector3(r.Next(0, 10), r.Next(0, 10), 0), 0.4f, 0.5f, 0.9f);
                }
                NativeFunction.Natives.SET_PED_PROP_INDEX(random_ped, 0, drawableID, 0, true);
            }


        }
        public static void Main()
        {

            
            Game.LogTrivial("Player Teleport" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " has been initialised.");
            Vector3 cantiere1 = new Vector3(-456.0898f, -998.0738f, 23.84991f);
            Vector3 cantiere2 = new Vector3(1384.0f, -2057.1f, 52.0f);
            Vector3 deserto = new Vector3(2121.7f, 4796.3f, 41.1f);
            Vector3 scalo = new Vector3(978.5798f,-3061.928f, 5.900765f);
            Vector3[] locatios = { cantiere1, cantiere2, deserto, scalo };


            Vector3 cam0 = new Vector3(-129.7665f, -1036.746f, 32.27356f);
            Vector3 cam1 = new Vector3((float)-126.7665, (float)-1050.746, (float)30.27356);
            Vector3 cam2 = new Vector3((float)-115.17, (float)-1044.435, (float)27.27806);
            Vector3 cam3 = new Vector3((float)-125.7179, (float)-1046.953, (float)27.2736);
            Vector3[] camere = { cam0, cam1 , cam2, cam3};
          
            while (true)
            {
                if (Game.IsKeyDown(Keys.I))
                {
                    Random r = new Random();
                    Vector3 player_pos = Game.LocalPlayer.Character.Position;
                    Game.LogTrivial("Posizione player " + player_pos);

                    World.TeleportLocalPlayer(locatios[r.Next(0, locatios.Length)], true);
                }

                if (Game.IsKeyDown(Keys.O))
                {
                    
                    Random r = new Random();
                    Camera.DeleteAllCameras();
                    Camera cam = new Camera(true);
                    Ped[] all_peds;


                    double prob = r.NextDouble();
                  
                    if (prob <= 0.5)
                    {

                        cam.Position = Game.LocalPlayer.Character.Position + new Vector3(r.Next(0, 10), r.Next(0, 10), r.Next(0, 4));
                        Rotator rot = new Rotator(r.Next(-20, 15), 0, r.Next(0, 360));
                        cam.Rotation = rot;


                    }
                    else
                    {

                        Ped r_ped = World.GetAllPeds()[r.Next(0, World.GetAllPeds().Length)];
                        Game.LogTrivial("distaza minima " + r_ped);
                        cam.Position = r_ped.Position + new Vector3(0, 0.7f, 0.7f);
                        Rotator rot = new Rotator(0, 0, 180);
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
                   for(int i = 0;i<20;i++)
                    {

                        random_ped();
                        Thread.Sleep(50);

                    }
                }

                if (Game.IsKeyDown(Keys.T))
                {
                    NativeFunction.Natives.ADD_SCENARIO_BLOCKING_AREA( -1000000.0f, -1000000.0f, -100000.0f, 1000000.0f, 1000000.0f, 100000.0f, 0, 1, 1, 1);
                    NativeFunction.Natives.SET_CREATE_RANDOM_COPS(false);
                    NativeFunction.Natives.SET_RANDOM_BOATS(false);
                    NativeFunction.Natives.SET_RANDOM_TRAINS(false);
                    NativeFunction.Natives.SET_GARBAGE_TRUCKS(false);
      
                    NativeFunction.Natives.SET_PED_POPULATION_BUDGET(0);
                    NativeFunction.Natives.SET_VEHICLE_POPULATION_BUDGET(0);
                    NativeFunction.Natives.SET_ALL_LOW_PRIORITY_VEHICLE_GENERATORS_ACTIVE(false);
                    NativeFunction.Natives.SET_NUMBER_OF_PARKED_VEHICLES(0);    //  -1, 0
  
                    NativeFunction.Natives.DISABLE_VEHICLE_DISTANTLIGHTS(true);

                    Ped[] all_peds;
                    all_peds = World.GetAllPeds();
                    Game.LogTrivial("num di peds " + all_peds.Length);
                    for (int i=0; i < all_peds.Length; i++)
                    {
                        float dist = all_peds[i].DistanceTo(Game.LocalPlayer.Character.Position);
                        if (dist >0) {
                            all_peds[i].Delete();

                        }
                    }
                }
                    Rage.GameFiber.Yield();
                }
            }
        }
    }