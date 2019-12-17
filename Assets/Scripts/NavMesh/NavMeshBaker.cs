using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBaker : MonoBehaviour
{
    [Tooltip("True: Automatikusan bakeli a navmeshet a játék indításakor")]
    public bool autoBake = false;
    private NavMeshSurface surface;
    void Start()
    {
        if(autoBake)
        {
            surface = GetComponent<NavMeshSurface>();
            surface.BuildNavMesh();
        }
    }
}
