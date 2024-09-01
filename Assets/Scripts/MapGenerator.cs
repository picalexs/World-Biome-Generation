using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private enum DrawMode { NoiseMap, ColorMap };
    [SerializeField] private DrawMode drawMode;

    [Min(1), SerializeField] private int mapWidth;
    [Min(1), SerializeField] private int mapHeight;
    [SerializeField] private float noiseScale;
    [Range(0, 10), SerializeField] private int octaves;
    [Range(0, 1), SerializeField] private float persistance;
    [Range(1, 10), SerializeField] private float lacunarity;

    [SerializeField] private int seed;
    [SerializeField] private Vector2 offset;
    [Range(0.1f, 5), SerializeField] private float contrast;
    [Range(0.1f, 3), SerializeField] private float fudgeFactor;

    public bool autoUpdate;

    public TerrainType[] regions;

    public void GenerateMap()
    {
        float[,] noiseMap = NoiseGenerator.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset, contrast, fudgeFactor);

        Color[] colorMap = new Color[mapWidth * mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colorMap[y * mapWidth + x] = regions[i].color;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
        }
    }
    public void GenerateRandomMapSeed()
    {
        seed = Random.Range(-100000, 100000);
        GenerateMap();
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    [Range(0, 1)] public float height;
    public Color color;
}
