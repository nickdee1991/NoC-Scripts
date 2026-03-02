using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;


public class RemoveNavmesh : MonoBehaviour
{
    public void Awake()
    {
        ForceCleanupNavMesh();
    }

    [MenuItem("Light Brigade/Debug/Force Cleanup NavMesh")]
    public static void ForceCleanupNavMesh()
    {
        if (Application.isPlaying)
            return;

        NavMesh.RemoveAllNavMeshData();
    }
}
