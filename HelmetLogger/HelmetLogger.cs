using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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

        private static Vector3 MultiplyVector3(Vector3 vec1, Vector3 vec2)
        {
            return new Vector3(vec1.X * vec2.X, vec1.Y * vec2.Y, vec1.Z * vec2.Z);
        }

        public static void Main()
        {
            int num_screenshots = 0;
            int num_peds, num_helmets;
            Game.LogTrivial("Helmet Logger " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " has been initialised.");
            StreamWriter writer;
            bool isClose;
            while (true)
            {
                if (Game.IsKeyDown(Keys.PrintScreen))
                {
                    Vector3 player_pos = Game.LocalPlayer.Character.Position;
                    num_peds = 0;
                    writer = new StreamWriter("C:\\GTAV_helmet_data\\im" + num_screenshots + ".txt", true, Encoding.ASCII);
                    Ped[] peds = World.GetAllPeds();
                    Rage.Object[] objects = World.GetAllObjects();
                    Vector3 obj_pos3D, obj_dim3D;
                    Vector2 obj_pos2D, obj_dim2D;
                    Vector2[] bbox_corners = new Vector2[8];
                    Vector3[] bbox_map = { new Vector3(1, 1, 1), new Vector3(-1, 1, 1), new Vector3(1, -1, 1), new Vector3(1, 1, -1), new Vector3(-1, -1, 1), new Vector3(1, -1, -1), new Vector3(-1,1,-1), new Vector3(-1,-1,-1)};
                    Vector2 min_corner = new Vector2(0, 0);
                    Vector2 max_corner = new Vector2(0, 0);
                    num_helmets = 0;
                    foreach (Rage.Object obj in objects)
                    {
                        //Compute 2D bounding box based on width, height and length in 3D space
                        if (obj.Model.GetHashCode() == -537490919 || obj.Model.GetHashCode() == -246563715 && obj.IsVisible && obj.IsRendered && obj.IsOnScreen)
                        {
                            num_helmets++;
                            //Get position (center of the 3D bounding box)
                            obj_pos3D = obj.Position;
                            obj_pos2D = World.ConvertWorldPositionToScreenPosition(obj_pos3D);
                            obj_dim3D = obj.Model.Dimensions/2;
                            //Compute the 2D coordinates of the 3D bounding box corners, then take the min and max component to find the 2D bounding box
                            for(int i=0; i<7; ++i)
                            {
                                bbox_corners[i] = World.ConvertWorldPositionToScreenPosition(obj_pos3D + MultiplyVector3(bbox_map[i], obj_dim3D));
                                if (i == 0)
                                {
                                    min_corner = max_corner = bbox_corners[i];
                                }
                                else
                                {
                                    min_corner = Vector2.Minimize(bbox_corners[i], min_corner);
                                    max_corner = Vector2.Maximize(bbox_corners[i], max_corner);
                                }
                            }
                            //Compute 2D bounding box width and height
                            obj_dim2D = new Vector2(max_corner.X - min_corner.X, max_corner.Y - min_corner.Y);
                            writer.WriteLine("Helmet at (" + obj_pos2D.X + "," + obj_pos2D.Y + ") with size " + obj_dim2D.X + " x " + obj_dim2D.Y);
                            Game.LogTrivial("Helmet at (" + obj_pos2D.X + "," + obj_pos2D.Y + ") with size " + obj_dim2D.X + " x " + obj_dim2D.Y);

                        }
                    }
                    foreach (Ped ped in peds)
                    {   
                        float ped_distance = ped.DistanceTo(player_pos);
                        isClose = ped_distance <= 20;
                        if(ped.IsRendered && ped.IsVisible && ped.IsOnScreen && isClose)
                        {
                            num_peds++;
                            Game.LogTrivial("Distance: " + ped_distance);
                            Vector2 ped_position = World.ConvertWorldPositionToScreenPosition(ped.Position);
                            if (ped.IsWearingHelmet)
                            {
                                writer.WriteLine("Pedestrian " + num_peds + " with helmet at (" + ped_position.X + "," + ped_position.Y + ")");
                                Game.LogTrivial("Pedestrian " + num_peds + " with helmet at (" + ped_position.X + "," + ped_position.Y + ")");
                            }
                            else
                            {
                                writer.WriteLine("Pedestrian " + num_peds + " without helmet at (" + ped_position.X + "," + ped_position.Y + ")");
                                Game.LogTrivial("Pedestrian " + num_peds + " without helmet at (" + ped_position.X + "," + ped_position.Y + ")");
                            }
                        }
                    }
                    num_screenshots++;
                    Game.LogTrivial("Pedestrians found " + num_peds + ", helmets found " + num_helmets);
                    writer.Close();
                }
                if (Game.IsKeyDown(Keys.F12))
                {
                    writer = new StreamWriter("C:\\GTAV_helmet_data\\objects.txt", true, Encoding.ASCII);
                    Rage.Object[] objects = World.GetAllObjects();
                    Vector3 obj_pos3D, obj_dim3D;
                    Vector2 obj_pos2D, obj_dim2D;
                    Vector2[] bbox_corners = new Vector2[8];
                    Vector3[] bbox_map = { new Vector3(1, 1, 1), new Vector3(-1, 1, 1), new Vector3(1, -1, 1), new Vector3(1, 1, -1), new Vector3(-1, -1, 1), new Vector3(1, -1, -1), new Vector3(-1, 1, -1), new Vector3(-1, -1, -1) };
                    Vector2 min_corner = new Vector2(0, 0);
                    Vector2 max_corner = new Vector2(0, 0);
                    foreach (Rage.Object obj in objects)
                    {
                        //Compute 2D bounding box based on width, height and length in 3D space
                        if (obj.IsVisible && obj.IsRendered && obj.IsOnScreen)
                        {
                            //Get position (center of the 3D bounding box)
                            obj_pos3D = obj.Position;
                            obj_pos2D = World.ConvertWorldPositionToScreenPosition(obj_pos3D);
                            obj_dim3D = obj.Model.Dimensions / 2;
                            //Compute the 2D coordinates of the 3D bounding box corners, then take the min and max component to find the 2D bounding box
                            for (int i = 0; i < 7; ++i)
                            {
                                bbox_corners[i] = World.ConvertWorldPositionToScreenPosition(obj_pos3D + MultiplyVector3(bbox_map[i], obj_dim3D));
                                if (i == 0)
                                {
                                    min_corner = max_corner = bbox_corners[i];
                                }
                                else
                                {
                                    min_corner = Vector2.Minimize(bbox_corners[i], min_corner);
                                    max_corner = Vector2.Maximize(bbox_corners[i], max_corner);
                                }
                            }
                            //Compute 2D bounding box width and height
                            obj_dim2D = new Vector2(max_corner.X - min_corner.X, max_corner.Y - min_corner.Y);
                            writer.WriteLine("Object " + obj.Model.GetHashCode() + " " + obj.Model.Name + " at (" + obj_pos2D.X + "," + obj_pos2D.Y + ") with size " + obj_dim2D.X + " x " + obj_dim2D.Y);
                            Game.LogTrivial("Object " + obj.Model.GetHashCode() + " " + obj.Model.Name + " at (" + obj_pos2D.X + "," + obj_pos2D.Y + ") with size " + obj_dim2D.X + " x " + obj_dim2D.Y);

                        }
                    }

                }
                if (Game.IsKeyDown(Keys.NumPad0))
                {
                    Ped[] peds = World.GetAllPeds();
                    foreach (Ped ped in peds)
                    {
                        if (ped.IsRendered && ped.IsVisible && ped.IsOnScreen)
                        {
                            ped.GiveHelmet(false, HelmetTypes.FiremanHelmet, 0);                           
                        }
                    }
                }
                if (Game.IsKeyDown(Keys.NumPad1))
                {
                    Ped[] peds = World.GetAllPeds();
                    foreach (Ped ped in peds)
                    {
                        if (ped.IsRendered && ped.IsVisible && ped.IsOnScreen)
                        {
                            ped.GiveHelmet(false, HelmetTypes.FiremanHelmet, 1);
                        }
                    }
                }
                if (Game.IsKeyDown(Keys.NumPad2))
                {
                    Ped[] peds = World.GetAllPeds();
                    foreach (Ped ped in peds)
                    {
                        if (ped.IsRendered && ped.IsVisible && ped.IsOnScreen)
                        {
                            ped.GiveHelmet(false, HelmetTypes.FiremanHelmet, 2);
                        }
                    }
                }
                if (Game.IsKeyDown(Keys.NumPad3))
                {
                    Ped[] peds = World.GetAllPeds();
                    foreach (Ped ped in peds)
                    {
                        if (ped.IsRendered && ped.IsVisible && ped.IsOnScreen)
                        {
                            ped.GiveHelmet(false, HelmetTypes.FiremanHelmet, 3);
                        }
                    }
                }
                if (Game.IsKeyDown(Keys.NumPad4))
                {
                    Ped[] peds = World.GetAllPeds();
                    foreach (Ped ped in peds)
                    {
                        if (ped.IsRendered && ped.IsVisible && ped.IsOnScreen)
                        {
                            ped.GiveHelmet(false, HelmetTypes.PilotHeadset, 0);
                        }
                    }
                }
                if (Game.IsKeyDown(Keys.NumPad5))
                {
                    Ped[] peds = World.GetAllPeds();
                    foreach (Ped ped in peds)
                    {
                        if (ped.IsRendered && ped.IsVisible && ped.IsOnScreen)
                        {
                            ped.GiveHelmet(false, HelmetTypes.PilotHelmet, 0);
                        }
                    }
                }
                if (Game.IsKeyDown(Keys.NumPad6))
                {
                    Ped[] peds = World.GetAllPeds();
                    foreach (Ped ped in peds)
                    {
                        if (ped.IsRendered && ped.IsVisible && ped.IsOnScreen)
                        {
                            ped.GiveHelmet(false, HelmetTypes.PoliceMotorcycleHelmet, 0);
                        }
                    }
                }
                if (Game.IsKeyDown(Keys.NumPad7))
                {
                    Ped[] peds = World.GetAllPeds();
                    foreach (Ped ped in peds)
                    {
                        if (ped.IsRendered && ped.IsVisible && ped.IsOnScreen)
                        {
                            ped.GiveHelmet(false, HelmetTypes.RegularMotorcycleHelmet, 0);
                        }
                    }
                }
                if (Game.IsKeyDown(Keys.NumPad9))
                {
                    Ped[] peds = World.GetAllPeds();
                    foreach (Ped ped in peds)
                    {
                        if (ped.IsRendered && ped.IsVisible && ped.IsOnScreen)
                        {
                            ped.RemoveHelmet(true);
                        }
                    }
                }
                Rage.GameFiber.Yield();
            }
        }
    }
}
