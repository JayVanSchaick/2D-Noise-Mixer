using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NoiseMixerClassEditor.BaseLayer;

public class NoiseMixerEditorSaves : ScriptableObject
{

    public int Height = 512;
    public int Width = 512;

    public float StartingValue;

    public LayerData[] Layers;


    [System.Serializable]
    public class LayerData
    {
        public bool Disable;
        public int layerNum;

        public int typeLayer;

        //erosion layer
        public  int seedType;
        public int iteration;
        public int seed;
        public int ErosionRadius;
        public float Inertia;
        public float SedimentCapacityFactor;
        public float MinSedimentCapacity;
        public float ErodeSpeed;
        public float DepositSpeed;
        public float EvaporateSpeed;
        public float Gravity;
        public float MaxDropletLifetime;
        public float InitialWaterVolume;
        public float InitialSpeed;

        //for layers that need an effect amount argument 
        public float effectAmount;

        //for layers that need an effect amount argument 
        public int amountOfTiers;

        //for mixer shift amount
        public float shiftAmount;

        //for mixer scale amount
        public float scaleAmount;

        //for smooth mixer
        public int filterSize;


        public NoiseType mainNoise;

    }

    [System.Serializable]
    public class NoiseType
    {

        public NoiseMixerClassEditor.AllNoiseTypes noiseType;

        public float NoiseScale;


        public bool Inverse;

        public bool shift;
        public float shiftAmount;

        public bool scale;
        public float scaleAmount;


        public bool hasMask;

        public float MaskFloat;
        public string backgroundImage;


        public float noiseMaskScale;

        public MaskType maskType;
        public NoiseMixerClassEditor.AllNoiseTypes noiseMaskType;

        public GradientType gradientType;

        public FractionalType fractionalType;

        public VoronoiType voronoiType;


        public  GradientType gradientTypeMask;

        public FractionalType fractionalTypeMask;

        public VoronoiType voronoiTypeMask;

    }

    [System.Serializable]
    public class GradientType
    {

        public int gradientNoiseType;
        public int seedType;
        public int seed;
        public bool Normalized;

        public NoiseMixer.OpenSimplexNoise2S.EvaluateType evaluateType;

    }

    [System.Serializable]
    public class FractionalType
    {
        //FractionalTypes
        public int fractionalNoiseType;
        public int octaves;
        public float persistence;
        public float lacunarity;
        public float initFrequency;
        public bool normalizeReturn;

        public float offset;

        public NoiseMixerClassEditor.GradientNoiseType.GradientNoises gradientNoise;

    }

    [System.Serializable]
    public class VoronoiType
    {
        public int VoronoiNoiseType;

        public NoiseMixerClassEditor.SeedType SeedType;
        public int seed;

        public int pointsAmount;

        public int xPointsAmount;
        public int yPointsAmount;

        public float distance;

        public NoiseMixerClassEditor.VoronoiNoiseTypes.PointPlacement pointPlacement;

    }

}
