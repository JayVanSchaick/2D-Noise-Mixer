using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NoiseMixer;

[CustomEditor(typeof(NoiseMixerClass))]
public class NoiseMixerClassEditor : Editor
{

    const string SAVE_FOLDER_NAME = "Noise Mixer Editor Save Files";

    List<BaseAction> actions = new List<BaseAction>();

    static GUIStyle style = new GUIStyle();

    //To Save Noise between classes
    INoise noiseSave;
    NoiseMixer.NoiseMixer.LayerBase layerSave;

    int IDSave; //For OnDestroy

    NoiseMixerEditorSaves saveFile;

    EditorApplication.CallbackFunction value;

    public void OnEnable()
    {
        

        // if prefab duplicate break link to save file.
         StopDuplicate();

        //if is prefab asset break link to save file.
        if(StopPrefabAssetSave() == false)
        {
            GetSaveFile();

            LoadData();
        }

        HotKeySetup();
    }

    public override void OnInspectorGUI()
    {

        style = new GUIStyle();

        NoiseMixerClass noiseMixer = (NoiseMixerClass)target;

        style.fontSize = 15;
        style.fontStyle = FontStyle.Bold;

        GUILayout.BeginVertical();

        style.padding = new RectOffset(10, 10, 10, 10);
        GUILayout.Label("Noise Mixer", style);


        if (actions.Count == 0)
        {
            actions.Add(new NewMixer());
        }

        for (int i = 0; i < actions.Count; i++)
        {
            actions[i].GUILayoutCall();

        }

        if (GUILayout.Button("Add New Layer (Shift + E)"))
        {
            actions.Add(new MixerLayer(actions.Count, this));
        }



        if (GUILayout.Button("Calculate Noise (Shift + Q)"))
        {

            Calculate();



        }

        GUILayout.EndVertical();

        if (PrefabUtility.IsPartOfPrefabAsset(((NoiseMixerClass)target)) == false)
        {
            SaveData();
        }


    }

    /// <summary>
    /// A layer type holder class. What is created when a player makes a new layer
    /// </summary>
    public class MixerLayer : BaseAction
    {

        public bool Disable;
        public int layerNum;
        NoiseMixerClassEditor noiseMixerClassEditor;


        public NoiseTypeLayer layer;

        //erosion layer
        public SeedType seedType;
        public int iteration = 100000;
        public int seed;
        public int ErosionRadius = 3;
        public float Inertia = 0.05f;
        public float SedimentCapacityFactor = 4;
        public float MinSedimentCapacity = 0.01f;
        public float ErodeSpeed = 0.3f;
        public float DepositSpeed = 0.3f;
        public float EvaporateSpeed = 0.01f;
        public float Gravity = 4;
        public float MaxDropletLifetime = 30;
        public float InitialWaterVolume = 1;
        public float InitialSpeed = 1;

        //for layers that need an effect amount argument 
        public float effectAmount = 0;

        //for layers that need an effect amount argument 
        public int amountOfTiers = 10;

        //for mixer shift amount
        public float shiftAmount = 0;

        //for mixer scale amount
        public float scaleAmount = 1;

        //for smooth mixer
        public int filterSize = 2;

        //a list of ever type of layer
       public enum MixerLayerTypes
        {
            CombineLayer,
            AddLayer,
            SubtractLayer,
            MultiplyLayer,
            DivideLayer,
            OnlyHigherLayer,
            OnlyLowerLayer,
            MixerTier,
            MixerShift,
            MixerScale,
            MixerInvert,
            HydraulicErosion,
            MixerSmooth
        }

       public MixerLayerTypes mixerLayer;


        public MixerLayer(int LayerNum, NoiseMixerClassEditor NoiseMixerClassEditor )
        {
            layerNum = LayerNum;

            noiseMixerClassEditor = NoiseMixerClassEditor;

            layer = new NoiseTypeLayer();
        }

        public override void Code(NoiseMixerClassEditor noiseMixerClassEditor)
        {


            if (Disable) return;

            NoiseMixerClass noiseMixerClass = (NoiseMixerClass)noiseMixerClassEditor.target;

            NoiseMixer.NoiseMixer noiseMixer = noiseMixerClass.Mixer;

            switch (mixerLayer)
            {
                case MixerLayerTypes.CombineLayer:
                    layer.Code(noiseMixerClassEditor);
                    noiseMixerClassEditor.layerSave =  noiseMixer.NewCombineLayer(noiseMixerClassEditor.noiseSave, layer.scaleNoise);
                    layer.CodeBase(noiseMixerClassEditor);
                    break;
                case MixerLayerTypes.AddLayer:
                    layer.Code(noiseMixerClassEditor);
                    noiseMixerClassEditor.layerSave = noiseMixer.NewAddLayer(noiseMixerClassEditor.noiseSave, layer.scaleNoise);
                    layer.CodeBase(noiseMixerClassEditor);
                    break;
                case MixerLayerTypes.SubtractLayer:
                    layer.Code(noiseMixerClassEditor);
                    noiseMixerClassEditor.layerSave = noiseMixer.NewSubtractLayer(noiseMixerClassEditor.noiseSave, layer.scaleNoise);
                    layer.CodeBase(noiseMixerClassEditor);
                    break;
                case MixerLayerTypes.MultiplyLayer:
                    layer.Code(noiseMixerClassEditor);
                    noiseMixerClassEditor.layerSave = noiseMixer.NewMultiplyLayer(noiseMixerClassEditor.noiseSave, layer.scaleNoise);
                    layer.CodeBase(noiseMixerClassEditor);
                    break;
                case MixerLayerTypes.DivideLayer:
                    layer.Code(noiseMixerClassEditor);
                    noiseMixerClassEditor.layerSave = noiseMixer.NewDivideLayer(noiseMixerClassEditor.noiseSave, layer.scaleNoise);
                    layer.CodeBase(noiseMixerClassEditor);
                    break;
                case MixerLayerTypes.OnlyHigherLayer:
                    layer.Code(noiseMixerClassEditor);
                    noiseMixerClassEditor.layerSave = noiseMixer.NewOnlyHigherLayer(noiseMixerClassEditor.noiseSave, layer.scaleNoise);
                    layer.CodeBase(noiseMixerClassEditor);
                    break;
                case MixerLayerTypes.OnlyLowerLayer:
                    layer.Code(noiseMixerClassEditor);
                    noiseMixerClassEditor.layerSave = noiseMixer.NewOnlyLowerLayer(noiseMixerClassEditor.noiseSave, layer.scaleNoise);
                    layer.CodeBase(noiseMixerClassEditor);
                    break;
                case MixerLayerTypes.MixerTier:
                    noiseMixer.Tier(amountOfTiers, effectAmount);
                    break;
                case MixerLayerTypes.MixerShift:
                    noiseMixer.Shift( shiftAmount);
                    break;
                case MixerLayerTypes.MixerScale:
                    noiseMixer.Scale(scaleAmount);
                    break;
                case MixerLayerTypes.MixerInvert:
                    noiseMixer.Invert();
                    break;
                case MixerLayerTypes.HydraulicErosion:
                    noiseMixer.HydraulicErosion(iteration, GetSeed(seedType, seed), ErosionRadius, Inertia, SedimentCapacityFactor, MinSedimentCapacity, ErodeSpeed, DepositSpeed, EvaporateSpeed, Gravity, MaxDropletLifetime, InitialWaterVolume, InitialSpeed);
                    break;
                case MixerLayerTypes.MixerSmooth:
                    noiseMixer.SmoothValues(filterSize, effectAmount);
                    break;
                default:
                    break;
            }

        }


