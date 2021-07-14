using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
	public Gradient heatMapGradient;
	private (int, int) index;
	public (int, int) Index => index;

	private SpriteRenderer overlay;
	private SpriteRenderer heatOverlay;
	private List<Cell> neighbours = new List<Cell>();
	public List<Cell> Neighbours => neighbours;

	private float overlayAlpha = 1.0f;
	private float heatScale = 0.0f;
	private float memoryPersistence = 1.0f;

	[HideInInspector] public bool isWater = false;
	public float MemoryPersistence => memoryPersistence;

	private readonly float learnRate = 30.0f;
	private readonly float heatRate = 0.0003f;

	private void Start()
	{
		overlay = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
		heatOverlay = gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>();
		overlay.color = new Color(0, 0, 0, overlayAlpha);
	}

	private void Update()
	{
		if (memoryPersistence < 1000.0f)
		{
			if (gameObject.layer == 4)
				SetOverlayAlpha(-0.0000001f);
			else
				SetOverlayAlpha(-Time.deltaTime * 0.25f / memoryPersistence);
		}

		if (overlayAlpha == 1.0f)
			isWater = false;

		heatOverlay.color = heatMapGradient.Evaluate(heatScale);
	}

	public void Explore()
	{
		SetOverlayAlpha(1.0f);
		foreach (Cell neighbour in neighbours)
		{
			if (neighbour.isWater == false && neighbour.gameObject.layer == 4)
			{
				neighbour.isWater = true;
				neighbour.Explore();
				neighbour.IncreaseMemoryPersistence();
			}
			else if (neighbour.gameObject.layer != 4)
			{
				neighbour.SetOverlayAlpha(0.5f);
			}
		}
	}

	private void SetOverlayAlpha(float value)
	{
		overlayAlpha -= value;
		overlayAlpha = Mathf.Clamp(overlayAlpha, 0.0f, 1.0f);
		overlay.color = new Color(0, 0, 0, overlayAlpha);
	}

	public void IncreaseMemoryPersistence()
	{
		memoryPersistence += Time.deltaTime * learnRate;
		foreach (Cell neighbour in neighbours)
			neighbour.IncreasePersistenceAsNeighbour();
	}

	private void IncreasePersistenceAsNeighbour()
	{
		memoryPersistence += Time.deltaTime * learnRate / 2;
	}

	public void IncreaseHeatScale()
	{
		heatScale += heatRate;
		heatScale = Mathf.Clamp(heatScale, 0.0f, 1.0f);

		foreach (Cell neighbour in neighbours)
			neighbour.IncreaseHeatScaleAsNeighbour();
	}

	public void IncreaseHeatScaleAsNeighbour()
	{
		heatScale += heatRate / 2;
		heatScale = Mathf.Clamp(heatScale, 0.0f, 1.0f);
	}

	public void ResetCell()
	{
		overlayAlpha = 1.0f;
		heatScale = 0.0f;
		memoryPersistence = 1.0f;
		isWater = false;
	}

	#region OnGenerationMethods
	public void SetRegion(Color color)
    {
		var tempMaterial = new Material(gameObject.GetComponent<MeshRenderer>().sharedMaterial);
		tempMaterial.color = color;
		gameObject.GetComponent<MeshRenderer>().material = tempMaterial;
    }

	public float GetUnitScale()
	{
		return transform.lossyScale.x * 10;
	}

	public void SetIndex(int w, int h) => index = (w,h);

	public void AddNeighbour(Cell neighbour)
	{
		neighbours.Add(neighbour);
	}
	#endregion

	#region PrintMethods
	public void PrintIndex()
	{
		Debug.Log($"Index: {index.Item1}, {index.Item2}");
	}

	public void PrintOutNeighbours(GameObject[,] cells)
	{
		foreach (Cell neighbour in neighbours)
			Debug.Log($"Neighbour: [{neighbour.transform.position.x}, {neighbour.transform.position.z}]");
	}
	#endregion
}