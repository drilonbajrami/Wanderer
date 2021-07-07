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
    public GameObject[,] Cells => _cells;

    private float cellHalfLength;
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
    }

	private void PopulateWithCells()
    {
        // Height
        for (int h = 0; h < DIMENSION; h++)
        {
            // Width
            for (int w = 0; w < DIMENSION; w++)
            {
                float x = cellHalfLength * (-DIMENSION + 2 * w + 1);
                float z = cellHalfLength * (DIMENSION - 2 * h - 1);
                GameObject cell = Instantiate(_cellPrefab, new Vector3(x, 0, z), Quaternion.identity, transform);
                cell.GetComponent<Cell>().SetIndex(w, h);
                _cells[w, h] = cell;
            }
        }

        for (int h = 0; h < DIMENSION; h++)
            for (int w = 0; w < DIMENSION; w++)
                SetNeighbours(w, h);
    }

    private void GenerateMap()
    {
        if (!generatingMap)
        {
            generatingMap = true;
            noiseMap = Noise.GenerateNoiseMap(DIMENSION, DIMENSION, Random.Range(0, 1000));
            MapTerrain();
            BakeNavMesh();
            generatingMap = false;
        }
    }

    private void MapTerrain()
    {
        for (int h = 0; h < DIMENSION; h++)
        {
            for (int w = 0; w < DIMENSION; w++)
            {
                // Default layer
                _cells[w, h].layer = 8; 
                float currentHeight = noiseMap[w, h];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        _cells[w, h].GetComponent<Cell>().SetRegion(regions[i].color);
                        if (currentHeight <= regions[0].height)
                            _cells[w, h].layer = 4; // Water layer
                        break;
                    }
                }
            }
        }
    }

    private void BakeNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }

    private void SetNeighbours(int w, int h)
    {
        if (DIMENSION > 0)
        {
            for (int y = Mathf.Max(0, h - 1); y <= Mathf.Min(h + 1, DIMENSION - 1); y++)
                for (int x = Mathf.Max(0, w - 1); x <= Mathf.Min(w + 1, DIMENSION - 1); x++)
                    if (x != w || y != h)
                        _cells[w, h].gameObject.GetComponent<Cell>().AddNeighbour(_cells[x, y].gameObject.GetComponent<Cell>());
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    public float height;
    public Color color;
}