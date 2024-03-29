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


namespace HelmetDatasetGenerator
{
    public sealed class ScenarioCreator
    {

        private static ScenarioCreator instance = null;

        private static String[] models = { "G_M_M_CHEMWORK_01", "S_M_Y_FIREMAN_01", "U_M_M_FIBARCHITECT", "PLAYER_ZERO", "PLAYER_ONE", "PLAYER_TWO", "S_M_Y_CONSTRUCT_01", "S_M_Y_CONSTRUCT_02", "S_M_M_DOCKWORK_01", "S_M_Y_DOCKWORK_01" };
        private static Ped[] curr_peds = new Ped[15];

        //Training locations
        private static Vector3 cantiere1 = new Vector3(-456.0898f, -998.0738f, 23.84991f);
        private static Vector3 cantiere2 = new Vector3(1384.0f, -2057.1f, 52.0f);
        private static Vector3 cantiere3 = new Vector3(-157.3799f, -984.6835f, 254.1314f);
        private static Vector3 deserto = new Vector3(2121.7f, 4796.3f, 41.1f);
        private static Vector3 scalo = new Vector3(978.5798f, -3061.928f, 5.900765f);

        //Validation locations
        private static Vector3 parco_eolico = new Vector3(2354.0f, 1830.3f, 101.1f);
        private static Vector3 campo_aeroporto = new Vector3(-1336.0f, -3044.0f, 13.9f);

        private static Vector3[] locations;

