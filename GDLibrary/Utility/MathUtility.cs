/*
Function: 		Provide common mathematical operations on in-built data types e.g. Round(Vector3) 
Author: 		NMCG
Version:		1.0
Bugs:			None
Fixes:			None
*/

using Microsoft.Xna.Framework;
using System;

namespace GDLibrary
{
    public class MathUtility
    {
        public static Vector2 Lerp(Vector2 a, Vector2 b, float lerpFactor)
        {
            //takes two translations x1,y1 and x2,y2 and interpolates linearly between them using a factor    
            return new Vector2(MathHelper.Lerp(a.X, b.X, lerpFactor), MathHelper.Lerp(a.Y, b.Y, lerpFactor));
        }

        public static Vector3 Lerp(Vector3 a, Vector3 b, float lerpFactor)
        {
            //takes two translations x1,y1,z1 and x2,y2,z2 and interpolates linearly between them using a factor    
            return new Vector3(MathHelper.Lerp(a.X, b.X, lerpFactor),
                MathHelper.Lerp(a.Y, b.Y, lerpFactor), MathHelper.Lerp(a.Z, b.Z, lerpFactor));
        }

        public static Color Lerp(Color a, Color b, float lerpFactor)
        {
            //Lerp between R, G, B, and A channels for each color
            return new Color((int)MathHelper.Lerp(a.R, b.R, lerpFactor),
                        (int)MathHelper.Lerp(a.G, b.G, lerpFactor),
                            (int)MathHelper.Lerp(a.B, b.B, lerpFactor),
                                (int)MathHelper.Lerp(a.A, b.A, lerpFactor));
        }

        public static Vector2 Round(Vector2 a, int precision)
        {
            //takes two translations x1,y1 and x2,y2 and interpolates linearly between them using a factor    
            return new Vector2((float)Math.Round(a.X, precision), (float)Math.Round(a.Y, precision));
        }

        public static Vector3 Round(Vector3 a, int precision)
        {
            //takes two translations x1,y1 and x2,y2 and interpolates linearly between them using a factor    
            return new Vector3((float)Math.Round(a.X, precision),
                (float)Math.Round(a.Y, precision),
                    (float)Math.Round(a.Z, precision));
        }
    }
}
