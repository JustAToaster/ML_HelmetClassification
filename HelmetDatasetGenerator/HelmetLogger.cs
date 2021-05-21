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


namespace HelmetDatasetGenerator
{
    public sealed class HelmetLogger
    {
        private static HelmetLogger instance = null;

        private static Vector3[] bbox_map = new Vector3[] { new Vector3(1, 1, 1), new Vector3(-1, 1, 1), new Vector3(1, -1, 1), new Vector3(1, 1, -1), new Vector3(-1, -1, 1), new Vector3(1, -1, -1), new Vector3(-1, 1, -1), new Vector3(-1, -1, -1) };
        private static Vector2 min_box;
        private static Vector3 head_dim3D;
        private static Vector2 min_corner;
        private static Vector2 max_corner;
        private static int num_peds, num_helmets;
        private static StreamWriter writer;
        private static bool isClose;
        private static Vector2 res;
        private static string txt_dir;
        private static ScreenshotTaker screenshotTaker = ScreenshotTaker.Instance;

        private HelmetLogger()
        {
            min_corner = new Vector2(0, 0);
            max_corner = new Vector2(0, 0);
            res = new Vector2((float)Game.Resolution.Width, (float)Game.Resolution.Height);
            min_box = DivideVector2(new Vector2(20.0f, 20.0f), res); //Minimum possible size of a 2D bounding box in pixels, normalized with the resolution 
            head_dim3D = new Vector3(0.15f, 0.17f, 0.23f); //Overestimation of a head with a helmet
            isClose = false;
        }

        public static HelmetLogger Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new HelmetLogger();
                }
                return instance;
            }
        }

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

        private static bool BoxBigEnough(Vector2 head_dim2D)
        {
            return ((head_dim2D.X > min_box.X) && (head_dim2D.Y > min_box.Y));
        }

        //Check for occlusion by tracing a ray to 27 points uniformly distributed around the head center
        private static bool IsHeadNotOccluded(Ped ped, Vector3 camera_pos, Vector3 head_pos)
        {
            float pos_shift = 0.04f;
            Vector3[] shift_map = new Vector3[] {
                new Vector3(pos_shift, pos_shift, pos_shift),
                new Vector3(pos_shift, pos_shift, 0),
                new Vector3(pos_shift, 0, pos_shift),
                new Vector3(0, pos_shift, pos_shift),
                new Vector3(0, 0, pos_shift),
                new Vector3(0, pos_shift, 0),
                new Vector3(pos_shift, 0, 0),
                new Vector3(-pos_shift, pos_shift, pos_shift),
                new Vector3(-pos_shift, -pos_shift, pos_shift),
                new Vector3(-pos_shift, pos_shift, -pos_shift),
                new Vector3(pos_shift, -pos_shift, 0),
                new Vector3(-pos_shift, 0, pos_shift),
                new Vector3(0, pos_shift, -pos_shift)
            };
            Vector3[] head_points = new Vector3[27];
            for (int i = 0; i < 13; ++i)
            {
                head_points[2*i] = head_pos + shift_map[i];
                head_points[2*i + 1] = head_pos - shift_map[i];
            }
            head_points[26] = head_pos;
            HitResult res;
            foreach (Vector3 head_point in head_points)
            {
                res = World.TraceLine(camera_pos, head_point, TraceFlags.IntersectEverything, ped);
                if (!res.Hit)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool HasHelmet(Ped ped)
        {
            //Get its model hash to recognize the character
            String ped_name = ped.Model.Name;
            //Get prop index (drawable ID): it returns -1 if it has no helmet/hat
            int prop_index = NativeFunction.Natives.GET_PED_PROP_INDEX<int>(ped, 0);
            if (prop_index == -1)
            {
                return false;
            }
            //We need to check for every possible character that can wear a protection helmet
            //If it just returns true, any hat the character wears is a protection helmet
            switch (ped_name)
            {
                case "G_M_M_CHEMWORK_01":
                case "S_M_Y_FIREMAN_01":
                case "U_M_M_FIBARCHITECT":
                case "PLAYER_ZERO":
                case "S_M_Y_CONSTRUCT_02":
                case "S_M_M_DOCKWORK_01":
                    if (prop_index == 0) return true;
                    else return false;
                case "PLAYER_ONE":
                    if (prop_index == 5 || prop_index == 7) return true;
                    else return false;
                case "PLAYER_TWO":
                    if (prop_index == 0 || prop_index == 4) return true;
                    else return false;
                case "S_M_Y_CONSTRUCT_01":
                case "S_M_Y_DOCKWORK_01":
                    if (prop_index == 1) return true;
                    else return false;
                default:
                    return false;
            }
        }

        private Vector2 GetHead2DBBox(Vector3 head_pos3D)
        {
            Vector2[] bbox_corners = new Vector2[8];
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
            return new Vector2(max_corner.X - min_corner.X, max_corner.Y - min_corner.Y);
        }

        public bool LogInformationOnScreen(int num_screenshots)
        {
            num_peds = 0;
            num_helmets = 0;
            txt_dir = "C:\\GTAV_helmet_data\\helmet_dataset\\labels\\helmet_train\\im" + num_screenshots + ".txt";
            File.Delete(@txt_dir);
            writer = new StreamWriter(txt_dir, true, Encoding.ASCII);
            writer.NewLine = "\n";
            Ped[] peds = World.GetAllPeds();
            Vector2 head_dim2D, head_pos2D;
            foreach (Ped ped in peds)
            {
                float ped_distance = ped.DistanceTo(Camera.RenderingCamera.Position);
                //Game.LogTrivial("Distance " + ped_distance.ToString() + " for ped " + ped.Model);
                isClose = ped_distance <= 150;
                //Get position of face (center of the 3D bounding box)
                Vector3 head_pos3D = ped.GetBonePosition(PedBoneId.Head);
                if (ped.IsRendered && ped.IsVisible && ped.IsOnScreen && ped.IsHuman && isClose && IsHeadNotOccluded(ped, Camera.RenderingCamera.Position, head_pos3D))
                {
                    //Game.LogTrivial("Trying to annotate for ped " + ped.Model);
                    //num_peds++;
                    head_dim2D = GetHead2DBBox(head_pos3D);
                    //Game.LogTrivial("Got head bbox of ped " + ped.Model);
                    head_pos2D = DivideVector2(World.ConvertWorldPositionToScreenPosition(head_pos3D), res);
                    if (InBounds(head_pos2D, head_dim2D) && BoxBigEnough(head_dim2D))
                    {
                        string class_num;
                        if (HasHelmet(ped))
                        {
                            class_num = "1 ";
                        }
                        else
                        {
                            class_num = "0 ";
                        }
                        writer.WriteLine(class_num + head_pos2D.X + " " + head_pos2D.Y + " " + head_dim2D.X + " " + head_dim2D.Y);
                        //Game.LogTrivial("Pedestrians found " + num_peds + ", helmets found " + num_helmets);
                    }
                    //else Game.LogTrivial("Pedestrian found out of image bounds");
                }
            }
            writer.Close();
            if (File.Exists(@txt_dir) && screenshotTaker.SaveScreenshot(num_screenshots)) return true;
            File.Delete(@txt_dir);
            return false;
        }
    }
}
