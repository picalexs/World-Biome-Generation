using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public static class TextureGenerator
{
    public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }

    public static Texture2D TextureFromMap(float[,] map)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);

        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, map[x, y]);
            }
        }
        return TextureFromColorMap(colorMap, width, height);
    }

    public static Texture2D TextureFromCombinedMaps(float[,] heightMap, float[,] moistureMap, TerrainType[] regions)
    {
        if (regions.Length < 1)
        {
            Debug.LogError("Regions array is empty");
            return null;
        }

        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int i = regions.Length - 1; i >= 0; i--)
                {
                    if (heightMap[x, y] >= regions[i].height && moistureMap[x, y] >= regions[i].moisture)
                    {
                        colorMap[y * width + x] = regions[i].color;
                        break;
                    }
                }
            }
        }
        return TextureFromColorMap(colorMap, width, height);
    }
}
