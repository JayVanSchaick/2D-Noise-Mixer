using System.Collections;
using System.Collections.Generic;
using System;
using System.Numerics;

namespace NoiseMixerLib
{
    /// <summary>
    ///  Created by Jay Van Schaick. This class recreates the Worley Noise 2d algorithm.  
    /// </summary>
    public class WorleyNoise
    {

        private Vector2[] points;


        /// <summary>
        /// A new Worley Noise class, where a random point is the value 1 and the farther way a position is the closer to 0 it becomes. 
        /// </summary>
        /// <param name="CanvasXResolution">The X size of the Worley Noise canvas to distribute the points in.</param>
        /// <param name="CanvasYResolution">The X size of the Worley Noise canvas to distribute the points in.</param>
        /// <param name="PointsAmount">The amount of points to be distributed</param>
        /// <param name="Seed">The randomness to use when distributing points.</param>
        /// <param name="MaxDistance">The max distance from a point before a position is zero.</param>
        /// <returns></returns>
        public WorleyNoise(uint CanvasXResolution, uint CanvasYResolution, uint PointsAmount, int Seed)
        {

            points = new Vector2[PointsAmount];

            Random random = new Random(Seed);

            for (int i = 0; i < PointsAmount; i++)
            {

                points[i] = new Vector2(random.Next(0, (int)CanvasXResolution), random.Next(0, (int)CanvasYResolution));

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
        /// <param name="MaxDistance">The max distance from a point before a position is zero.</param>
        public WorleyNoise(uint CanvasXResolution, uint CanvasYResolution, uint XpointsAmount, uint YpointsAmount, int Seed)
        {

            points = new Vector2[XpointsAmount * YpointsAmount];

            Random random = new Random(Seed);

            uint ColumnSize = CanvasXResolution / XpointsAmount;
            uint RowSize = CanvasYResolution / YpointsAmount;

            

            for (int x = 0; x < XpointsAmount; x++)
            {
                for (int y = 0; y < YpointsAmount; y++)
                {
                    points[((x * YpointsAmount) + y) ] = new Vector2(random.Next((int)ColumnSize * x, (int)ColumnSize * (x + 1)), random.Next((int)RowSize * y, (int)RowSize * (y + 1)));
                }


            }
        }

        /// <summary>
        /// Get the value from the first point. More optimized if only looking form the closest point.
        /// </summary>
        /// <param name="XPos">The X position to use.</param>
        /// <param name="YPos">The Y position to use.</param>
        /// <param name="MaxDistance">The max distance from a point before a position is zero.</param>
        /// <returns>A float between 0, and 1</returns>
        public double GetValueFromClosestPoint(double XPos, double YPos, double MaxDistance)
        {

            double closestArrayDist = MaxDistance;

            Vector2 wantedPoint = new Vector2((int)XPos, (int)YPos);

            double lastDistanceCalc;

            foreach (Vector2 point in points)
            {

                if ((lastDistanceCalc = Distance(point, wantedPoint)) < closestArrayDist)
                {
                    closestArrayDist = lastDistanceCalc;
                }
            }


            return Lerp(0, 1, Normalize(closestArrayDist, MaxDistance, 0));


        }

        private float Distance(Vector2 Point1, Vector2 Point2)
        {

            float a = Point1.X - Point2.X;
            float b = Point1.Y - Point2.Y;

            return (float)Math.Sqrt(a * a + b * b);

        }

        private double Lerp(double firstFloat, double secondFloat, double by)
        {
            return firstFloat * (1 - by) + secondFloat * by;
        }

        private double Normalize(double val, double max, double min) { return (val - min) / (max - min); }


    }
}
