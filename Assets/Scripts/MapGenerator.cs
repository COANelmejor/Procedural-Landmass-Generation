using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode {NoiseMap, ColorMap, Mesh}
    public DrawMode drawMode;

    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;
    
    public int seed;
    public Vector2 offset;

    public float heightMultiplier;
    public bool autoUpdate;
    public TerrainType[] regions;
    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);
        Color[] colorMap = new Color[mapWidth * mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int r = 0; r < regions.Length; r++)
                {
                    if(currentHeight < regions[r].height)
                    {
                        colorMap[x * mapWidth + y] = regions[r].color;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        } else if (drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
        } else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, heightMultiplier), TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
        }
    }

    private void OnValidate()
    {
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }
        if (mapHeight < 1)
        {
            mapHeight = 1;
        }
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 1)
        {
            octaves = 1;
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    [Range(0, 1)]
    public float height;
    public Color color;
}