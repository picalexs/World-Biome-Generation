using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField] public enum DrawMode { HeightMap, MoistureMap, ColorMap };

[System.Serializable]
public struct TerrainType
{
    public string name;
    [Range(0, 1)] public float height;
    [Range(0, 1)] public float moisture;
    public Color color;
}

[System.Serializable]
public class OctaveSettings
{
    [Range(0, 1)] public float amplitude = 1f;
    [Range(0.01f, 10)] public float frequency = 1f;
}
public enum SmoothingMethod
{
    None,
    GaussianBlur,
    SimpleAverage,
    LaplacianSmoothing
}

[System.Serializable]
public struct MapProprieties
{
    [Min(0.01f)] public float noiseScale;
    [Range(0, 10)] public int octaves;
    public bool useCustomOctaveSettings;
    public OctaveSettings[] customOctaveSettings;
    [Range(0, 1)] public float persistance;
    [Range(1, 10)] public float lacunarity;

    public int seed;
    public Vector2 offset;
    [Range(0.1f, 5)] public float contrast;
    [Range(0.1f, 3)] public float fudgeFactor;
    public SmoothingMethod smoothingMethod;
    public float smoothingFactor;

    public void InitializeOctaves()
    {
        if (useCustomOctaveSettings)
        {
            octaves = customOctaveSettings.Length;
        }
        else
        {
            customOctaveSettings = new OctaveSettings[octaves];
            for (int i = 0; i < octaves; i++)
            {
                customOctaveSettings[i] = new OctaveSettings
                {
                    amplitude = Mathf.Pow(persistance, i),
                    frequency = Mathf.Pow(lacunarity, i)
                };
            }
        }
    }
}


public class MapGenerator : MonoBehaviour
{
    [Header("General Map Proprieties")]
    [SerializeField, Min(1)] public int mapWidth;
    [SerializeField, Min(1)] public int mapHeight;
    [SerializeField] private DrawMode drawMode;

    [Header("Height Map Proprieties")]
    [SerializeField] private MapProprieties heightMapProprieties;

    [Header("Moisture Map Proprieties")]
    [SerializeField] private MapProprieties moistureMapProprieties;

    [Header("Special Proprieties")]
    public TerrainType[] regions;
    public bool autoUpdate;

    public void GenerateMap()
    {
        float[,] heightMap = NoiseGenerator.GenerateNoiseMap(heightMapProprieties, mapWidth, mapHeight);
        float[,] moistureMap = NoiseGenerator.GenerateNoiseMap(moistureMapProprieties, mapWidth, mapHeight);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        switch (drawMode)
        {
            case DrawMode.HeightMap:
                display.DrawTexture(TextureGenerator.TextureFromMap(heightMap));
                break;
            case DrawMode.MoistureMap:
                display.DrawTexture(TextureGenerator.TextureFromMap(moistureMap));
                break;
            case DrawMode.ColorMap:
                display.DrawTexture(TextureGenerator.TextureFromCombinedMaps(heightMap, moistureMap, regions));
                break;
        }
    }
    public void GenerateRandomMapSeed()
    {
        heightMapProprieties.seed = moistureMapProprieties.seed = Random.Range(-100000, 100000);
        GenerateMap();
    }

    public void InitializeRegions()
    {
        regions = new TerrainType[]
        {
        new TerrainType { name = "OCEAN", height = 0f, moisture = 0f, color = new Color(0f, 0.2f, 0.8f) }, // Deep Blue
        new TerrainType { name = "BEACH", height = 0.1f, moisture = 0.1f, color = new Color(1f, 1f, 0.6f) }, // Sandy Yellow
        new TerrainType { name = "MANGROVE", height = 0.2f, moisture = 0.9f, color = new Color(0f, 0.3f, 0f) }, // Dark Green
        new TerrainType { name = "LIMESTONE_CAVES", height = 0.2f, moisture = 0.4f, color = new Color(0.9f, 0.9f, 0.5f) }, // Pale Stone Color
        new TerrainType { name = "TUNDRA", height = 0.3f, moisture = 0.3f, color = new Color(0.8f, 0.8f, 0.6f) }, // Light Grayish
        new TerrainType { name = "GRASSLAND", height = 0.3f, moisture = 0.4f, color = new Color(0.5f, 0.8f, 0.5f) }, // Light Green
        new TerrainType { name = "MARSH", height = 0.3f, moisture = 0.8f, color = new Color(0.2f, 0.5f, 0.2f) }, // Dark Marsh Green
        new TerrainType { name = "DESERT", height = 0.4f, moisture = 0.1f, color = new Color(1f, 0.9f, 0.6f) }, // Light Tan
        new TerrainType { name = "SHRUBLAND", height = 0.4f, moisture = 0.3f, color = new Color(0.4f, 0.6f, 0.4f) }, // Light Olive Green
        new TerrainType { name = "PLAINS", height = 0.4f, moisture = 0.4f, color = new Color(0.6f, 0.9f, 0.5f) }, // Light Greenish Yellow
        new TerrainType { name = "TEMPERATE_FOREST", height = 0.5f, moisture = 0.5f, color = new Color(0.3f, 0.6f, 0.3f) }, // Moderate Green
        new TerrainType { name = "TROPICAL_RAIN_FOREST", height = 0.5f, moisture = 0.8f, color = new Color(0f, 0.5f, 0f) }, // Rich Green
        new TerrainType { name = "SUBTROPICAL_DESERT", height = 0.5f, moisture = 0.2f, color = new Color(1f, 0.8f, 0.4f) }, // Sandy Orange
        new TerrainType { name = "SAVANNAH", height = 0.5f, moisture = 0.3f, color = new Color(1f, 0.9f, 0.4f) }, // Golden Yellow
        new TerrainType { name = "ALPINE", height = 0.7f, moisture = 0.4f, color = new Color(0.9f, 0.9f, 0.8f) }, // Light Gray
        new TerrainType { name = "MOUNTAIN", height = 0.8f, moisture = 0.3f, color = new Color(0.5f, 0.5f, 0.5f) }, // Gray Mountain
        new TerrainType { name = "SNOW", height = 0.8f, moisture = 0.5f, color = Color.white }, // White
        new TerrainType { name = "ICECAP", height = 0.9f, moisture = 0.5f, color = new Color(0.8f, 0.9f, 1f) }, // Light Blue
        new TerrainType { name = "ARCTIC", height = 0.9f, moisture = 0.6f, color = new Color(0.6f, 0.7f, 0.9f) }, // Frosty Blue
        };
    }

}

