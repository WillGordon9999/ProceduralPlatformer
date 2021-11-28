using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Initially learned from: https://www.youtube.com/watch?v=vFvwyu_ZKfU
public class LandscapeGenerator : MonoBehaviour
{
    //MUST BE A POWER OF TWO!!
    public int width = 256;
    //MUST BE A POWER OF TWO!!
    public int length = 256;

    public int height = 20;
    public float scale;
    

    public float MaxOffsetX = 100f;
    public float MaxOffsetY = 100f;

    public float minScale = 0.5f;
    public float maxScale = 5.0f;

    float offsetX, offsetY;

    Terrain terrain;
    // Start is called before the first frame update
    void Start()
    {
        Create();
    }

    void Create()
    {
        //offsetX = Random.Range(0f, MaxOffsetX);
        //offsetY = Random.Range(0f, MaxOffsetY);

        scale = Random.Range(minScale, maxScale);


        terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    Create();
        //}
    }

    TerrainData GenerateTerrain(TerrainData data)
    {
        data.heightmapResolution = width;

        data.size = new Vector3(width, height, length);

        data.SetHeights(0, 0, GenerateHeights());

        return data;
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[width, length];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < length; y++)
            {
                heights[x, y] = CalculateHeight(x, y);
            }
        }

        return heights;
    }

    float CalculateHeight(int x, int y)
    {
        //float xCoord = (float)x / width * scale + offsetX;
        //float yCoord = (float)y / height * scale + offsetY;
        float xCoord = (float)x / width * scale;
        float yCoord = (float)y / height * scale;

        float p1 = Mathf.PerlinNoise(xCoord + MaxOffsetX, yCoord + MaxOffsetY);
        float p2 = Mathf.PerlinNoise(xCoord * scale, yCoord * scale);
        return Mathf.Lerp(p1, p2, 0.5f);

        //return Mathf.PerlinNoise(xCoord + MaxOffsetX, yCoord + MaxOffsetY);
    }
}
