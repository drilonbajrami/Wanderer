using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wanderer : MonoBehaviour
{
    private NavMeshAgent _agent;
    private GameObject[,] _memoryMap;

    void Start()
    {
        _agent = gameObject.GetComponent<NavMeshAgent>();
        _memoryMap = GameObject.FindGameObjectWithTag("Terrain").GetComponent<TerrainGenerator>().Cells;
    }

    void Update()
    {
		if (_agent.velocity.magnitude < 0.2f)
			_agent.SetDestination(RandomNavSphere(20));

		//if (Input.GetMouseButtonDown(1))
		//{
		//    RaycastHit hit;

		//    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
		//    {
		//        _agent.SetDestination(hit.point);
		//    }
		//}
	}

    private Vector3 RandomDestination(float radius)
    {
        Vector3 randomPoint = (Random.insideUnitSphere + transform.position) * 10;

        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        else
        {
            finalPosition = RandomDestination(radius);
        }

        return finalPosition;
    }

    public Vector3 RandomNavSphere(float distance)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;

        randomDirection += gameObject.transform.position;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, distance, -1);
        return navHit.position;
    }

	public void OnTriggerEnter(Collider other)
	{
        other.gameObject.GetComponent<Cell>().Explore();
	}

	public void OnTriggerStay(Collider other)
	{
        other.gameObject.GetComponent<Cell>().IncreaseMemoryPersistence();
        other.gameObject.GetComponent<Cell>().IncreaseHeatScale();
	}
}