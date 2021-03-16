using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointsGizmos : MonoBehaviour
{
    [SerializeField] private Transform[] checkpoints;
    
    private void OnDrawGizmos()
    {
        for (var i = 0; i < checkpoints.Length - 1; i++)
        {
            var current = checkpoints[i];
            var next = checkpoints[i + 1];
            Gizmos.DrawLine(current.position, next.position);
        }
        
        Gizmos.DrawLine(checkpoints[checkpoints.Length - 1].position, checkpoints[0].position);
        
        
    }
}
