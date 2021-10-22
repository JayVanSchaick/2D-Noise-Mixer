using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMixerOutput : MonoBehaviour, INoiseMixerReturn
{

    public Terrain Terrain;

    public int TerrainSize = 500;

    public bool TerrainMapBestResolution = true;

    public void Return(float[,] results)
    {
        if (!Terrain)
        {
            return;
        }

        Terrain.terrainData.heightmapResolution = results.GetLength(0) ;
        Terrain.terrainData.SetHeights(0, 0, results);
        Terrain.terrainData.size = new Vector3(TerrainSize, 256, TerrainSize);

        if (TerrainMapBestResolution)
        {
            Terrain.heightmapPixelError = 1;
        }
       

    }
}
