using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TerrainMixerOutput), typeof(TextureOutput))]
public class NoiseMixerClass : MonoBehaviour
{
    //For the custom editor class
    [SerializeField]
    int ID;

    public NoiseMixer.NoiseMixer Mixer;

}



public interface INoiseMixerReturn
{

    void Return(float[,] results);

}

