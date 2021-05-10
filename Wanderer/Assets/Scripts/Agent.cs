using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Agent : MonoBehaviour
{
    private Bounds bounds;
    private NavMeshAgent agent;
    
    
    private void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        bounds = GameObject.FindGameObjectWithTag("Terrain").GetComponent<MeshCollider>().bounds;
    }

    private void Update()
    {
        if(!agent.hasPath)
            agent.SetDestination(GetRandomDestination());
    }

    private Vector3 GetRandomDestination()
    {
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector3(x, y, 0);
    }
}
