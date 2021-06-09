using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] GameObject _cellPrefab;

    private NavMeshSurface navMeshSurface;
    private const int DIMENSION = 50;
    private GameObject[,] _cells = new GameObject[DIMENSION, DIMENSION];

    private float cellHalfLength;
    public bool generated = false;
    private bool generatingMap = false;

    // Noise Map variables
    private float[,] noiseMap = new float[DIMENSION, DIMENSION];

    [Space(5)]
    public TerrainType[] regions;

    private void Awake()
	{
        navMeshSurface = gameObject.GetComponent<NavMeshSurface>();
        cellHalfLength = _cellPrefab.GetComponent<Cell>().GetUnitScale() / 2;
        _cells = new GameObject[DIMENSION, DIMENSION];
        noiseMap = new float[DIMENSION, DIMENSION];
        PopulateWithCells();
        GenerateMap();
        generated = true;
    }

    private void PopulateWithCells()
    {
        // Height
        for (int h = 0; h < DIMENSION; h++)
        {
            // Width
            for (int w = 0; w < DIMENSION; w++)
            {
                float x = cellHalfLength * (-DIMENSION + 2*w + 1);
                float z = cellHalfLength * (DIMENSION - 2*h - 1);
                _cells[w, h] = Instantiate(_cellPrefab, new Vector3(x, 0, z), Quaternion.identity, transform);
            }
        }
    }

    public void GenerateMap()
    {
        if (!generatingMap)
        {
            generatingMap = true;
            noiseMap = Noise.GenerateNoiseMap(DIMENSION, DIMENSION, Random.Range(0, 1000));
            ColorMapTerrain();
            BakeNavMesh();
            generatingMap = false;
        }
    }

    private void ColorMapTerrain()
    {
        for (int h = 0; h < DIMENSION; h++)
        {
            for (int w = 0; w < DIMENSION; w++)
            {
                _cells[w, h].layer = 8;
                float currentHeight = noiseMap[w, h];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        _cells[w, h].GetComponent<Cell>().SetColor(regions[i].color);
                        if (currentHeight <= regions[0].height)
                            _cells[w, h].layer = 4;
                        break;
                    }
                }
            }
        }
    }

    public void BakeNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }
}

[System.Serializable]
public struct TerrainType
{
    public float height;
    public Color color;
}