        // Handles the GUI call for this class  
        public override void GUILayoutCall()
        {


            GUIStyle BoxStyle = new GUIStyle(GUI.skin.box);

            BoxStyle.padding = new RectOffset(10, 10, 10, 10);


            if (Disable)
            {
                BoxStyle.normal.background = MakeTex(10, 10, new Color(0, 0, 0, 0.5f));

                GUILayout.BeginVertical("Box", BoxStyle);
            }
            else
            {
                GUILayout.BeginVertical("Box", BoxStyle);
            }
           
            GUILayout.BeginHorizontal();

            if (Disable)
            {

                if (GUILayout.Button("Activate"))
                {
                    Disable = false;
                }
              
            }
            else if (GUILayout.Button("Disable"))
            {
                Disable = true;
            }

            if (!Disable)
            {
                if (GUILayout.Button("X"))
                {
                    noiseMixerClassEditor.actions.Remove(this);


                }

                if (GUILayout.Button("▲"))
                {

                    if ((noiseMixerClassEditor.actions.IndexOf(this)) > 1)
                    {
                        int pos = noiseMixerClassEditor.actions.IndexOf(this);

                        noiseMixerClassEditor.actions[pos] = noiseMixerClassEditor.actions[pos - 1];
                        noiseMixerClassEditor.actions[pos - 1] = this;

                    }

                }

                if (GUILayout.Button("▼"))
                {

                    if ((noiseMixerClassEditor.actions.IndexOf(this)) < noiseMixerClassEditor.actions.Count - 1)
                    {
                        int pos = noiseMixerClassEditor.actions.IndexOf(this);

                        noiseMixerClassEditor.actions[pos] = noiseMixerClassEditor.actions[pos + 1];
                        noiseMixerClassEditor.actions[pos + 1] = this;

                    }

                }
            }


            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            style.padding = new RectOffset(5, style.padding.right, 2, 3);

            style.richText = true;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 15;


            GUILayout.Label("<i>Layer " + layerNum + " :</i>", style);


            GUIStyle popupStyle = new GUIStyle(EditorStyles.popup);
            popupStyle.fontSize = 15;
            popupStyle.fontStyle = FontStyle.Bold;
            popupStyle.fixedHeight = 20;

            mixerLayer = (MixerLayerTypes)EditorGUILayout.EnumPopup(mixerLayer,  popupStyle);

            GUILayout.EndHorizontal();

            style.fontStyle = FontStyle.Normal;
            style.fontSize = 11;

            switch (mixerLayer)
            {
                case MixerLayerTypes.CombineLayer:
                case MixerLayerTypes.AddLayer:
                case MixerLayerTypes.SubtractLayer:
                case MixerLayerTypes.MultiplyLayer:
                case MixerLayerTypes.DivideLayer:
                case MixerLayerTypes.OnlyHigherLayer:
                case MixerLayerTypes.OnlyLowerLayer:
                    layer.GUILayoutCall();
                    break;
                case MixerLayerTypes.MixerTier:

                    GUILayout.BeginHorizontal();

                    GUILayout.Space(style.padding.left);
                    amountOfTiers = EditorGUILayout.IntField("Amount of Tiers", amountOfTiers);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(style.padding.left);
                    effectAmount = EditorGUILayout.Slider("Effect Amount",effectAmount, 0, 1);
                    GUILayout.EndHorizontal();

                    break;
                case MixerLayerTypes.MixerShift:

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(style.padding.left);
                    shiftAmount = EditorGUILayout.FloatField("Shift Amount", shiftAmount);
                    GUILayout.EndHorizontal();

                    break;
                case MixerLayerTypes.MixerScale:

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(style.padding.left);
                    scaleAmount = EditorGUILayout.FloatField("Scale Amount", scaleAmount);
                    GUILayout.EndHorizontal();

                    break;
                case MixerLayerTypes.MixerInvert:

                    style.fontStyle = FontStyle.Bold;
                    style.alignment = TextAnchor.MiddleCenter;
                    GUILayout.Space(style.padding.left);
                    GUILayout.Label("The mixer will invert its values", style);
                    style.alignment = TextAnchor.MiddleLeft;
                    style.fontStyle = FontStyle.Normal;

                    break;
                case MixerLayerTypes.HydraulicErosion:

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(style.padding.left);
                    iteration = EditorGUILayout.IntField("Iteration", iteration);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(style.padding.left);
                    seedType = (SeedType)EditorGUILayout.EnumPopup("Seed Type", seedType);
                    GUILayout.EndHorizontal();

                    if (seedType == SeedType.Inputed)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(style.padding.left);
                        seed = EditorGUILayout.IntField("Seed", seed);
                        GUILayout.EndHorizontal();
                    }

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(style.padding.left);
                    ErosionRadius = EditorGUILayout.IntField("Erosion Radius", ErosionRadius);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(style.padding.left);
                    Inertia = EditorGUILayout.FloatField("Inertia", Inertia);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(style.padding.left);
                    SedimentCapacityFactor = EditorGUILayout.FloatField("Sediment Capacity Factor", SedimentCapacityFactor);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(style.padding.left);
                    MinSedimentCapacity = EditorGUILayout.FloatField("Min Sediment Capacity", MinSedimentCapacity);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(style.padding.left);
                    ErodeSpeed = EditorGUILayout.FloatField("Erode Speed", ErodeSpeed);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(style.padding.left);
                    DepositSpeed = EditorGUILayout.FloatField("Deposit Speed", DepositSpeed);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(style.padding.left);
                    EvaporateSpeed = EditorGUILayout.FloatField("Evaporate Speed", EvaporateSpeed);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(style.padding.left);
                    Gravity = EditorGUILayout.FloatField("Gravity", Gravity);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(style.padding.left);
                    MaxDropletLifetime = EditorGUILayout.FloatField("MaxDropletLifetime", MaxDropletLifetime);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(style.padding.left);
                    InitialWaterVolume = EditorGUILayout.FloatField("Initial Water Volume", InitialWaterVolume);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(style.padding.left);
                    InitialSpeed = EditorGUILayout.FloatField("Initial Speed", InitialSpeed);
                    GUILayout.EndHorizontal();


                    break;
                case MixerLayerTypes.MixerSmooth:

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(style.padding.left);
                    filterSize = EditorGUILayout.IntField("Filter Size", filterSize);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(style.padding.left);
                    effectAmount = EditorGUILayout.Slider("Effect Amount", effectAmount, 0,1);
                    GUILayout.EndHorizontal();


                    break;
                default:
                    break;
            }


