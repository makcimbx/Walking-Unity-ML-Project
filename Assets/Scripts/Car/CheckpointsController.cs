using UnityEngine;

public class CheckpointsController : MonoBehaviour
{
    [SerializeField] private Transform playerBody;
    [SerializeField] private float minDistance;

    [Space]
    [SerializeField] private Transform[] checkpoints;

    private int targetCheckpoint;

    private void Update()
    {
        var distance = Vector3.Distance(playerBody.position, checkpoints[targetCheckpoint].position);

        if (distance <= minDistance)
        {
            targetCheckpoint++;
        }
    }

    private void OnDrawGizmos()
    {
        if (checkpoints == null || checkpoints.Length == 0) return;

        for (var i = 0; i < checkpoints.Length - 1; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(checkpoints[i].position, checkpoints[i + 1].position);

            if (targetCheckpoint == i) Gizmos.color = Color.yellow;

            Gizmos.DrawSphere(checkpoints[i].position, minDistance);
        }

        if (targetCheckpoint == checkpoints.Length - 1) Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(checkpoints[checkpoints.Length - 1].position, minDistance);
    }
}