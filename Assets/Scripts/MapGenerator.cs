﻿using System.Collections;
using UnityEngine;

public class MapGenerator: MonoBehaviour {

    public enum DrawMode { NoiseMap, ColorMap, Mesh};
    public DrawMode drawMode;

    public const int mapChunkSize = 241;
    [Range(0, 6)]
    public int levelOfDeatil;
    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public float heightMultiplier;
    public AnimationCurve meshHeightCurve;

    public bool autoUpdate;

    public TerrainType[] regions;

    public void GenerateMap() {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);

        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++) {
            for (int x = 0; x < mapChunkSize; x++) {
                float currentHeight = noiseMap[x, y];
                for (int r = 0; r < regions.Length; r++) {
                    if (currentHeight <= regions[r].height) {
                        colorMap[y * mapChunkSize + x] = regions[r].color;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType <MapDisplay> ();
        if (drawMode == DrawMode.NoiseMap) {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        } else if (drawMode == DrawMode.ColorMap) {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
        } else if (drawMode == DrawMode.Mesh) {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, heightMultiplier, meshHeightCurve, levelOfDeatil), TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
        }
    }

    private void OnValidate() {
        if (lacunarity < 1) {
            lacunarity = 1;
        }
        if (octaves < 1) {
            octaves = 1;
        }
    }
}

[System.Serializable]
public struct TerrainType {
    public string name;
    [Range(0, 1)]
    public float height;
    public Color color;
}