            GUILayout.EndVertical();

        }
    }

    //All mixer layers that have a noise property to them
    public class NoiseTypeLayer : BaseLayer
    {
        public float scaleNoise;
        public BaseAction noise;

        public AllNoiseTypes noiseType;

        public NoiseTypeLayer()
        {
            noise = GetNoiseType(noiseType);
            scaleNoise = 0.9f;
        }

        public override void Code(NoiseMixerClassEditor noiseMixerClassEditor)
        {
            noise.Code(noiseMixerClassEditor);

        }

        public void CodeBase(NoiseMixerClassEditor noiseMixerClassEditor)
        {
            base.Code(noiseMixerClassEditor);
        }


        public override void GUILayoutCall()
        {

            

            GUILayout.BeginHorizontal();
            GUILayout.Space(style.padding.left);
            scaleNoise = EditorGUILayout.FloatField("Noise Scale", scaleNoise);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(style.padding.left);
            AllNoiseTypes newNoiseType = (AllNoiseTypes)EditorGUILayout.EnumPopup("Noise Type", noiseType);
            GUILayout.EndHorizontal();

            if (newNoiseType != noiseType)
            {
                noise = GetNoiseType(newNoiseType);
                noiseType = newNoiseType;
            }

            style.padding = new RectOffset(style.padding.left + 10, style.padding.right, style.padding.top, style.padding.bottom);
            noise.GUILayoutCall();
            style.padding = new RectOffset(style.padding.left - 8, style.padding.right, style.padding.top + 5, style.padding.bottom);

            
            base.GUILayoutCall();

        }
    }

    //A base layer that all layers that have a noise property share
    public class BaseLayer : BaseAction
    {
       public bool Inverse;

        public bool shift;
        public float shiftAmount = 0;

        public bool scale;
        public float scaleAmount = 1;


        public bool hasMask;

        public float MaskFloat = 0;
        public Texture2D backgroundImage;


        public float noiseMaskScale = 0.9f;
        public BaseAction noiseMask;

        public enum MaskType
        {
            ValueMask,
            NoiseMask,
            ImageMask
        }

        public MaskType maskType;
        public  AllNoiseTypes noiseMaskType;


        public override void Code(NoiseMixerClassEditor noiseMixerClassEditor)
        {

            if (hasMask)
            {

                switch (maskType)
                {
                    case MaskType.ValueMask:
                        noiseMixerClassEditor.layerSave.LayerMask(MaskFloat);
                            break;
                    case MaskType.NoiseMask:
                        noiseMask.GUILayoutCall();
                        noiseMixerClassEditor.layerSave.LayerMask(noiseMixerClassEditor.noiseSave, noiseMaskScale);
                        break;
                    case MaskType.ImageMask:
                        if (backgroundImage)
                        {
                            double[,] greyScale = new double[backgroundImage.width, backgroundImage.height];

                            for (int x = 0; x < backgroundImage.width; x++)
                            {
                                for (int y = 0; y < backgroundImage.height; y++)
                                {
                                    try
                                    {
                                        greyScale[x, y] = backgroundImage.GetPixel(x, y).grayscale;
                                    }
                                    catch
                                    {
                                        throw new System.Exception();
                                    }
                                }
                            }
                            noiseMixerClassEditor.layerSave.LayerMask(greyScale);
                        }

                        break;
                    default:
                        break;
                }

            }

            if (shift)
            {
                noiseMixerClassEditor.layerSave.Shift(shiftAmount);
            }
            if (scale)
            {
                noiseMixerClassEditor.layerSave.Scale(scaleAmount);
            }
            if (Inverse)
            {
                noiseMixerClassEditor.layerSave.Inverse();
            }


        }

        public override void GUILayoutCall()
        {
            GUILayout.Space(style.padding.top);

            GUILayout.BeginHorizontal();
            GUILayout.Space(style.padding.left);
            hasMask = EditorGUILayout.Toggle("Has Mask", hasMask);
            GUILayout.EndVertical();

            if (hasMask)
            {
                style.padding = new RectOffset(style.padding.left + 10, style.padding.right, style.padding.top, style.padding.bottom);


                GUILayout.BeginHorizontal();
                GUILayout.Space(style.padding.left);
                MaskType newMaskType = (MaskType)EditorGUILayout.EnumPopup("Mask Type", maskType);
                GUILayout.EndHorizontal();

                if (maskType != newMaskType)
                {
                    if (newMaskType != MaskType.NoiseMask)
                    {
                        noiseMask = null;
                    }
                    else
                    {
                        noiseMask = GetNoiseType(noiseMaskType);
                    }

                }

                maskType = newMaskType;

                if (newMaskType == MaskType.NoiseMask)
                {

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(style.padding.left);
                    noiseMaskScale = EditorGUILayout.FloatField("Scale Noise", noiseMaskScale);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(style.padding.left);
                    AllNoiseTypes newNoiseType = (AllNoiseTypes)EditorGUILayout.EnumPopup("Mask Noise Type", noiseMaskType);
                    GUILayout.EndHorizontal();

                    if (newNoiseType != noiseMaskType)
                    {

                        noiseMask = GetNoiseType(newNoiseType);
                        noiseMaskType = newNoiseType;

                    }
                }


                switch (maskType)
                {
                    case MaskType.NoiseMask:
                        noiseMask.GUILayoutCall();
                        break;
                    case MaskType.ValueMask:
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(style.padding.left);
                        GUILayout.Label("Mask Amount");
                        MaskFloat = EditorGUILayout.Slider(MaskFloat, 0, 1);
                        GUILayout.EndHorizontal();
                        break;
                    case MaskType.ImageMask:
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(style.padding.left);
                        GUILayout.Label("Mask Image");
                        backgroundImage = (Texture2D)EditorGUILayout.ObjectField("Image", backgroundImage, typeof(Texture2D), false);

                        GUILayout.EndHorizontal();
                        break;
                    default:
                        break;
                }
                style.padding = new RectOffset(style.padding.left - 10, style.padding.right, style.padding.top, style.padding.bottom);
            }

            GUILayout.Space(style.padding.top);

            GUILayout.BeginHorizontal();
            GUILayout.Space(style.padding.left);
            shift = EditorGUILayout.Toggle("Shift", shift);
            GUILayout.EndVertical();

            style.padding = new RectOffset(style.padding.left + 10, style.padding.right, style.padding.top, style.padding.bottom);

            if (shift)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(style.padding.left);
                shiftAmount = EditorGUILayout.FloatField("Shift Amount", shiftAmount);
                GUILayout.EndHorizontal();
            }

            style.padding = new RectOffset(style.padding.left - 10, style.padding.right, style.padding.top, style.padding.bottom);

            GUILayout.Space(style.padding.top);

            GUILayout.BeginHorizontal();
            GUILayout.Space(style.padding.left);
            scale = EditorGUILayout.Toggle("Scale", scale);
            GUILayout.EndVertical();

            style.padding = new RectOffset(style.padding.left + 10, style.padding.right, style.padding.top, style.padding.bottom);

            if (scale)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(style.padding.left);
                scaleAmount = EditorGUILayout.FloatField("Scale Amount", scaleAmount);
                GUILayout.EndHorizontal();

            }

            style.padding = new RectOffset(style.padding.left - 10, style.padding.right, style.padding.top, style.padding.bottom);

            GUILayout.Space(style.padding.top);

            GUILayout.BeginHorizontal();
            GUILayout.Space(style.padding.left);
            Inverse = EditorGUILayout.Toggle("Inverse", Inverse);
            GUILayout.EndVertical();

        }
    }

    //All layers that are Fractal
    public class FractalType : BaseAction
    {

    public int octaves = 2;
    public float persistence =0.5f;
    public  float lacunarity = 2;
    public float initFrequency = 1;
    public  bool normalizeReturn;

    public float offset;


        public GradientNoiseType.GradientNoises gradientNoises;
        public BaseAction noise;

        public FractalNoises fractalNoises;

        public enum FractalNoises
        {
            FractalBrownianMotion,
            BillowNoise,
            RidgedNoiseMultifractal,
            RidgeNoise
        }



        public FractalType()
        {
        }

        public FractalType(FractalNoises type)
        {

            fractalNoises = type;
            noise = GetNoiseType(AllNoiseTypes.PerlinNoise);
        }

        public override void Code(NoiseMixerClassEditor noiseMixerClassEditor)
        {


            noise.Code(noiseMixerClassEditor);

            switch (fractalNoises)
            {
                case FractalNoises.FractalBrownianMotion:
                    noiseMixerClassEditor.noiseSave = new FractalBrownianMotion((IGradientNiose)noiseMixerClassEditor.noiseSave, (uint)octaves, normalizeReturn, persistence, lacunarity, initFrequency);
                    break;
                case FractalNoises.BillowNoise:
                    noiseMixerClassEditor.noiseSave = new BillowNoise((IGradientNiose)noiseMixerClassEditor.noiseSave, (uint)octaves, normalizeReturn, persistence, lacunarity, initFrequency);
                    break;
                case FractalNoises.RidgedNoiseMultifractal:
                    noiseMixerClassEditor.noiseSave = new RidgedNoiseMultifractal((IGradientNiose)noiseMixerClassEditor.noiseSave,(uint)octaves, offset, normalizeReturn, persistence, lacunarity, initFrequency);
                    break;
                case FractalNoises.RidgeNoise:
                    noiseMixerClassEditor.noiseSave = new RidgeNoise((IGradientNiose)noiseMixerClassEditor.noiseSave,(uint)octaves, normalizeReturn, persistence, lacunarity, initFrequency);
                    break;
                default:
                    break;
            }

        }

        public override void GUILayoutCall()
        {

            GUILayout.BeginHorizontal();
            GUILayout.Space(style.padding.left);
            GradientNoiseType.GradientNoises newGradientNoises = (GradientNoiseType.GradientNoises)EditorGUILayout.EnumPopup("Fractal Noise From", gradientNoises);
            GUILayout.EndHorizontal();


            if (newGradientNoises != gradientNoises)
            {
                GradientNoiseType N = (GradientNoiseType)noise;
                N.gradientNoise = newGradientNoises;
                gradientNoises = newGradientNoises;
            }

            style.padding = new RectOffset(style.padding.left + 15, style.padding.right, style.padding.top, style.padding.bottom);
            noise.GUILayoutCall();
            style.padding = new RectOffset(style.padding.left - 15, style.padding.right, style.padding.top, style.padding.bottom);


            GUILayout.BeginHorizontal();
            GUILayout.Space(style.padding.left);
            octaves = EditorGUILayout.IntField("Amount of Octaves", octaves);
            GUILayout.EndHorizontal();

            if (fractalNoises == FractalNoises.RidgedNoiseMultifractal)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(style.padding.left);
                offset = EditorGUILayout.FloatField("Offset", offset);
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(style.padding.left);
            persistence = EditorGUILayout.FloatField("Persistence", persistence);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(style.padding.left);
            lacunarity = EditorGUILayout.FloatField("Lacunarity", lacunarity);
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Space(style.padding.left);
            initFrequency = EditorGUILayout.FloatField("Initial Frequency", initFrequency);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(style.padding.left);
            normalizeReturn = EditorGUILayout.Toggle("Normalize Noise Values", normalizeReturn);
            GUILayout.EndHorizontal();

        }

    }

    //All layers that are Gradient Noise
    public class GradientNoiseType : BaseAction
    {

        public enum GradientNoises
        {
            PerlinNoise,
            SimplexNoise,
            OpenSimplexNoise,
            OpenSimplexNoise2S
        }

        public GradientNoises gradientNoise;


        public SeedType seedType;

        public OpenSimplexNoise2S.EvaluateType evaluateType;

        public int seed;
        public bool Normalize;

        public GradientNoiseType()
        {
        }

        public GradientNoiseType(GradientNoises type)
        {
            gradientNoise = type;
        }

        public override void Code(NoiseMixerClassEditor noiseMixerClassEditor)
        {

            switch (gradientNoise)
            {
                case GradientNoises.PerlinNoise:
                    noiseMixerClassEditor.noiseSave = new PerlinNoise(GetSeed(seedType, seed), Normalize);
                    break;
                case GradientNoises.SimplexNoise:
                    noiseMixerClassEditor.noiseSave = new SimplexNoise(GetSeed(seedType, seed), Normalize);

                    break;
                case GradientNoises.OpenSimplexNoise:
                    noiseMixerClassEditor.noiseSave = new OpenSimplexNoise(GetSeed(seedType, seed), Normalize);
                    break;
                case GradientNoises.OpenSimplexNoise2S:
                    noiseMixerClassEditor.noiseSave = new OpenSimplexNoise2S(evaluateType, GetSeed(seedType, seed), Normalize);
                    break;
                default:
                    break;
            }
        }


        public override void GUILayoutCall()
        {


            if (gradientNoise == GradientNoises.OpenSimplexNoise2S)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(style.padding.left);
                evaluateType = (OpenSimplexNoise2S.EvaluateType)EditorGUILayout.EnumPopup("Evaluate Type", evaluateType);
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(style.padding.left);
            seedType = (SeedType)EditorGUILayout.EnumPopup("Seed Type", seedType);
            GUILayout.EndHorizontal();

            if (seedType == SeedType.Inputed)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(style.padding.left + 5);
                seed = EditorGUILayout.IntField("Seed", seed);
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(style.padding.left);
            Normalize = EditorGUILayout.Toggle("Normalize Noise Values", Normalize);
            GUILayout.EndHorizontal();



        }
    }

    //All layers that are Voronoi type Noise
    public class VoronoiNoiseTypes : BaseAction
    {

        public SeedType seedType;
        public int seed;

        public int pointsAmount = 3;

        public int xPointsAmount = 3;
        public  int yPointsAmount = 3;

        public float distance = 10;

        public enum VoronoiType
        {
            Voronoi,
            Worley
        }

        public VoronoiType voronoiType;

        public enum PointPlacement
        {
            Random,
            ControlledRandom
        }

        public PointPlacement pointPlacement;

        public VoronoiNoiseTypes()
        {
            
        }

        public VoronoiNoiseTypes(VoronoiType VoronoiType)
        {
            this.voronoiType = VoronoiType;
        }

        public override void Code(NoiseMixerClassEditor noiseMixerClassEditor)
        {
            int x = 0; 
            int y = 0;

            if (noiseMixerClassEditor.actions[0] is NewMixer mixer)
            {
                x = mixer.XResolution;
                y = mixer.YResolution;
            }

            

            switch (voronoiType)
            {
                case VoronoiType.Voronoi:
                    if (pointPlacement == PointPlacement.Random)
                    {
                        noiseMixerClassEditor.noiseSave = new VoronoiNoise((uint)x, (uint)y, (uint)pointsAmount, GetSeed(seedType, seed));
                    }
                    else
                    {
                        noiseMixerClassEditor.noiseSave = new VoronoiNoise((uint)x, (uint)y, (uint)xPointsAmount, (uint)yPointsAmount, GetSeed(seedType, seed));
                    }
                    break;
                case VoronoiType.Worley:
                    if (pointPlacement == PointPlacement.Random)
                    {
                        noiseMixerClassEditor.noiseSave = new WorleyNoise((uint)x, (uint)y, (uint)pointsAmount, GetSeed(seedType, seed), distance);
                    }
                    else
                    {
                        noiseMixerClassEditor.noiseSave = new WorleyNoise((uint)x, (uint)y, (uint)xPointsAmount, (uint)yPointsAmount, GetSeed(seedType, seed), distance);
                    }
                    break;

            }


        }

        

        public override void GUILayoutCall()
        {

            GUILayout.BeginHorizontal();
            GUILayout.Space(style.padding.left);
            seedType = (SeedType)EditorGUILayout.EnumPopup("Seed Type", seedType);
            GUILayout.EndHorizontal();

            if (seedType == SeedType.Inputed)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(style.padding.left + 5);
                seed = EditorGUILayout.IntField("Seed", seed);
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(style.padding.left);
            pointPlacement = (PointPlacement)EditorGUILayout.EnumPopup("Point Placement Method", pointPlacement);
            GUILayout.EndHorizontal();

            if (pointPlacement == PointPlacement.Random)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(style.padding.left + 5);
                pointsAmount = EditorGUILayout.IntField("Amount of Points", pointsAmount);
                GUILayout.EndHorizontal();

            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(style.padding.left + 5);
                xPointsAmount = EditorGUILayout.IntField("Amount of Row Points", xPointsAmount);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(style.padding.left + 5);
                yPointsAmount = EditorGUILayout.IntField("Amount of Column Points", yPointsAmount);
                GUILayout.EndHorizontal();

            }

            if (voronoiType == VoronoiType.Worley)
            {

                GUILayout.BeginHorizontal();
                GUILayout.Space(style.padding.left + 5);
                distance = EditorGUILayout.FloatField("Distance from Point", distance);
                GUILayout.EndHorizontal();
            }


        }
    }

    public class NewMixer : BaseAction
    {

        int xResolution = 512;

        int yResolution = 512;

        float fillValue = 0;

        public int XResolution { get => xResolution; set => xResolution = value; }
        public int YResolution { get => yResolution; set => yResolution = value; }
        public float FillValue { get => fillValue; set => fillValue = value; }

        public override void Code(NoiseMixerClassEditor noiseMixerClassEditor)
        {
            NoiseMixerClass noiseMixer = (NoiseMixerClass)noiseMixerClassEditor.target;

            noiseMixer.Mixer = new NoiseMixer.NoiseMixer((uint)xResolution, (uint)yResolution, FillValue);

        }

        public override void GUILayoutCall()
        {
            GUIStyle BoxStyle = new GUIStyle(GUI.skin.box);

            BoxStyle.padding = style.padding;

            GUILayout.BeginVertical( BoxStyle);

            style.padding = new RectOffset(5, style.padding.right, 2, 3);

            style.richText = true;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 15;

            GUILayout.Label("Noise Mixer Settings", style);

            GUILayout.BeginHorizontal();
            GUILayout.Space(style.padding.left);
            xResolution = EditorGUILayout.IntField("The Width of the noise", xResolution);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(style.padding.left);
            yResolution = EditorGUILayout.IntField("The Height of the noise", yResolution);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(style.padding.left);
            FillValue = EditorGUILayout.FloatField("Base Layer Starting Value", FillValue);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
    }

    public abstract class BaseAction
    {

       public abstract void GUILayoutCall();

        public abstract void Code(NoiseMixerClassEditor noiseMixerClassEditor);
    }

    public enum AllNoiseTypes
    {
        FractalBrownianMotion,
        BillowNoise,
        RidgedNoiseMultifractal,
        RidgeNoise,
        PerlinNoise,
        SimplexNoise,
        OpenSimplexNoise,
        OpenSimplexNoise2S,
        Voronoi,
        Worley
    }

    public enum SeedType
    {
        Random,
        Inputed
    }

    static BaseAction GetNoiseType(AllNoiseTypes noiseType)
    {
        switch (noiseType)
        {
            case AllNoiseTypes.FractalBrownianMotion:
                return new FractalType(FractalType.FractalNoises.FractalBrownianMotion);
            case AllNoiseTypes.BillowNoise:
                return new FractalType(FractalType.FractalNoises.BillowNoise);
            case AllNoiseTypes.RidgedNoiseMultifractal:
                return new FractalType(FractalType.FractalNoises.RidgedNoiseMultifractal);
            case AllNoiseTypes.RidgeNoise:
                return new FractalType(FractalType.FractalNoises.RidgeNoise);
            case AllNoiseTypes.PerlinNoise:
                return new GradientNoiseType(GradientNoiseType.GradientNoises.PerlinNoise);
            case AllNoiseTypes.SimplexNoise:
                return new GradientNoiseType(GradientNoiseType.GradientNoises.SimplexNoise);
            case AllNoiseTypes.OpenSimplexNoise:
                return new GradientNoiseType(GradientNoiseType.GradientNoises.OpenSimplexNoise);
            case AllNoiseTypes.OpenSimplexNoise2S:
                return new GradientNoiseType(GradientNoiseType.GradientNoises.OpenSimplexNoise2S);
            case AllNoiseTypes.Voronoi:
                return new VoronoiNoiseTypes(VoronoiNoiseTypes.VoronoiType.Voronoi);
            case AllNoiseTypes.Worley:
                return new VoronoiNoiseTypes(VoronoiNoiseTypes.VoronoiType.Worley);
            default:
                return new GradientNoiseType(GradientNoiseType.GradientNoises.PerlinNoise);
        }




    }

    static int GetSeed(SeedType type, int val)
    {
        switch (type)
        {
            case SeedType.Random:

                return new System.Random().Next(int.MaxValue);
            case SeedType.Inputed:
                return val;
            default:
                return new System.Random().Next();
        }

    }

    //Get or make a save file
    void GetSaveFile()
    {

        SerializedObject SO = new SerializedObject(target);


        SerializedProperty property = SO.FindProperty("ID");
        if (property.intValue == 0)
        {
            property.intValue = (int)System.DateTime.Now.Ticks;
        }

        SO.ApplyModifiedProperties();

        IDSave = property.intValue;

        MonoScript ms = MonoScript.FromScriptableObject(this);
        string ScriptFilePath = AssetDatabase.GetAssetPath(ms);


        string FilePath = ScriptFilePath.TrimEnd((this.ToString() + ".cs").ToCharArray());

        if (AssetDatabase.IsValidFolder(FilePath + SAVE_FOLDER_NAME) == false)
        {
            AssetDatabase.CreateFolder(FilePath.TrimEnd('/'), SAVE_FOLDER_NAME);

            saveFile = ScriptableObject.CreateInstance<NoiseMixerEditorSaves>();

            AssetDatabase.CreateAsset(saveFile, FilePath + SAVE_FOLDER_NAME + "/" + property.intValue + ".asset");
            AssetDatabase.SaveAssets();
        }
        else if (AssetDatabase.FindAssets(property.intValue.ToString()).Length == 0)
        {
            saveFile = ScriptableObject.CreateInstance<NoiseMixerEditorSaves>();

            AssetDatabase.CreateAsset(saveFile, FilePath + SAVE_FOLDER_NAME + "/" + property.intValue + ".asset");
            AssetDatabase.SaveAssets();

        }
        else
        {

            saveFile = (NoiseMixerEditorSaves)AssetDatabase.LoadAssetAtPath(FilePath + SAVE_FOLDER_NAME + "/" + property.intValue + ".asset", typeof(NoiseMixerEditorSaves));

        }



    }

    void SaveData()
    {


        NewMixer mixer = (NewMixer)actions[0];

        saveFile.Height = mixer.YResolution;
        saveFile.Width = mixer.XResolution;

        saveFile.StartingValue = mixer.FillValue;

        saveFile.Layers = new NoiseMixerEditorSaves.LayerData[actions.Count - 1];



        for (int i = 1; i < actions.Count; i++)
        {
            MixerLayer layer = (MixerLayer)actions[i];

            saveFile.Layers[i - 1] = new NoiseMixerEditorSaves.LayerData
            {
                Disable = layer.Disable,
                layerNum = layer.layerNum,

                typeLayer = (int)layer.mixerLayer,
                seedType = (int)layer.seedType,
                iteration = layer.iteration,
                seed = layer.seed,
                ErosionRadius = layer.ErosionRadius,
                Inertia = layer.Inertia,
                SedimentCapacityFactor = layer.SedimentCapacityFactor,
                MinSedimentCapacity = layer.MinSedimentCapacity,
                ErodeSpeed = layer.ErodeSpeed,
                DepositSpeed = layer.DepositSpeed,
                EvaporateSpeed = layer.EvaporateSpeed,
                Gravity = layer.Gravity,
                MaxDropletLifetime = layer.MaxDropletLifetime,
                InitialWaterVolume = layer.InitialWaterVolume,
                InitialSpeed = layer.InitialSpeed,
                effectAmount = layer.effectAmount,
                amountOfTiers = layer.amountOfTiers,
                shiftAmount = layer.shiftAmount,
                scaleAmount = layer.scaleAmount,
                filterSize = layer.filterSize,

                mainNoise = new NoiseMixerEditorSaves.NoiseType(),
            };




            saveFile.Layers[i - 1].mainNoise.NoiseScale = layer.layer.scaleNoise;
            saveFile.Layers[i - 1].mainNoise.noiseType = layer.layer.noiseType;


            SaveNoiseType(layer.layer.noiseType, layer.layer.noise, i - 1, true);

            saveFile.Layers[i - 1].mainNoise.Inverse = layer.layer.Inverse;
            saveFile.Layers[i - 1].mainNoise.shift = layer.layer.shift;
            saveFile.Layers[i - 1].mainNoise.shiftAmount = layer.layer.shiftAmount;
            saveFile.Layers[i - 1].mainNoise.scale = layer.layer.scale;
            saveFile.Layers[i - 1].mainNoise.scaleAmount = layer.layer.scaleAmount;

            saveFile.Layers[i - 1].mainNoise.hasMask = layer.layer.hasMask;
            saveFile.Layers[i - 1].mainNoise.maskType = layer.layer.maskType;

            saveFile.Layers[i - 1].mainNoise.MaskFloat = layer.layer.MaskFloat;
            saveFile.Layers[i - 1].mainNoise.backgroundImage = AssetDatabase.GetAssetPath(layer.layer.backgroundImage);
            saveFile.Layers[i - 1].mainNoise.noiseMaskScale = layer.layer.noiseMaskScale;
            saveFile.Layers[i - 1].mainNoise.noiseMaskType = layer.layer.noiseMaskType;

            if (layer.layer.noiseMask != null)
                SaveNoiseType(layer.layer.noiseMaskType, layer.layer.noiseMask, i - 1, false);

        }

        EditorUtility.SetDirty(saveFile);
    }

    void SaveNoiseType(AllNoiseTypes noiseType, BaseAction noise, int LayerNum, bool MainNoise)
    {


        if (noiseType == AllNoiseTypes.FractalBrownianMotion || noiseType == AllNoiseTypes.BillowNoise ||
            noiseType == AllNoiseTypes.RidgedNoiseMultifractal || noiseType == AllNoiseTypes.RidgeNoise)
        {

            NoiseMixerEditorSaves.FractionalType noiseSave = new NoiseMixerEditorSaves.FractionalType();

            if (MainNoise)
                saveFile.Layers[LayerNum].mainNoise.fractionalType = noiseSave;
            else
                saveFile.Layers[LayerNum].mainNoise.fractionalTypeMask = noiseSave;


            FractalType fractalType = (FractalType)noise;
            noiseSave.fractionalNoiseType = (int)fractalType.fractalNoises;
            noiseSave.octaves = fractalType.octaves;
            noiseSave.persistence = fractalType.persistence;
            noiseSave.lacunarity = fractalType.lacunarity;
            noiseSave.initFrequency = fractalType.initFrequency;
            noiseSave.normalizeReturn = fractalType.normalizeReturn;
            noiseSave.offset = fractalType.offset;


            NoiseMixerEditorSaves.GradientType baseNoiseSave = new NoiseMixerEditorSaves.GradientType();


            if (MainNoise)
                saveFile.Layers[LayerNum].mainNoise.gradientType = baseNoiseSave;
            else
                saveFile.Layers[LayerNum].mainNoise.gradientTypeMask = baseNoiseSave;

            GradientNoiseType gradientNoiseType = (GradientNoiseType)fractalType.noise;

            baseNoiseSave.gradientNoiseType = (int)gradientNoiseType.gradientNoise;
            baseNoiseSave.seedType = (int)gradientNoiseType.seedType;
            baseNoiseSave.seed = gradientNoiseType.seed;
            baseNoiseSave.evaluateType = gradientNoiseType.evaluateType;
            baseNoiseSave.Normalized = gradientNoiseType.Normalize;
        }

        if (noiseType == AllNoiseTypes.PerlinNoise || noiseType == AllNoiseTypes.SimplexNoise ||
           noiseType == AllNoiseTypes.OpenSimplexNoise || noiseType == AllNoiseTypes.OpenSimplexNoise2S)
        {

            NoiseMixerEditorSaves.GradientType noiseSave = new NoiseMixerEditorSaves.GradientType();


            if (MainNoise)
                saveFile.Layers[LayerNum].mainNoise.gradientType = noiseSave;
            else
                saveFile.Layers[LayerNum].mainNoise.gradientTypeMask = noiseSave;

            GradientNoiseType gradientNoiseType = (GradientNoiseType)noise;

            noiseSave.gradientNoiseType = (int)gradientNoiseType.gradientNoise;
            noiseSave.seedType = (int)gradientNoiseType.seedType;
            noiseSave.seed = gradientNoiseType.seed;
            noiseSave.evaluateType = gradientNoiseType.evaluateType;
            noiseSave.Normalized = gradientNoiseType.Normalize;
        }


        if (noiseType == AllNoiseTypes.Voronoi || noiseType == AllNoiseTypes.Worley)
        {

            NoiseMixerEditorSaves.VoronoiType noiseSave = new NoiseMixerEditorSaves.VoronoiType();

            if (MainNoise)
                saveFile.Layers[LayerNum].mainNoise.voronoiType = noiseSave;
            else
                saveFile.Layers[LayerNum].mainNoise.voronoiTypeMask = noiseSave;


            VoronoiNoiseTypes voronoiNoiseType = (VoronoiNoiseTypes)noise;

            noiseSave.VoronoiNoiseType = (int)voronoiNoiseType.voronoiType;
            noiseSave.pointsAmount = voronoiNoiseType.pointsAmount;
            noiseSave.xPointsAmount = voronoiNoiseType.xPointsAmount;
            noiseSave.yPointsAmount = voronoiNoiseType.yPointsAmount;
            noiseSave.SeedType = voronoiNoiseType.seedType;
            noiseSave.seed = voronoiNoiseType.seed;
            noiseSave.pointPlacement = voronoiNoiseType.pointPlacement;
            noiseSave.distance = voronoiNoiseType.distance;
        }

    }

    void LoadNoiseType(AllNoiseTypes noiseType, BaseAction noise, int LayerNum, bool MainNoise)
    {


        if ((noiseType == AllNoiseTypes.FractalBrownianMotion || noiseType == AllNoiseTypes.BillowNoise ||
            noiseType == AllNoiseTypes.RidgedNoiseMultifractal || noiseType == AllNoiseTypes.RidgeNoise))
        {

            NoiseMixerEditorSaves.FractionalType noiseSave;

            if (MainNoise)
                noiseSave = saveFile.Layers[LayerNum].mainNoise.fractionalType;
            else
                noiseSave = saveFile.Layers[LayerNum].mainNoise.fractionalTypeMask;

            if (noiseSave == null) return;

            FractalType fractalType = (FractalType)noise;
            fractalType.fractalNoises = (FractalType.FractalNoises)noiseSave.fractionalNoiseType;
            fractalType.octaves = noiseSave.octaves;
            fractalType.persistence = noiseSave.persistence;
            fractalType.lacunarity = noiseSave.lacunarity;
            fractalType.initFrequency = noiseSave.initFrequency;
            fractalType.normalizeReturn = noiseSave.normalizeReturn;
            fractalType.offset = noiseSave.offset;


            NoiseMixerEditorSaves.GradientType baseNoiseSave;

            if (MainNoise)
                baseNoiseSave = saveFile.Layers[LayerNum].mainNoise.gradientType;
            else
                baseNoiseSave = saveFile.Layers[LayerNum].mainNoise.gradientTypeMask;

            fractalType.gradientNoises = (GradientNoiseType.GradientNoises)baseNoiseSave.gradientNoiseType;

            GradientNoiseType gradientNoiseType = (GradientNoiseType)fractalType.noise;

            gradientNoiseType.gradientNoise = (GradientNoiseType.GradientNoises)baseNoiseSave.gradientNoiseType;
            gradientNoiseType.seedType = (SeedType)baseNoiseSave.seedType;
            gradientNoiseType.seed = baseNoiseSave.seed;
            gradientNoiseType.evaluateType = baseNoiseSave.evaluateType;
            gradientNoiseType.Normalize = baseNoiseSave.Normalized;
        }

        if (noiseType == AllNoiseTypes.PerlinNoise || noiseType == AllNoiseTypes.SimplexNoise ||
           noiseType == AllNoiseTypes.OpenSimplexNoise || noiseType == AllNoiseTypes.OpenSimplexNoise2S)
        {

            NoiseMixerEditorSaves.GradientType noiseSave;

            if (MainNoise)
                noiseSave = saveFile.Layers[LayerNum].mainNoise.gradientType;
            else
                noiseSave = saveFile.Layers[LayerNum].mainNoise.gradientTypeMask;

            if (noiseSave == null) return;

            GradientNoiseType gradientNoiseType = (GradientNoiseType)noise;

            gradientNoiseType.gradientNoise = (GradientNoiseType.GradientNoises)noiseSave.gradientNoiseType;
            gradientNoiseType.seedType = (SeedType)noiseSave.seedType;
            gradientNoiseType.seed = noiseSave.seed;
            gradientNoiseType.evaluateType = noiseSave.evaluateType;
            gradientNoiseType.Normalize = noiseSave.Normalized;

        }


        if (noiseType == AllNoiseTypes.Voronoi || noiseType == AllNoiseTypes.Worley)
        {

            NoiseMixerEditorSaves.VoronoiType noiseSave;

            if (MainNoise)
                noiseSave = saveFile.Layers[LayerNum].mainNoise.voronoiType;
            else
                noiseSave = saveFile.Layers[LayerNum].mainNoise.voronoiTypeMask;

            if (noiseSave == null) return;

            VoronoiNoiseTypes voronoiNoiseType = (VoronoiNoiseTypes)noise;

            voronoiNoiseType.voronoiType = (VoronoiNoiseTypes.VoronoiType)noiseSave.VoronoiNoiseType;
            voronoiNoiseType.pointsAmount = noiseSave.pointsAmount;
            voronoiNoiseType.xPointsAmount = noiseSave.xPointsAmount;
            voronoiNoiseType.yPointsAmount = noiseSave.yPointsAmount;
            voronoiNoiseType.seedType = noiseSave.SeedType;
            voronoiNoiseType.seed = noiseSave.seed;
            voronoiNoiseType.pointPlacement = noiseSave.pointPlacement;
            voronoiNoiseType.distance = noiseSave.distance;
        }

    }

    void LoadData()
    {

        NewMixer mixer = new NewMixer();

        actions.Add(mixer);

        mixer.YResolution = saveFile.Height;
        mixer.XResolution = saveFile.Width;
        mixer.FillValue = saveFile.StartingValue;


        if (saveFile.Layers != null)
            for (int i = 1; i <= saveFile.Layers.Length; i++)
            {

                MixerLayer layer = new MixerLayer(i, this);

                actions.Add(layer);

                layer.Disable = saveFile.Layers[i - 1].Disable;
                layer.layerNum = saveFile.Layers[i - 1].layerNum;

                layer.mixerLayer = (MixerLayer.MixerLayerTypes)saveFile.Layers[i - 1].typeLayer;

                layer.seedType = (SeedType)saveFile.Layers[i - 1].seedType;
                layer.iteration = saveFile.Layers[i - 1].iteration;
                layer.seed = saveFile.Layers[i - 1].seed;
                layer.ErosionRadius = saveFile.Layers[i - 1].ErosionRadius;
                layer.Inertia = saveFile.Layers[i - 1].Inertia;
                layer.SedimentCapacityFactor = saveFile.Layers[i - 1].SedimentCapacityFactor;
                layer.MinSedimentCapacity = saveFile.Layers[i - 1].MinSedimentCapacity;
                layer.ErodeSpeed = saveFile.Layers[i - 1].ErodeSpeed;
                layer.DepositSpeed = saveFile.Layers[i - 1].DepositSpeed;
                layer.EvaporateSpeed = saveFile.Layers[i - 1].EvaporateSpeed;
                layer.Gravity = saveFile.Layers[i - 1].Gravity;
                layer.MaxDropletLifetime = saveFile.Layers[i - 1].MaxDropletLifetime;
                layer.InitialWaterVolume = saveFile.Layers[i - 1].InitialWaterVolume;
                layer.InitialSpeed = saveFile.Layers[i - 1].InitialSpeed;
                layer.effectAmount = saveFile.Layers[i - 1].effectAmount;
                layer.amountOfTiers = saveFile.Layers[i - 1].amountOfTiers;
                layer.shiftAmount = saveFile.Layers[i - 1].shiftAmount;
                layer.scaleAmount = saveFile.Layers[i - 1].scaleAmount;
                layer.filterSize = saveFile.Layers[i - 1].filterSize;

                if (saveFile.Layers[i - 1].mainNoise != null)
                {

                    layer.layer.scaleNoise = saveFile.Layers[i - 1].mainNoise.NoiseScale;
                    layer.layer.noiseType = saveFile.Layers[i - 1].mainNoise.noiseType;
                    layer.layer.noise = GetNoiseType(saveFile.Layers[i - 1].mainNoise.noiseType);

                    LoadNoiseType(saveFile.Layers[i - 1].mainNoise.noiseType, layer.layer.noise, i - 1, true);

                    layer.layer.Inverse = saveFile.Layers[i - 1].mainNoise.Inverse;
                    layer.layer.shift = saveFile.Layers[i - 1].mainNoise.shift;
                    layer.layer.shiftAmount = saveFile.Layers[i - 1].mainNoise.shiftAmount;
                    layer.layer.scale = saveFile.Layers[i - 1].mainNoise.scale;
                    layer.layer.scaleAmount = saveFile.Layers[i - 1].mainNoise.scaleAmount;

                    layer.layer.hasMask = saveFile.Layers[i - 1].mainNoise.hasMask;
                    layer.layer.maskType = saveFile.Layers[i - 1].mainNoise.maskType;

                    layer.layer.MaskFloat = saveFile.Layers[i - 1].mainNoise.MaskFloat;
                    layer.layer.backgroundImage = (Texture2D)AssetDatabase.LoadAssetAtPath(saveFile.Layers[i - 1].mainNoise.backgroundImage, typeof(Texture2D));

                    layer.layer.noiseMask = GetNoiseType(saveFile.Layers[i - 1].mainNoise.noiseMaskType);

                    if (layer.layer.noiseMask != null)
                        LoadNoiseType(saveFile.Layers[i - 1].mainNoise.noiseMaskType, layer.layer.noiseMask, i - 1, false);

                    layer.layer.noiseMaskScale = saveFile.Layers[i - 1].mainNoise.noiseMaskScale;
                    layer.layer.noiseMaskType = saveFile.Layers[i - 1].mainNoise.noiseMaskType;

                }


            }

    }

    void HotKeySetup()
    {
        System.Reflection.FieldInfo info = typeof(EditorApplication).GetField("globalEventHandler", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

        value += (EditorApplication.CallbackFunction)info.GetValue(null);

        value += EditorKeyPress;

        info.SetValue(null, value);

    }

    void EditorKeyPress()
    {
       
        if (this != null)
        {
            
            Event e = Event.current;

            if (e != null && e.type == EventType.KeyDown && e.keyCode == KeyCode.Q && e.shift)
            {

                Calculate();

            }

            if (e != null && e.type == EventType.KeyDown && e.keyCode == KeyCode.E && e.shift)
            {

                actions.Add(new MixerLayer(actions.Count, this));

            }



        }

    }

    bool StopPrefabAssetSave()
    {

        if (PrefabUtility.IsPartOfPrefabAsset(((NoiseMixerClass)target)))
        {
            SerializedObject SO = new SerializedObject(target);

            SerializedProperty property = SO.FindProperty("ID");
            property.intValue = 0;

            SO.ApplyModifiedProperties();

            return true;

        }

        return false;
    }

    void StopDuplicate()
    {

        if (PrefabUtility.IsPartOfPrefabInstance((NoiseMixerClass)target))
        {
            SerializedObject SO = new SerializedObject(target);

            SerializedProperty property = SO.FindProperty("ID");

            NoiseMixerClass[] noiseMixerClasses = GameObject.FindObjectsOfType<NoiseMixerClass>();

            for (int i = 0; i < noiseMixerClasses.Length; i++)
            {

                if (noiseMixerClasses[i] != (NoiseMixerClass)target)
                {

                    SerializedObject otherScript = new SerializedObject(noiseMixerClasses[i]);

                    SerializedProperty otherScriptID = otherScript.FindProperty("ID");

                    if (otherScriptID.intValue == property.intValue)
                    {
                        property.intValue = 0;
                        SO.ApplyModifiedProperties();
                    }

                }

            }

        }

    }

    void Calculate()
    {
        for (int i = 0; i < actions.Count; i++)
        {
            actions[i].Code(this);
        }

        float[,] noiseReturn = ((NoiseMixerClass)target).Mixer.ApplyF(true);

        foreach (INoiseMixerReturn output in ((MonoBehaviour)this.target).gameObject.GetComponents<INoiseMixerReturn>())
        {
            output.Return(noiseReturn);

        }



    }

    private static Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    private void OnDestroy()
    {

        if (((NoiseMixerClass)target) == null)
        {
            MonoScript ms = MonoScript.FromScriptableObject(this);
            string ScriptFilePath = AssetDatabase.GetAssetPath(ms);


            string FilePath = ScriptFilePath.TrimEnd((this.ToString() + ".cs").ToCharArray());

            saveFile = (NoiseMixerEditorSaves)AssetDatabase.LoadAssetAtPath(FilePath + SAVE_FOLDER_NAME + "/" + IDSave + ".asset", typeof(NoiseMixerEditorSaves));

            AssetDatabase.DeleteAsset(FilePath + SAVE_FOLDER_NAME + "/" +  IDSave + ".asset");


        }

    }

}
