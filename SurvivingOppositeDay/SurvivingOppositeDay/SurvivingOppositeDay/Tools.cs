using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivingOppositeDay
{
    public static class Tools
    {
        public static class Math
        {
            public static class Points
            {
                public static Point FromVector(Vector2 vector)
                {
                    return new Point((int)vector.X, (int)vector.Y);
                }
            }
            public static class Vectors
            {
                public static Vector2 FromPoint(Point point)
                {
                    return new Vector2(point.X, point.Y);
                }

                public static Vector2 FromTrig(float angle, float magnitude)
                {
                    return new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * magnitude;
                }

                public static float Theta(Vector2 vector)
                {
                    return (float)System.Math.Atan2(vector.Y, vector.X);
                }

                public static float Magnitude(Vector2 vector)
                {
                    return (float)System.Math.Sqrt(System.Math.Pow(vector.X, 2) + System.Math.Pow(vector.Y, 2));
                }
            }
        }
    }
}
