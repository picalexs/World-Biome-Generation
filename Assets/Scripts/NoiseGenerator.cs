using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGenerator
{
    public static float[,] GenerateNoiseMap(MapProprieties mapProprieties, int mapWidth, int mapHeight)
    {
        mapProprieties.InitializeOctaves();

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

        if (!mapProprieties.useCustomOctaveSettings)
        {
            for (int i = 0; i < mapProprieties.octaves; i++)
            {
                maxPossibleHeight += amplitude;
                amplitude *= mapProprieties.persistance;
            }
        }
        else
        {
            for (int i = 0; i < mapProprieties.octaves; i++)
            {
                maxPossibleHeight += mapProprieties.customOctaveSettings[i].amplitude;
            }
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

                    if (!mapProprieties.useCustomOctaveSettings)
                    {
                        noiseHeight += perlinValue * amplitude;
                        amplitude *= mapProprieties.persistance;
                        frequency *= mapProprieties.lacunarity;
                    }
                    else
                    {
                        noiseHeight += perlinValue * mapProprieties.customOctaveSettings[i].amplitude;
                        frequency = mapProprieties.customOctaveSettings[i].frequency;
                    }
                }

                noiseMap[x, y] = noiseHeight / maxPossibleHeight;
                noiseMap[x, y] = Mathf.Pow(noiseMap[x, y] * mapProprieties.fudgeFactor, mapProprieties.contrast);
                noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y]);
            }
        }

        switch (mapProprieties.smoothingMethod)
        {
            case SmoothingMethod.GaussianBlur:
                noiseMap = ApplyGaussianBlur(noiseMap, (int)mapProprieties.smoothingFactor);
                break;
            case SmoothingMethod.SimpleAverage:
                noiseMap = SimpleAverage(noiseMap, (int)mapProprieties.smoothingFactor);
                break;
            case SmoothingMethod.LaplacianSmoothing:
                noiseMap = ApplyLaplacianSmoothing(noiseMap, mapProprieties.smoothingFactor);
                break;
        }

        return noiseMap;
    }

    public static float[,] ApplyGaussianBlur(float[,] noiseMap, int kernelSize)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        float[,] blurredMap = new float[width, height];

        float[,] kernel = new float[kernelSize, kernelSize];
        float sigma = kernelSize / 3f;
        float mean = kernelSize / 2f;
        float sum = 0f;

        for (int x = 0; x < kernelSize; x++)
        {
            for (int y = 0; y < kernelSize; y++)
            {
                kernel[x, y] = Mathf.Exp(-((Mathf.Pow(x - mean, 2) + Mathf.Pow(y - mean, 2)) / (2 * sigma * sigma)));
                sum += kernel[x, y];
            }
        }

        for (int x = 0; x < kernelSize; x++)
        {
            for (int y = 0; y < kernelSize; y++)
            {
                kernel[x, y] /= sum;
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float blurredValue = 0f;
                for (int kernelX = 0; kernelX < kernelSize; kernelX++)
                {
                    for (int kernelY = 0; kernelY < kernelSize; kernelY++)
                    {
                        int sampleX = x + kernelX - kernelSize / 2;
                        int sampleY = y + kernelY - kernelSize / 2;
                        sampleX = Mathf.Clamp(sampleX, 0, width - 1);
                        sampleY = Mathf.Clamp(sampleY, 0, height - 1);

                        blurredValue += noiseMap[sampleX, sampleY] * kernel[kernelX, kernelY];
                    }
                }
                blurredMap[x, y] = blurredValue;
            }
        }
        return blurredMap;
    }

    public static float[,] SimpleAverage(float[,] noiseMap, int radius)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        float[,] averagedMap = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float sum = 0f;
                int count = 0;

                for (int offsetX = -radius; offsetX <= radius; offsetX++)
                {
                    for (int offsetY = -radius; offsetY <= radius; offsetY++)
                    {
                        int sampleX = x + offsetX;
                        int sampleY = y + offsetY;
                        sampleX = Mathf.Clamp(sampleX, 0, width - 1);
                        sampleY = Mathf.Clamp(sampleY, 0, height - 1);

                        sum += noiseMap[sampleX, sampleY];
                        count++;
                    }
                }
                averagedMap[x, y] = sum / count;
            }
        }
        return averagedMap;
    }

    public static float[,] ApplyLaplacianSmoothing(float[,] noiseMap, float smoothingFactor)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        float[,] smoothedMap = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float laplacian = 0f;
                for (int offsetX = -1; offsetX <= 1; offsetX++)
                {
                    for (int offsetY = -1; offsetY <= 1; offsetY++)
                    {
                        if (offsetX == 0 && offsetY == 0) continue;

                        int sampleX = x + offsetX;
                        int sampleY = y + offsetY;
                        sampleX = Mathf.Clamp(sampleX, 0, width - 1);
                        sampleY = Mathf.Clamp(sampleY, 0, height - 1);
                        laplacian += noiseMap[sampleX, sampleY];
                    }
                }
                laplacian = laplacian - 4 * noiseMap[x, y];
                smoothedMap[x, y] = noiseMap[x, y] + laplacian * smoothingFactor;
            }
        }
        return smoothedMap;
    }
}
