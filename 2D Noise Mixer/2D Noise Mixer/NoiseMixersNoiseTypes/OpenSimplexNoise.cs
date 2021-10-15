namespace NoiseMixer
{
    /// <summary>
    /// This class was written by Jay Van Schaick as a wrapper class between the Open Simplex Noise from the NoiseMixerLib and the NoiseMixer.
    /// </summary>
    public class OpenSimplexNoise : IGradientNiose
    {
        private NoiseMixerLib.OpenSimplex openSimplex ;

        double seed;
        bool normalizeReturn;

        /// <summary>
        /// The constructor
        /// </summary>
        public OpenSimplexNoise()
        {

            openSimplex = new NoiseMixerLib.OpenSimplex();

        }

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="Seed">The seed is a starting point in generating random numbers. Changing this will change the randomness.</param>
        public OpenSimplexNoise(float Seed)
        {
            openSimplex = new NoiseMixerLib.OpenSimplex((long)Seed);

        }

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="Seed">The seed is a starting point in generating random numbers. Changing this will change the randomness.</param>
        /// <param name="NormalizeReturn">Should the return data values be between 0 - 1?</param>
        public OpenSimplexNoise(float Seed, bool NormalizeReturn)
        {
            openSimplex = new NoiseMixerLib.OpenSimplex((long)Seed);

            normalizeReturn = NormalizeReturn;

        }

        /// <summary>
        /// Get or Set the seed to be used in the Perlin Noise instance. Does not change the permutation of the perlin noise, but rather uses the z axis as a seed.
        /// </summary>
        public double Seed { get { return seed; } set { openSimplex.Seed((long)value); seed = value; } }

        /// <summary>
        /// The return data values be between (0,1) if true, or between (-1,1) if false.
        /// </summary>
        public bool NormalizeReturn { get { return normalizeReturn; } set { normalizeReturn = value; } }

        /// <summary>
        /// Get the value of the noise at the specified point.
        /// </summary>
        /// <param name="XPos">The X position on a 2d plain.</param>
        /// <param name="YPos">The Y position on a 2d plain.</param>
        /// <returns>A float</returns>
        public float GetValue(float XPos, float YPos)
        {

            if (!normalizeReturn)
            {
                //return data values be between (-1,1)
                return (float)openSimplex.Evaluate((float)XPos, (float)YPos);
            }
            else
            {
                //return data values be between (0,1) 
                return (float)(openSimplex.Evaluate((float)XPos, (float)YPos) + 1) / 2;
            }
        }

        /// <summary>
        /// Get the value of the noise at the specified point.
        /// </summary>
        /// <param name="XPos">The X position on a 2d plain.</param>
        /// <param name="YPos">The Y position on a 2d plain.</param>
        /// <returns>A double</returns>
        public double GetValue(double XPos, double YPos)
        {
            if (!normalizeReturn)
            {
                //return data values be between (-1,1)
                return openSimplex.Evaluate(XPos, YPos);
            }
            else
            {
                //return data values be between (0,1) 
                return (openSimplex.Evaluate(XPos, YPos) + 1) / 2;
            }
        }
    }
}


    