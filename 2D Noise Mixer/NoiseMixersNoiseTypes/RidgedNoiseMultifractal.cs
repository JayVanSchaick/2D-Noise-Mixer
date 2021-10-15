
namespace NoiseMixer
{

    public class RidgedNoiseMultifractal : INoise 
    {
        //the noise to be used settings
        IGradientNiose NoiseType;

        // the octaves settings
        uint octaves;
        double persistence;
        double lacunarity;
        double initFrequency;
        bool normalizeReturn;
        double offset;

        /// <summary>
        /// A "Default Constructor" for Ridged Noise Multi-fractal. Only meant for quick Billow Noise, not suitable for most use cases.
        /// </summary>
        /// <param name="Octaves">The number of levels of detail you want you noise to have. </param>
        public RidgedNoiseMultifractal(uint Octaves, double offset) : this(Octaves,  offset, false)
        {

        }

        /// <summary>
        /// A constructor for Billow Noise. Sets up the Ridged Noise Multi-fractal, to use Perlin noise for its noise source.
        /// </summary>
        /// <param name="Octaves">The number of levels of detail you want you noise to have. </param>
        /// <param name="NormalizeReturn">The return data values be between (0,1) if true, or between (-1,1) if false.</param> 
        /// <param name="Persistence">The number that determines how much each octave contributes to the overall shape.</param>
        /// <param name="Lacunarity">The number that determines how much detail is added or removed at each octave.</param>
        /// <param name="InitFrequency">The starting value the frequency should to initialized at.</param>
        public RidgedNoiseMultifractal(uint Octaves, double offset, bool NormalizeReturn, float Persistence = 0.5f, float Lacunarity = 2f, float InitFrequency = 1f)
        : this(new PerlinNoise(), Octaves, offset, NormalizeReturn, Persistence, Lacunarity, InitFrequency)
        {


        }

        /// <summary>
        /// A constructor for Ridged Noise Multi-fractal, give the max control over set up;
        /// </summary>
        /// <param name="NoiseType">The type of noise to use from the NoiseMixer Library, Perlin, Simplex, etc. </param>
        /// <param name="Octaves">The number of levels of detail you want you noise to have. </param>
        /// <param name="NormalizeReturn">The return data values be between (0,1) if true, or between (-1,1) if false.</param> 
        /// <param name="Persistence">The number that determines how much each octave contributes to the overall shape.</param>
        /// <param name="Lacunarity">The number that determines how much detail is added or removed at each octave.</param>
        /// <param name="InitFrequency">The starting value the frequency should to initialized at.</param>
        public RidgedNoiseMultifractal(IGradientNiose gradientNiose, uint Octaves, double offset, bool NormalizeReturn, float Persistence = 0.5f, float Lacunarity = 2f, float InitFrequency = 1f)
        {

            NoiseType = gradientNiose;

            this.Octaves = Octaves;
            this.Persistence = Persistence;
            this.Lacunarity = Lacunarity;
            this.InitFrequency = InitFrequency;
            this.offset = offset;
            this.NormalizeReturn = NormalizeReturn;

        }

        /// <summary>
        /// Get or Set the Octaves, this controls number of levels of detail you want you noise to have.
        /// </summary>
        public uint Octaves { get => octaves; set => octaves = value; }

        /// <summary>
        /// The number that determines how much each octave contributes to the overall shape.
        /// </summary>
        public double Persistence { get => persistence; set => persistence = value; }

        /// <summary>
        /// The number that determines how much detail is added or removed at each octave.
        /// </summary>
        public double Lacunarity { get => lacunarity; set => lacunarity = value; }

        /// <summary>
        /// The starting value the frequency should to initialized at.
        /// </summary>
        public double InitFrequency { get => initFrequency; set => initFrequency = value; }

        /// <summary>
        /// The offset value to bring Ridged Noise Multi-fractal back into the (-1, 1) range.
        /// </summary>
        public double Offset { get => offset; set => offset = value; }

        /// <summary>
        /// Get or set the noise type being used in the Ridged Noise Multi-fractal generation. 
        /// </summary>
        public IGradientNiose GetNoise { get => NoiseType; set => NoiseType = value; }


        /// <summary>
        /// The return data values be between (0,1) if true, or between (-1,1) if false.
        /// </summary>
        public bool NormalizeReturn { get => normalizeReturn; set => normalizeReturn = value; }
        

        /// <summary>
        /// Get the value of the noise at the specified point.
        /// </summary>
        /// <param name="XPos">The X position on a 2d plain.</param>
        /// <param name="YPos">The Y position on a 2d plain.</param>
        /// <returns>A float</returns>
        public float GetValue(float XPos, float YPos)
        {

            return (float)DerivedNoiseTypes.RidgedNoiseMultifractal(NoiseType, XPos, YPos, Octaves, offset, NormalizeReturn, Persistence, Lacunarity, InitFrequency);

        }

        /// <summary>
        /// Get the value of the noise at the specified point.
        /// </summary>
        /// <param name="XPos">The X position on a 2d plain.</param>
        /// <param name="YPos">The Y position on a 2d plain.</param>
        /// <returns>A double</returns>
        public double GetValue(double XPos, double YPos)
        {
            return DerivedNoiseTypes.RidgedNoiseMultifractal(NoiseType, XPos, YPos, Octaves, offset, NormalizeReturn, Persistence, Lacunarity, InitFrequency);
        }



    }

}


