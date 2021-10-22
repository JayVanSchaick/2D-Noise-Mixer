using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureOutput : MonoBehaviour, INoiseMixerReturn
{
    public Renderer GameObjectToRender;


    public void Return(float[,] results)
    {
        if (GameObjectToRender)
        {
            Texture2D texture2D = new Texture2D(results.GetLength(0), results.GetLength(1), TextureFormat.ARGB32, false);

            for (int i = 0; i < results.GetLength(0); i++)
            {
                for (int j = 0; j < results.GetLength(1); j++)
                {
                    texture2D.SetPixel(i, j, new Color(results[i, j], results[i, j], results[i, j]));
                }
            }


            texture2D.Apply();
            GameObjectToRender.sharedMaterial.mainTexture = texture2D;
            //GameObjectToRender.material.mainTexture = texture2D;
        }

    }

}

