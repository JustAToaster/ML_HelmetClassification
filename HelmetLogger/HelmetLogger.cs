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

[assembly: Rage.Attributes.Plugin("Helmet Logger", Description = "Logs coordinates of peds with helmets.", Author = "Dani")]

namespace HelmetLogger
{
    public static class EntryPoint
    {

        private static Vector3 MultiplyVector3(Vector3 vec1, Vector3 vec2)
        {
            return new Vector3(vec1.X * vec2.X, vec1.Y * vec2.Y, vec1.Z * vec2.Z);
        }
        private static Vector2 DivideVector2(Vector2 vec1, Vector2 vec2)
        {
            return new Vector2(vec1.X / vec2.X, vec1.Y / vec2.Y);
        }

        private static bool InBounds(Vector2 vec1, Vector2 vec2)
        {
            return ((vec1.X >= 0 && vec1.X <= 1) && (vec1.Y >= 0 && vec1.Y <= 1) && (vec2.X >= 0 && vec2.X <= 1) && (vec2.Y >= 0 && vec2.Y <= 1));
        }

        private static bool HasHelmet(Ped ped)
        {
            //Get its model hash to recognize the character
            uint ped_hash = ped.Model.Hash;
            //Get prop index (drawable ID): it returns -1 if it has no helmet/hat
            int prop_index = NativeFunction.Natives.GET_PED_PROP_INDEX<int>(ped, 0);
            if(prop_index == -1)
            {
                return false;
            }
            //We need to check for every possible character that can wear a protection helmet
            //If it just returns true, any hat the character wears is a protection helmet
            switch (ped_hash)
            {
                //Casey
                case 0xEA969C40:
                    //Helmet is at drawableID 0
                    return true;
                //CS Dave Norton
                case 0x8587248C:
                    //Helmet is at drawableID 0
                    return true;
                //CS Jimmy Di Santo
                case 0xB8CC92B4:
                    //Helmet is at drawableID 0
                    return true;
                //CS Nervous Ron
                case 0x7896DA94:
                    if (prop_index == 1) return true;
                    else return false;
                //CS Wade
                case 0xD266D9D6:
                    //Helmet is at drawableID 0
                    return true;
                //HC Driver (exclude for now, he wears bike helmets)
                //case 0x3B474ADF:
                //    if (prop_index >= 2) return true;
                //    else return false;
                //HC Gunman
                case 0xB881AEE:
                    if (prop_index == 1 || prop_index == 5) return true;
                    else return false;
                //IG Dave Norton
                case 0x15CD4C33:
                    //Helmet is at drawableID 0
                    return true;
                //IG Jimmy Di Santo
                case 0x570462B9:
                    //Helmet is at drawableID 0
                    return true;
                //IG Lamar Davis (exclude for now, he wears bike helmets)
                //case 0x65B93076:
                //    if (prop_index == 1) return true;
                //    else return false;
                //IG Nervous Ron
                case 0xBD006AF1:
                    if (prop_index == 1) return true;
                    else return false;
                //IG Wade
                case 0x92991B72:
                    //Helmet is at drawableID 0
                    return true;
                //MP F Freemode 1
                case 0x9C9EFFD8:
                    if (prop_index == 16 || prop_index == 17 || prop_index == 18) return true;
                    else return false;
                //MP M Freemode 1
                case 0x705E61F2:
                    if (prop_index == 17) return true;
                    else return false;
                //MP M S Armoured 1
                case 0xCDEF5408:
                    if (prop_index == 1) return true;
                    else return false;
                //Player One (Franklin)
                case 0x9B22DBAF:
                    if (prop_index == 5 || prop_index == 7) return true;
                    else return false;
                //Player Two (Trevor)
                case 0x9B810FA2:
                    if (prop_index == 0 || prop_index == 4 || prop_index == 5 || prop_index == 6 || prop_index == 9 || prop_index == 11 || prop_index == 24) return true;
                    else return false;
                //Player Zero (Michael)
                case 0xD7114C9:
                    if (prop_index == 0 || prop_index == 4 || prop_index == 5 || prop_index == 26) return true;
                    else return false;
                default:
                    return false;
            }
            return false;
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
                    Vector2 res = new Vector2((float)Game.Resolution.Width, (float)Game.Resolution.Height);
                    num_peds = 0;
                    num_helmets = 0;
                    writer = new StreamWriter("C:\\GTAV_helmet_data\\im" + num_screenshots + ".txt", true, Encoding.ASCII);
                    writer.NewLine = "\n";
                    Ped[] peds = World.GetAllPeds();
                    Rage.Object[] objects = World.GetAllObjects();
                    Vector3 head_pos3D, head_dim3D;
                    Vector2 head_pos2D, head_dim2D;
                    Vector2[] bbox_corners = new Vector2[8];
                    Vector3[] bbox_map = { new Vector3(1, 1, 1), new Vector3(-1, 1, 1), new Vector3(1, -1, 1), new Vector3(1, 1, -1), new Vector3(-1, -1, 1), new Vector3(1, -1, -1), new Vector3(-1,1,-1), new Vector3(-1,-1,-1)};
                    Vector2 min_corner = new Vector2(0, 0);
                    Vector2 max_corner = new Vector2(0, 0);
                    foreach (Ped ped in peds)
                    {   
                        float ped_distance = ped.DistanceTo(player_pos);
                        isClose = ped_distance <= 12;
                        if(ped.IsRendered && ped.IsVisible && ped.IsOnScreen && ped.IsHuman && isClose)
                        {
                            num_peds++;
                            //Game.LogTrivial("Distance: " + ped_distance);
                            Vector2 ped_position = World.ConvertWorldPositionToScreenPosition(ped.Position);
                            //Get position of face (center of the 3D bounding box)
                            head_pos3D = ped.GetBonePosition(PedBoneId.Head);
                            head_pos2D = DivideVector2(World.ConvertWorldPositionToScreenPosition(head_pos3D), res);
                            head_dim3D = new Vector3(0.15f, 0.15f, 0.2f); //Overestimation of the dimensions of a head with an helmet
                            //Compute the 2D coordinates of the 3D bounding box corners, then take the min and max component to find the 2D bounding box
                            for (int i = 0; i < 7; ++i)
                            {
                                bbox_corners[i] = DivideVector2(World.ConvertWorldPositionToScreenPosition(head_pos3D + MultiplyVector3(bbox_map[i], head_dim3D)), res);
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
                            head_dim2D = new Vector2(max_corner.X - min_corner.X, max_corner.Y - min_corner.Y);
                            if (InBounds(head_pos2D, head_dim2D))
                            {
                                if (HasHelmet(ped))
                                {
                                    writer.WriteLine("1 " + head_pos2D.X + " " + head_pos2D.Y + " " + head_dim2D.X + " " + head_dim2D.Y);
                                    num_helmets++;
                                }
                                else
                                {
                                    writer.WriteLine("0 " + head_pos2D.X + " " + head_pos2D.Y + " " + head_dim2D.X + " " + head_dim2D.Y);
                                }
                            }
                            else Game.LogTrivial("Pedestrian found out of image bounds");
                        }
                    }
                    num_screenshots++;
                    Game.LogTrivial("Pedestrians found " + num_peds + ", helmets found " + num_helmets);
                    writer.Close();
                }
                Rage.GameFiber.Yield();
            }
        }
    }
}
