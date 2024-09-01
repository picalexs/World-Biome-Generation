using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGenerator
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistence, float lacunarity, Vector2 offset, float contrast, float fudgeFactor)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        float maxPossibleHeight = 0;
        float amplitude = 1;

        // Calculate max possible height
        for (int i = 0; i < octaves; i++)
        {
            maxPossibleHeight += amplitude;
            amplitude *= persistence;
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = ((x - halfWidth) / scale + octaveOffsets[i].x) * frequency;
                    float sampleY = ((y - halfHeight) / scale + octaveOffsets[i].y) * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                noiseMap[x, y] = noiseHeight / maxPossibleHeight;
                noiseMap[x, y] = Mathf.Pow(noiseMap[x, y] * fudgeFactor, contrast);
                noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}
