using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoiseMixer
{

    public static class DerivedNoiseTypes
    {


        /// <summary>
        /// /// The implementation of Fractal Brownian Motion.
        /// </summary>
        /// <param name="NoiseType">The type of noise to use from the NoiseMixer Library, Perlin, Simplex, etc. </param>
        /// <param name="xPos">The X position to return.</param>
        /// <param name="yPos">The Y position to return.</param>
        /// <param name="Octaves">The number of levels of detail you want you perlin noise to have.</param>
        /// <param name="NormalizeReturn">Should the return data values be between 0 - 1?</param>
        /// <param name="Persistence">The number that determines how much each octave contributes to the overall shape.</param>
        /// <param name="Lacunarity">The number that determines how much detail is added or removed at each octave.</param>
        /// /// <param name="InitFrequency">The starting value the frequency should to initialized at.</param>
        /// <returns>Returns a double between.</returns>
        public static double FractalBrownianMotion(IGradientNiose NoiseType, double xPos, double yPos, uint Octaves, bool NormalizeReturn, double Persistence = 0.5f, double Lacunarity = 2f, double InitFrequency = 1f)
        {
            double total = 0;
            double frequency = InitFrequency;
            double amplitude = 1;

            //Save the users settings to be restored later after modification.
            object[] savesNoiseSettings = SaveNoiseSettings(NoiseType);

            //set Normalize type for to the type needed
            NoiseType.NormalizeReturn = false;



            for (int i = 0; i < Octaves; i++)
            {

                total += NoiseType.GetValue(xPos * frequency, yPos * frequency) * amplitude;

                frequency *= Lacunarity;
                amplitude *= Persistence;

            }

            //Restore user's noise settings.
            RestoreNoiseSettings(NoiseType, savesNoiseSettings);

            total = Clamp(total, -1, 1);

            if (NormalizeReturn)
                return (total + 1) / 2 ;  // Normalize result to 0.0 - 1.0
            else
                return total; 
        }



        /// <summary>
        /// /// The implementation of BillowNoise from Fractal Brownian Motion.
        /// </summary>
        /// <param name="NoiseType">The type of noise to use from the NoiseMixer Library, Perlin, Simplex, etc. </param>
        /// <param name="xPos">The X position to return.</param>
        /// <param name="yPos">The Y position to return.</param>
        /// <param name="Seed">The seed is a starting point in generating random numbers. Changing this will change the randomness.</param>
        /// <param name="Octaves">The number of levels of detail you want you perlin noise to have.</param>
        /// <param name="NormalizeReturn">Should the return data values be between 0 - 1?</param>
        /// <param name="Persistence"></param>
        /// <param name="Lacunarity">The number that determines how much detail is added or removed at each octave.</param>
        /// /// <param name="InitFrequency">The starting value the frequency should to initialized at.</param>
        /// <returns>Returns a float.</returns>
        public static double BillowNoise(IGradientNiose NoiseType, double xPos, double yPos, uint Octaves, bool NormalizeReturn = true, double Persistence = 0.5f, double Lacunarity = 2f, double InitFrequency = 1f)
        {
            double total = 0;
            double frequency = InitFrequency;
            double amplitude = 1;

            //Save the users settings to be restored later after modification.
            object[] savesNoiseSettings = SaveNoiseSettings(NoiseType);

            //set Normalize type for to the type needed
            NoiseType.NormalizeReturn = false;

            for (int i = 0; i < Octaves; i++)
            {
                

                total += Math.Abs(NoiseType.GetValue(xPos * frequency, yPos * frequency) * amplitude);


                frequency *= Lacunarity;
                amplitude *= Persistence;

            }

            //Restore user's noise settings.
            RestoreNoiseSettings(NoiseType, savesNoiseSettings);

            total = Clamp(total, -1, 1);

            if (NormalizeReturn)
                return total; // Normalize result to 0.0 - 1.0
            else
                return (total * 2) - 1;  //result to -1.0 - 1.0
        }

        /// <summary>
        /// /// The implementation of BillowNoise from Perlin Noise.
        /// </summary>
        /// <param name="NoiseType">The type of noise to use from the NoiseMixer Library, Perlin, Simplex, etc. </param>
        /// <param name="xPos">The X position to return.</param>
        /// <param name="yPos">The Y position to return.</param>
        /// <param name="Seed">The seed is a starting point in generating random numbers. Changing this will change the randomness.</param>
        /// <param name="NormalizeReturn">Should the return data values be between 0 - 1?</param>
        /// <returns>Returns a float.</returns>
        public static float BillowNoise(IGradientNiose NoiseType, float xPos, float yPos, float Seed, bool NormalizeReturn = true)
        {

            if (NormalizeReturn)
            {
                //Save the users settings to be restored later after modification.
                object[] savesNoiseSettings = SaveNoiseSettings(NoiseType);

                //set Normalize type for to the type needed
                NoiseType.NormalizeReturn = false;

                float value = Math.Abs((float)NoiseType.GetValue(xPos, yPos)); // Normalize result to 0.0 - 1.0

                //Restore user's noise settings.
                RestoreNoiseSettings(NoiseType, savesNoiseSettings);

                return value;
            }
            else
            //return 
            {
                //Save the users settings to be restored later after modification.
                object[] savesNoiseSettings = SaveNoiseSettings(NoiseType);

                //set Normalize type for to the type needed
                NoiseType.NormalizeReturn = false;
                NoiseType.Seed = Seed;

                float value = Math.Abs(NoiseType.GetValue(xPos, yPos) * 2) - 1;  //result to -1.0 - 1.0

                //Restore user's noise settings.
                RestoreNoiseSettings(NoiseType, savesNoiseSettings);

                return value;
            }
        }


        /// <summary>
        /// The implementation of Ridged Noise Multi-fractal from Perlin Noise.
        /// </summary>
        /// <param name="NoiseType">The type of noise to use from the NoiseMixer Library, Perlin, Simplex, etc. </param>
        /// <param name="x">The X position to return.</param>
        /// <param name="y">The Y position to return.</param>
        /// <param name="Octaves">The number of levels of detail you want you perlin noise to have.</param>
        /// <param name="offset"> The offset added to the signal at each step. Higher Offset means more rough results(more ridges).</param>
        /// <param name="NormalizeReturn">Should the return data values be between 0 - 1?</param>
        /// <param name="Persistence"></param>
        /// <param name="Lacunarity">The number that determines how much detail is added or removed at each octave.</param>
        /// <param name="InitFrequency">The starting value the frequency should to initialized at.</param>
        /// <returns>Returns a float.</returns> 
        public static double RidgedNoiseMultifractal(IGradientNiose NoiseType, double xPos, double yPos, uint Octaves, double offset, bool NormalizeReturn = true, double Persistence = 0.5f, double Lacunarity = 2f, double InitFrequency = 1f)
        {
            //Take from https://thebookofshaders.com/13/

            double value = 0f;
            double amplitude = 1f;
            double frequency = InitFrequency;

            //Save the users settings to be restored later after modification.
            object[] savesNoiseSettings = SaveNoiseSettings(NoiseType);

            //set Normalize type for to the type needed
            NoiseType.NormalizeReturn = false;

            for (var i = 0; i < Octaves; i++)
            {
                //NoiseType.Seed = (float)(Seed * frequency);

                double n = Math.Abs((float)NoiseType.GetValue(xPos * frequency, yPos * frequency) * amplitude);
                n = offset - n;
                value += n * n;
                amplitude *= Persistence;
                frequency *= Lacunarity;


            }

            //Restore user's noise settings.
            RestoreNoiseSettings(NoiseType, savesNoiseSettings);

            value = Clamp(value, -1, 1);

            if (NormalizeReturn)
            { return value; } //return between (0,1)
            else
            { return value * 2 - 1; }//return between (-1,1)
        }

        /// <summary>
        /// The implementation of Ridged Noise from Fractal Brownian Motion.
        /// </summary>
        /// <param name="NoiseType">The type of noise to use from the NoiseMixer Library, Perlin, Simplex, etc. </param>
        /// <param name="xPos">The X position to return.</param>
        /// <param name="yPos">The Y position to return.</param>
        /// <param name="Seed">The seed is a starting point in generating random numbers. Changing this will change the randomness.</param>
        /// <param name="Octaves">The number of levels of detail you want you perlin noise to have.</param>
        /// <param name="NormalizeReturn">Should the return data values be between 0 - 1?</param>
        /// <param name="Persistence"></param>
        /// <param name="Lacunarity">The number that determines how much detail is added or removed at each octave.</param>
        /// /// <param name="InitFrequency">The starting value the frequency should to initialized at.</param>
        /// <returns>Returns a float.</returns>
        public static double RidgeNoise(IGradientNiose NoiseType, double xPos, double yPos, uint Octaves, bool NormalizeReturn = true, double Persistence = 0.5f, double Lacunarity = 2f, double InitFrequency = 1f)
        {
             if (NormalizeReturn)
                return 1 - BillowNoise(NoiseType, xPos, yPos, Octaves, true, Persistence, Lacunarity, InitFrequency) ;
             else
                return (1 - BillowNoise(NoiseType, xPos, yPos, Octaves, false, Persistence, Lacunarity, InitFrequency) - 1) ;



        }

        /// <summary>
        /// /// The implementation of Ridge Noise from Perlin Noise.
        /// </summary>
        /// <param name="NoiseType">The type of noise to use from the NoiseMixer Library, Perlin, Simplex, etc. </param>
        /// <param name="xPos">The X position to return.</param>
        /// <param name="yPos">The Y position to return.</param>
        /// <param name="Seed">The seed is a starting point in generating random numbers. Changing this will change the randomness.</param>
        /// <param name="NormalizeReturn">Should the return data values be between 0 - 1?</param>
        /// <returns>Returns a float.</returns>
        public static float RidgeNoise(IGradientNiose NoiseType, float xPos, float yPos, float Seed, bool NormalizeReturn = true)
        {
            if (NormalizeReturn)
                return 1 - BillowNoise(NoiseType, xPos, yPos, Seed, true);
            else
                return 1 - BillowNoise(NoiseType, xPos, yPos, Seed, false) - 1;

        }


        private static double Clamp(double value, double min, double max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        private static object[] SaveNoiseSettings(IGradientNiose NoiseType)
        {

            return new object[] {
                NoiseType.NormalizeReturn,
                //NoiseType.Seed
            };

        }

        private static void RestoreNoiseSettings(IGradientNiose NoiseType, object[] SettingsSaved)
        {
            NoiseType.NormalizeReturn = (bool)SettingsSaved[0];
           // NoiseType.Seed = (float)SettingsSaved[1];

        }

    }

    public interface INoise
    {
        /// <summary>
        /// Get value, what ever has be pre-setup. this value is from the INose
        /// </summary>
        /// <param name="XPos">The X position to use.</param>
        /// <param name="YPos">The Y position to use.</param>
        /// <returns>A float between 0, and 1</returns>
        float GetValue(float XPos, float YPos);
        double GetValue(double XPos, double YPos);

    }

    public interface IVoronoiTypeNiose : INoise
    {

    }

    public interface IGradientNiose : INoise
    {

        double Seed { set; get; }

        bool NormalizeReturn { set; get; }
    }



}





