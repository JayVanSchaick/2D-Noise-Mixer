

using System;
using System.Collections.Generic;
using System.Numerics;

namespace NoiseMixerLib
{

    /// <summary>
    ///  Created by Jay Van Schaick. This class recreates the Voronoi Noise 2d algorithm to work with my noise mixer.  
    /// </summary>
    public class VoronoiNoise
    {
        private Point[] points;


        /// <summary>
        /// A new Worley Noise class, where a random point is the value 1 and the farther way a position is the closer to 0 it becomes. 
        /// </summary>
        /// <param name="CanvasXResolution">The X size of the Worley Noise canvas to distribute the points in.</param>
        /// <param name="CanvasYResolution">The X size of the Worley Noise canvas to distribute the points in.</param>
        /// <param name="PointsAmount">The amount of points to be distributed</param>
        /// <param name="Seed">The randomness to use when distributing points.</param>
        /// <returns></returns>
        public VoronoiNoise(uint CanvasXResolution, uint CanvasYResolution, uint PointsAmount, int Seed)
        {

            points = new Point[PointsAmount];

            Random random = new Random(Seed);
            Random randomFromList = new Random(Seed + 1);

            double pointValues = 1f / PointsAmount;

           

            List<double> ValuesToChooseFrom = new List<double>() ;

            for (int i = 0; i < PointsAmount; i++)
            {
                ValuesToChooseFrom.Add(pointValues * (i + 1));
            }


            for (int i = 0; i < PointsAmount; i++)
            {
                points[i] = new Point();
                points[i].point2D = new Vector2(random.Next(0, (int)CanvasXResolution), random.Next(0, (int)CanvasYResolution));
                points[i].value = ValuesToChooseFrom[randomFromList.Next(0, ValuesToChooseFrom.Count - 1)];
                ValuesToChooseFrom.Remove(points[i].value);
            }
        }

        /// <summary>
        ///  A new Worley Noise class, where a random point is the value 1 and the farther way a position is the closer to 0 it becomes. 
        ///  This constructor organizes the points in different squares of equal size, assigning the point randomly inside of the square.
        /// </summary>
        /// <param name="CanvasXResolution">The X size of the Worley Noise canvas to distribute the points in.</param>
        /// <param name="CanvasYResolution">The X size of the Worley Noise canvas to distribute the points in.</param>
        /// <param name="XpointsAmount">The amount of columns in the X row to make. The amount of points in each row will be equal to this.</param>
        /// <param name="YpointsAmount">The amount of rows in the y columns to make. The amount of points in each columns will be equal to this.</param>
        /// <param name="Seed">The randomness to use when distributing points.</param>
        public VoronoiNoise(uint CanvasXResolution, uint CanvasYResolution, uint XpointsAmount, uint YpointsAmount, int Seed)
        {

            points = new Point[XpointsAmount * YpointsAmount];

            Random random = new Random(Seed);
            Random randomFromList = new Random(Seed + 1);

            double pointValues = 1f / (XpointsAmount * YpointsAmount);
            UnityEngine.Debug.Log(pointValues);

            List<double> ValuesToChooseFrom = new List<double>();

            for (int i = 0; i < XpointsAmount * YpointsAmount; i++)
            {
                ValuesToChooseFrom.Add(pointValues * (i + 1));
            }



            uint ColumnSize = CanvasXResolution / XpointsAmount;
            uint RowSize = CanvasYResolution / YpointsAmount;

            for (int x = 0; x < XpointsAmount; x++)
            {
                for (int y = 0; y < YpointsAmount; y++)
                {
                    points[((x * YpointsAmount) + y)] = new Point();
                    points[((x * YpointsAmount) + y)].point2D = new Vector2(random.Next((int)ColumnSize * x, (int)ColumnSize * (x + 1)), random.Next((int)RowSize * y, (int)RowSize * (y + 1)));
                    points[((x * YpointsAmount) + y)].value = ValuesToChooseFrom[randomFromList.Next(0, ValuesToChooseFrom.Count - 1)];
                    ValuesToChooseFrom.Remove(points[(x * YpointsAmount) + y].value);
                }


            }
        }

        /// <summary>
        /// Get the value from the first point.
        /// </summary>
        /// <param name="XPos">The X position to use.</param>
        /// <param name="YPos">The Y position to use.</param>
        /// <param name="MaxDistance">The max distance from a point before a position is zero.</param>
        /// <returns>A Double between 0, and 1</returns>
        public double GetValue(double XPos, double YPos)
        {
            Point closestPoint = null;
            double closestArrayDist = float.MaxValue;

            Vector2 wantedPoint = new Vector2((int)XPos, (int)YPos);

            double lastDistanceCalc;

            foreach (Point point in points)
            {

                if ((lastDistanceCalc = Distance(point.point2D, wantedPoint)) < closestArrayDist)
                {
                    closestPoint = point;
                    closestArrayDist = lastDistanceCalc;
                }
            }


            return closestPoint.value;


        }

        private double Distance(Vector2 Point1, Vector2 Point2)
        {

            double a = Point1.X - Point2.X;
            double b = Point1.Y - Point2.Y;

            return Math.Sqrt(a * a + b * b);

        }

        /* private float Lerp(float firstFloat, float secondFloat, float by)
         {
             return firstFloat * (1 - by) + secondFloat * by;
         }

         private float Normalize(float val, float max, float min) { return (val - min) / (max - min); }*/

        private class Point
        {
            public Vector2 point2D;

            public double value;

        }

    }
}


