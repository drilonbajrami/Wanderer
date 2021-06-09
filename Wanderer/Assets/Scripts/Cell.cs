using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
	public GameObject agent;
	private SpriteRenderer overlay;

	public Cell[] neighbours = new Cell[8];

	private void Start()
	{
		overlay = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
		overlay.color = new Color(0, 0, 0, 1);
	}

	private void Update()
	{
		if (agent == null)
			agent = GameObject.FindGameObjectWithTag("Player");

		if (overlay.color.a == 1 && agent != null)
		{
			float distance = Vector3.Distance(gameObject.transform.position, agent.transform.position);
			if (distance < 20)
				overlay.color = new Color(0, 0, 0, 0);
		}

		//if (agent != null)
		//{
		//	float distance = Vector3.Distance(gameObject.transform.position, agent.transform.position);
		//	if (distance > 20)
		//		overlay.color = new Color(0, 0, 0, 0.9f);
		//	else
		//		overlay.color = new Color(0, 0, 0, 0);
		//}
	}

	public void SetColor(Color color)
    {
		var tempMaterial = new Material(gameObject.GetComponent<MeshRenderer>().sharedMaterial);
		tempMaterial.color = color;
		gameObject.GetComponent<MeshRenderer>().material = tempMaterial;
    }

	public float GetUnitScale()
	{
		return transform.lossyScale.x * 10;
	}

	public void SetNeighbour(Cell neighbour, Spot spot)
	{
		if (spot == Spot.TOP_LEFT)
			neighbours[0] = neighbour;
		else if (spot == Spot.TOP)
			neighbours[1] = neighbour;
		else if (spot == Spot.TOP_RIGHT)
			neighbours[2] = neighbour;
		else if (spot == Spot.LEFT)
			neighbours[3] = neighbour;
		else if (spot == Spot.RIGHT)
			neighbours[4] = neighbour;
		else if (spot == Spot.BOTTOM_LEFT)
			neighbours[5] = neighbour;
		else if (spot == Spot.BOTTOM)
			neighbours[6] = neighbour;
		else
			neighbours[7] = neighbour;
	}
}

public enum Spot
{
	TOP_LEFT = 0,
	TOP,
	TOP_RIGHT,
	LEFT,
	RIGHT,
	BOTTOM_LEFT,
	BOTTOM,
	BOTTOM_RIGHT
}