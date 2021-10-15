using System;

namespace NoiseMixer
{
    /// <summary>
    /// This class was written by Jay Van Schaick as a wrapper class between the Worley Noise class from the NoiseMixerLib and the NoiseMixer.
    /// </summary>
    public class VoronoiNoise : IVoronoiTypeNiose
    {

        NoiseMixerLib.VoronoiNoise voronoiNoise;

       

        /// <summary>
        /// A new Worley Noise class, where a random point is the value 1 and the farther way a position is the closer to 0 it becomes. The constructor will create a random set of points throughout the "Canvas", anywhere with any distribution. 
        /// </summary>
        /// <param name="CanvasXResolution">The Width of the area to distribute the points.</param>
        /// <param name="CanvasYResolution">The Height of the area to distribute the points. </param>
        /// <param name="PointsAmount">The amount of points to distribute around the map.</param>
        /// <param name="MaxDistance">The max distance from a point before a position is zero.</param>
        public VoronoiNoise(uint CanvasXResolution, uint CanvasYResolution, uint PointsAmount)
            : this(CanvasXResolution, CanvasYResolution, PointsAmount, (int)DateTime.Now.Ticks)
        {

        }

        /// <summary>
        /// A new Worley Noise class, where a random point is the value 1 and the farther way a position is the closer to 0 it becomes. The constructor will create a random set of points throughout the "Canvas", anywhere with any distribution. 
        /// </summary>
        /// <param name="CanvasXResolution">The Width of the area to distribute the points.</param>
        /// <param name="CanvasYResolution">The Height of the area to distribute the points. </param>
        /// <param name="PointsAmount">The amount of points to distribute around the map.</param>
        /// <param name="Seed">The seed of the randomness to use in distributing.</param>
        /// <param name="MaxDistance">The max distance from a point before a position is zero.</param>
        public VoronoiNoise(uint CanvasXResolution, uint CanvasYResolution, uint PointsAmount, int Seed)
        {
            voronoiNoise = new NoiseMixerLib.VoronoiNoise(CanvasXResolution, CanvasYResolution, PointsAmount, Seed);
           

        }

        /// <summary>
        ///  A new Worley Noise class, where a random point is the value 1 and the farther way a position is the closer to 0 it becomes. 
        ///  This constructor organizes the points in different squares of equal size, assigning the point randomly inside of the square.
        /// </summary>
        /// <param name="CanvasXResolution">The Width of the area to distribute the points.</param>
        /// <param name="CanvasYResolution">The Height of the area to distribute the points. </param>
        /// <param name="XpointsAmount">The amount of columns in the X row to make. The amount of points in each row will be equal to this.</param>
        /// <param name="YpointsAmount">The amount of rows in the y columns to make. The amount of points in each columns will be equal to this.</param>
        /// <param name="MaxDistance">The max distance from a point before a position is zero.</param>
        public VoronoiNoise(uint CanvasXResolution, uint CanvasYResolution, uint XpointsAmount, uint YpointsAmount)
            : this(CanvasXResolution, CanvasYResolution, XpointsAmount, YpointsAmount, new System.Random().Next())
        {

        }

        /// <summary>
        ///  A new Worley Noise class, where a random point is the value 1 and the farther way a position is the closer to 0 it becomes. 
        ///  This constructor organizes the points in different squares of equal size, assigning the point randomly inside of the square.
        /// </summary>
        /// <param name="CanvasXResolution">The Width of the area to distribute the points.</param>
        /// <param name="CanvasYResolution">The Height of the area to distribute the points. </param>
        /// <param name="XpointsAmount">The amount of columns in the X row to make. The amount of points in each row will be equal to this.</param>
        /// <param name="YpointsAmount">The amount of rows in the y columns to make. The amount of points in each columns will be equal to this.</param>
        /// <param name="Seed">The seed of the randomness to use in distributing.</param>
        /// <param name="MaxDistance">The max distance from a point before a position is zero.</param>
        public VoronoiNoise(uint CanvasXResolution, uint CanvasYResolution, uint XpointsAmount, uint YpointsAmount, int Seed )
        {
            voronoiNoise = new NoiseMixerLib.VoronoiNoise(CanvasXResolution, CanvasYResolution, XpointsAmount, YpointsAmount, Seed);
        }


        /// <summary>
        /// Get a value of a position on the Worley Noise "canvas".
        /// </summary>
        /// <param name="XPos">The X value of the 2d position wanted.</param>
        /// <param name="YPos">The Y value of the 2d position wanted.</param>
        /// <returns>A float</returns>
        public float GetValue(float XPos, float YPos)
        {
            return (float)voronoiNoise.GetValue(XPos, YPos);
        }

        /// <summary>
        /// Get a value of a position on the Worley Noise "canvas".
        /// </summary>
        /// <param name="XPos">The X value of the 2d position wanted.</param>
        /// <param name="YPos">The Y value of the 2d position wanted.</param>
        /// <returns>A Double</returns>
        public double GetValue(double XPos, double YPos)
        {
            return voronoiNoise.GetValue(XPos, YPos);
        }
    }
}


