using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wanderer : MonoBehaviour
{
    private NavMeshAgent _agent;
    private GameObject[,] _memoryMap;
    private (int, int) currentCellIndex;

    void Start()
    {
        _agent = gameObject.GetComponent<NavMeshAgent>();
        _memoryMap = GameObject.FindGameObjectWithTag("Terrain").GetComponent<TerrainGenerator>().Cells;
    }

    void Update()
    {
        if (_agent.velocity.magnitude < 0.2f)
        {
            _agent.SetDestination(RandomNavSphere(20.0f));
            //if (Random.Range(0, 2) == 0)
            //_agent.SetDestination(RandomPositionWithinFOV(Random.Range(20.0f, 100.0f), 90.0f));
            //else
                //_agent.SetDestination(NextDestination());
            //_agent.SetDestination(RandomNavSphere(20.0f));
        }
	}

    private Vector3 NextDestination()
    {
        List<Cell> currentNeighbours = _memoryMap[currentCellIndex.Item1, currentCellIndex.Item2].GetComponent<Cell>().Neighbours;

        currentNeighbours.Sort(delegate (Cell a, Cell b)
        {
            return (a.MemoryPersistence).CompareTo(b.MemoryPersistence);
        });

        return currentNeighbours[0].gameObject.transform.position;
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

    private Vector3 RandomPositionWithinFOV(float radius, float FOV)
    {
        Vector3 randomPoint = Random.insideUnitSphere * radius + gameObject.transform.position;
        Vector3 directionToRandomPoint = randomPoint - gameObject.transform.position;
        float angle = Vector3.Angle(gameObject.transform.forward, directionToRandomPoint);

        if (Mathf.Abs(angle) < FOV / 2)
            return randomPoint;
        else
            return RandomPositionWithinFOV(radius, FOV);
    }

    public void OnTriggerEnter(Collider other)
	{
        other.gameObject.GetComponent<Cell>().Explore();
        currentCellIndex = other.gameObject.GetComponent<Cell>().Index;
	}

	public void OnTriggerStay(Collider other)
	{
        other.gameObject.GetComponent<Cell>().IncreaseMemoryPersistence();
        other.gameObject.GetComponent<Cell>().IncreaseHeatScale();
	}
}