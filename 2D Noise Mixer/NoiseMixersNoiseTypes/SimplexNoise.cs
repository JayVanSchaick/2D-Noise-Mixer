namespace NoiseMixer
{
    /// <summary>
    /// This class was written by Jay Van Schaick as a wrapper class between the Simplex Noise class from the NoiseMixerLib and the NoiseMixer.
    /// </summary>
    public class SimplexNoise : IGradientNiose
    {

        NoiseMixerLib.SimplexNoise simplexNoise;

        bool normalizeReturn;

        /// <summary>
        /// The constructor
        /// </summary>
        public SimplexNoise()
        {

            simplexNoise = new NoiseMixerLib.SimplexNoise();

        }

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="Seed">The seed is a starting point in generating random numbers. Changing this will change the randomness.</param>
        public SimplexNoise(int Seed)
        {

            simplexNoise = new NoiseMixerLib.SimplexNoise(Seed);

        }

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="Seed">The seed is a starting point in generating random numbers. Changing this will change the randomness.</param>
        /// <param name="NormalizeReturn">The return data values are between (0,1) if true, or between (-1,1) if false.</param>
        public SimplexNoise(int Seed, bool NormalizeReturn)
        {

            simplexNoise = new NoiseMixerLib.SimplexNoise(Seed);

            normalizeReturn = NormalizeReturn;

        }

        /// <summary>
        /// Get or Set the seed to be used in the Perlin Noise instance. Does not change the permutation of the perlin noise, but rather uses the z axis as a seed.
        /// </summary>
        public double Seed { get { return simplexNoise.Seed; } set { simplexNoise.Seed = (int)value; } }

        /// <summary>
        /// The return data values be between (0,1) if true, or between (-1,1) if false.
        /// </summary>
        public bool NormalizeReturn { get { return normalizeReturn; } set { normalizeReturn = value; } }

        public float GetValue(float XPos, float YPos)
        {

            if (!normalizeReturn)
            {
                //return data values be between (-1,1)
                return simplexNoise.Generate(XPos, YPos);
            }
            else
            {
                //return data values be between (0,1) 
                return (simplexNoise.Generate(XPos, YPos) + 1) / 2;
            }
            

        }


        public double GetValue(double XPos, double YPos)
        {
           return  GetValue((float)XPos, (float)YPos);

        }
    }
}
