using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [SerializeField] private TerrainGenerator terrain;
    [SerializeField] private GameObject agentPrefab;
    private GameObject agent;

	public void ResetSimulation()
	{
		Destroy(agent);
		terrain.ResetTerrain();
	}

	public void SpawnAgent()
	{
		agent = Instantiate(agentPrefab, Vector3.zero, Quaternion.identity);
	}

	public void AdjustTimeScale(float scale)
	{
		Time.timeScale = scale;
	}
}
