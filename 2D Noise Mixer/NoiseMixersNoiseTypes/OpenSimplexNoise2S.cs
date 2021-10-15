namespace NoiseMixer
{
    /// <summary>
    /// This class was written by Jay Van Schaick as a wrapper class between the Open Simplex Noise from the NoiseMixerLib and the NoiseMixer.
    /// </summary>
    public class OpenSimplexNoise2S : IGradientNiose
    {
        private NoiseMixerLib.OpenSimplex2S openSimplex2S;

        double seed;
        bool normalizeReturn;
        EvaluateType evaluateType;

        public enum EvaluateType
        {
            Regular,
            Noise2_XBeforeY
        }

        /// <summary>
        /// The constructor
        /// </summary>
        public OpenSimplexNoise2S(EvaluateType type )
        {

            openSimplex2S = new NoiseMixerLib.OpenSimplex2S();
            evaluateType = type;

        }

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="Seed">The seed is a starting point in generating random numbers. Changing this will change the randomness.</param>
        public OpenSimplexNoise2S(EvaluateType type, float Seed)
        {
            openSimplex2S = new NoiseMixerLib.OpenSimplex2S((long)Seed);
            evaluateType = type;
        }

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="Seed">The seed is a starting point in generating random numbers. Changing this will change the randomness.</param>
        /// <param name="NormalizeReturn">Should the return data values be between 0 - 1?</param>
        public OpenSimplexNoise2S(EvaluateType type, float Seed, bool NormalizeReturn)
        {
            openSimplex2S = new NoiseMixerLib.OpenSimplex2S((long)Seed);

            normalizeReturn = NormalizeReturn;
            evaluateType = type;
        }

        /// <summary>
        /// Get or Set the seed to be used in the Perlin Noise instance. Does not change the permutation of the perlin noise, but rather uses the z axis as a seed.
        /// </summary>
        public double Seed { get { return seed; } set { openSimplex2S.Seed((long)value); seed = value; } }

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
            if (evaluateType == EvaluateType.Regular)
            {
                if (!normalizeReturn)
                {
                    //return data values be between (-1,1)
                    return (float)openSimplex2S.Noise2((float)XPos, (float)YPos);
                }
                else
                {
                    //return data values be between (0,1) 
                    return (float)(openSimplex2S.Noise2((float)XPos, (float)YPos) + 1) / 2;
                }
            }
            else
            {
                if (!normalizeReturn)
                {
                    //return data values be between (-1,1)
                    return (float)openSimplex2S.Noise2_XBeforeY((float)XPos, (float)YPos);
                }
                else
                {
                    //return data values be between (0,1) 
                    return (float)(openSimplex2S.Noise2_XBeforeY((float)XPos, (float)YPos) + 1) / 2;
                }
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
            if (evaluateType == EvaluateType.Regular)
            {
                if (!normalizeReturn)
                {
                    //return data values be between (-1,1)
                    return openSimplex2S.Noise2(XPos, YPos);
                }
                else
                {
                    //return data values be between (0,1) 
                    return (openSimplex2S.Noise2(XPos, YPos) + 1) / 2;
                }
            }
            else
            {
                if (!normalizeReturn)
                {
                    //return data values be between (-1,1)
                    return openSimplex2S.Noise2_XBeforeY(XPos, YPos);
                }
                else
                {
                    //return data values be between (0,1) 
                    return (openSimplex2S.Noise2_XBeforeY(XPos, YPos) + 1) / 2;
                }
            }

        }
    }
}



