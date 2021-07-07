using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
	private (int, int) index;
	public (int, int) Index => index;

	public GameObject agent;
	private SpriteRenderer overlay;
	private List<Cell> neighbours = new List<Cell>();

	private float overlayAlpha = 1.0f;

	public bool isWater = false;
	[SerializeField] private float memoryPersistance = 1.0f;

	private void Start()
	{
		overlay = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
		overlay.color = new Color(0, 0, 0, overlayAlpha);
	}

	private void Update()
	{
		Debug.Log(Time.deltaTime);
		if (gameObject.layer == 4)
		{
			overlayAlpha += 0.0000001f;
			overlayAlpha = Mathf.Clamp(overlayAlpha, 0.0f, 1.0f);
			overlay.color = new Color(0, 0, 0, overlayAlpha);
		}
		else
		{
			overlayAlpha += Time.deltaTime * 0.01f / memoryPersistance;
			overlayAlpha = Mathf.Clamp(overlayAlpha, 0.0f, 1.0f);
			overlay.color = new Color(0, 0, 0, overlayAlpha);
		}

		if (overlayAlpha == 1.0f)
			isWater = false;
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
				neighbour.IncreaseMemoryPersistance();
			}
			else if (neighbour.gameObject.layer != 4)
			{
				neighbour.SetOverlayAlpha(0.5f);
			}
		}
	}

	private void SetOverlayAlpha(float value)
	{
		overlayAlpha = Mathf.Clamp(overlayAlpha - value, 0.0f, 1.0f);
		overlay.color = new Color(0, 0, 0, overlayAlpha);
	}

	public void IncreaseMemoryPersistance()
	{
		memoryPersistance += 0.1f;
		foreach (Cell neighbour in neighbours)
			neighbour.IncreasePersistanceAsNeighbour();
	}

	private void IncreasePersistanceAsNeighbour()
	{
		memoryPersistance += 0.05f;
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