        public static ScenarioCreator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ScenarioCreator();
                }
                return instance;
            }
        }

        private ScenarioCreator()
        {
            locations = new Vector3[] { cantiere1, cantiere2, cantiere3, deserto, scalo };
        }

        public void SetValidationLocations()
        {
            locations = new Vector3[] { parco_eolico, campo_aeroporto };
        }

        public void SetArbitraryLocation(Vector3 location)
        {
            locations = new Vector3[] { location };
        }

        public double GetRandomNumber(Random random, double minimum, double maximum)
        {
            return random.NextDouble() * (maximum - minimum) + minimum;
        }

        private static Ped random_ped()
        {
            Random r = new Random();
            Vector3 player_pos = Game.LocalPlayer.Character.Position;
            Vector3 spawn = Game.LocalPlayer.Character.Position + new Vector3(r.Next(-10, 10), r.Next(-10, 10), 0);
            
            int random_index = r.Next(0, models.Length - 1);
            String model_name = models[random_index];
            Vector3 ped_pos = spawn + new Vector3(r.Next(-10, 10), r.Next(-10, 10), 0);
            Ped random_ped = new Ped(new Model(model_name), ped_pos, r.Next(0, 360));
            random_ped.SetPositionWithSnap(ped_pos);

            double prob = r.NextDouble();
            int drawableID = 0;
            //Give helmet 50% of the time
            if (prob <= 0.5)
            {
                int[] drawable_list;
                switch (model_name)
                {
                    case "G_M_M_CHEMWORK_01":
                    case "S_M_Y_FIREMAN_01":
                    case "U_M_M_FIBARCHITECT":
                    case "PLAYER_ZERO":
                    case "S_M_Y_CONSTRUCT_02":
                    case "S_M_M_DOCKWORK_01":
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
                    case "S_M_Y_DOCKWORK_01":
                        drawableID = 1;
                        break;
                    default:
                        break;
                }
                NativeFunction.Natives.SET_PED_PROP_INDEX(random_ped, 0, drawableID, 0, true);

            }
            //Give random hat 20% of the time
            else if (prob <= 0.7)
            {
                int[] drawable_list;
                switch (model_name)
                {
                    case "PLAYER_ZERO":
                        drawable_list = new int[] { 1, 6, 8, 9, 12, 13, 21  };
                        drawableID = drawable_list[r.Next(0, drawable_list.Length - 1)];
                        NativeFunction.Natives.SET_PED_PROP_INDEX(random_ped, 0, drawableID, 0, true);
                        break;
                    case "S_M_Y_CONSTRUCT_02":
                        drawableID = 2;
                        NativeFunction.Natives.SET_PED_PROP_INDEX(random_ped, 0, drawableID, 0, true);
                        break;
                    case "S_M_M_DOCKWORK_01":
                        drawableID = 1;
                        NativeFunction.Natives.SET_PED_PROP_INDEX(random_ped, 0, drawableID, 0, true);
                        break;
                    case "PLAYER_ONE":
                        drawable_list = new int[] { 1, 3, 4, 15, 16, 18, 19 };
                        drawableID = drawable_list[r.Next(0, drawable_list.Length - 1)];
                        NativeFunction.Natives.SET_PED_PROP_INDEX(random_ped, 0, drawableID, 0, true);
                        break;
                    case "PLAYER_TWO":
                        drawable_list = new int[] { 1, 3, 7, 10, 12, 13, 26  };
                        drawableID = drawable_list[r.Next(0, drawable_list.Length - 1)];
                        NativeFunction.Natives.SET_PED_PROP_INDEX(random_ped, 0, drawableID, 0, true);
                        break;
                    case "S_M_Y_CONSTRUCT_01":
                        drawable_list = new int[] { 0, 3 };
                        drawableID = drawable_list[r.Next(0, drawable_list.Length - 1)];
                        NativeFunction.Natives.SET_PED_PROP_INDEX(random_ped, 0, drawableID, 0, true);
                        break;
                    case "S_M_Y_DOCKWORK_01":
                        drawable_list = new int[] { 0, 2 };
                        drawableID = drawable_list[r.Next(0, drawable_list.Length - 1)];
                        NativeFunction.Natives.SET_PED_PROP_INDEX(random_ped, 0, drawableID, 0, true);
                        break;
                    //Character has no hat in the game files
                    default:
                        break;
                }
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

            return random_ped;
        }

        public void TeleportToNextLocation()
        {
            Random r = new Random();
            World.TeleportLocalPlayer(locations[r.Next(0, locations.Length)], false);
        }

        public void GenerateRandomCamera()
        {
            Random r = new Random();
            Camera.DeleteAllCameras();
            Camera cam = new Camera(true);


            double prob = r.NextDouble();

            if (prob <= 0.5)
            {
                Vector3 vettore = new Vector3(r.Next(0, 10), r.Next(0, 10), r.Next(0, 4));
                Rotator rot = new Rotator(r.Next(-20, 15), r.Next(-5, 5), r.Next(0, 360));
                cam.Rotation = rot;
                cam.AttachToEntity(Game.LocalPlayer.Character, vettore, false);
            }
            else
            {
                Ped r_ped = curr_peds[r.Next(0, curr_peds.Length)];
                Rotator rot = new Rotator(r.Next(-10, +10), r.Next(-5, 5), 180);
                cam.Rotation = rot;
                cam.AttachToEntity(r_ped, new Vector3(0, (float)GetRandomNumber(r, 0.6, 0.9), 0.7f), false);
            }
        }

        public void RandomTimeOfDay()
        {
            Random t = new Random();
            NativeFunction.Natives.ADD_TO_CLOCK_TIME(t.Next(6, 20), 0, 0);
        }

        public void RandomWeather()
        {
            Random r = new Random();
            World.Weather = (WeatherType)r.Next(0, 13);
        }

        public void GenerateRandomPeds()
        {
            for (int i = 0; i < 15; i++)
            {

                curr_peds[i] = random_ped();
                GameFiber.Sleep(50);

            }
        }
        
        public void ClearArea()
        {
            NativeFunction.Natives.ADD_SCENARIO_BLOCKING_AREA(-1000000.0f, -1000000.0f, -100000.0f, 1000000.0f, 1000000.0f, 100000.0f, 0, 1, 1, 1);
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
            //Game.LogTrivial("num di peds " + all_peds.Length);
            for (int i = 0; i < all_peds.Length; i++)
            {
                float dist = all_peds[i].DistanceTo(Game.LocalPlayer.Character.Position);
                if (dist > 0)
                {
                    all_peds[i].Delete();

                }
            }
        }
    }
}
