using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;
    [SerializeField] private float noiseScale;

    public bool autoUpdate;

    public void GenerateMap()
    {
        mapHeight = (mapHeight <= 0) ? 1 : mapHeight;
        mapWidth = (mapWidth <= 0) ? 1 : mapWidth;
        noiseScale = (noiseScale <= 0) ? 0.0001f : noiseScale;

        float[,] noiseMap = NoiseGenerator.GenerateNoiseMap(mapWidth, mapHeight, noiseScale);
        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawNoiseMap(noiseMap);
    }

}
