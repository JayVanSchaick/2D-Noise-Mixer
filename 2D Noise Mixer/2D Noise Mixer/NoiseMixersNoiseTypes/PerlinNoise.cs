using System;



namespace NoiseMixer
{
    /// <summary>
    /// This class was written by Jay Van Schaick as a wrapper class between the Perlin Noise class from the NoiseMixerLib and the NoiseMixer.
    /// </summary>
    public class PerlinNoise : IGradientNiose
    {
        private NoiseMixerLib.PerlinNoise perlin;

        double seed;
        bool normalizeReturn;

        /// <summary>
        /// The constructor
        /// </summary>
        public PerlinNoise() : this(new Random().Next())
        {

        }

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="Seed">The seed is a starting point in generating random numbers. Changing this will change the randomness.</param>
        public PerlinNoise(float Seed) : this (Seed, false) 
        {

        }

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="Seed">The seed is a starting point in generating random numbers. Changing this will change the randomness.</param>
        /// <param name="NormalizeReturn">Should the return data values be between 0 - 1?</param>
        public PerlinNoise(float Seed, bool NormalizeReturn)
        {
            perlin = new NoiseMixerLib.PerlinNoise();
            seed = Seed;
            normalizeReturn = NormalizeReturn;

        }

        /// <summary>
        /// Get or Set the seed to be used in the Perlin Noise instance. Does not change the permutation of the perlin noise, but rather uses the z axis as a seed.
        /// </summary>
        public double Seed { get { return seed;  } set { seed = value;  } }

        /// <summary>
        /// The return data values be between (0,1) if true, or between (-1,1) if false.
        /// </summary>
        public bool NormalizeReturn { get { return normalizeReturn;  } set { normalizeReturn = value; } }

        /// <summary>
        /// Get the value of the noise at the specified point.
        /// </summary>
        /// <param name="XPos">The X position on a 2d plain.</param>
        /// <param name="YPos">The Y position on a 2d plain.</param>
        /// <returns>A float</returns>
        public float GetValue(float XPos, float YPos)
        {
            return (float)perlin.Perlin(XPos, YPos, seed, normalizeReturn);
        }

        /// <summary>
        /// Get the value of the noise at the specified point.
        /// </summary>
        /// <param name="XPos">The X position on a 2d plain.</param>
        /// <param name="YPos">The Y position on a 2d plain.</param>
        /// <returns>A double</returns>
        public double GetValue(double XPos, double YPos)
        {
             return (float)perlin.Perlin(XPos, YPos, seed, normalizeReturn);
        }
    }
}
