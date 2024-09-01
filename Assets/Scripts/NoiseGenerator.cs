using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class NoiseGenerator
{
    public static float[,] GenerateNoiseMap(MapProprieties mapProprieties, int mapWidth, int mapHeight)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(mapProprieties.seed);
        Vector2[] octaveOffsets = new Vector2[mapProprieties.octaves];

        for (int i = 0; i < mapProprieties.octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + mapProprieties.offset.x;
            float offsetY = prng.Next(-100000, 100000) + mapProprieties.offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        float maxPossibleHeight = 0;
        float amplitude = 1;

        for (int i = 0; i < mapProprieties.octaves; i++)
        {
            maxPossibleHeight += amplitude;
            amplitude *= mapProprieties.persistance;
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < mapProprieties.octaves; i++)
                {
                    float sampleX = ((x - halfWidth) / mapProprieties.noiseScale + octaveOffsets[i].x) * frequency;
                    float sampleY = ((y - halfHeight) / mapProprieties.noiseScale + octaveOffsets[i].y) * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= mapProprieties.persistance;
                    frequency *= mapProprieties.lacunarity;
                }

                noiseMap[x, y] = noiseHeight / maxPossibleHeight;
                noiseMap[x, y] = Mathf.Pow(noiseMap[x, y] * mapProprieties.fudgeFactor, mapProprieties.contrast);
                noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y]);
            }
        }
        return noiseMap;
    }
}
