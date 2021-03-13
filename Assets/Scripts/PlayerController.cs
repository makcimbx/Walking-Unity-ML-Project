using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class PlayerController : Agent
{
    [SerializeField] private Rigidbody rigBody;
    [SerializeField] private Transform finishTransform;
    [SerializeField] private bool UseVecObs;
    [SerializeField] private Color areaColor;

    private EnvironmentParameters m_ResetParams;

    private Quaternion startQuaternion;
    private int impulseCounter;

    public override void Initialize()
    {
        startQuaternion = transform.rotation;
        m_ResetParams = Academy.Instance.EnvironmentParameters;
        SetResetParameters();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (UseVecObs)
        {
            sensor.AddObservation(finishTransform.localPosition);
            sensor.AddObservation(gameObject.transform.localPosition);
            sensor.AddObservation(gameObject.transform.rotation);
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var actionX = actionBuffers.ContinuousActions[0];
        var actionY = actionBuffers.ContinuousActions[1];
        var actionZ = actionBuffers.ContinuousActions[2];
        var upImpulse = actionBuffers.ContinuousActions[3];

        rigBody.AddForce(new Vector3(actionX, actionY, actionZ) * 5);
        if (impulseCounter == 0) rigBody.AddForce(Vector3.up * upImpulse * 10, ForceMode.Impulse);

        var curDistance = Vector3.Distance(transform.localPosition, finishTransform.localPosition);
        if (curDistance > 12)
        {
            EndEpisode();
        }
        else if (curDistance < 1)
        {
            SetReward(1f);
            EndEpisode();
        }

        impulseCounter++;
        if (impulseCounter == 80) impulseCounter = 0;
    }

    public override void OnEpisodeBegin()
    {
        //Reset the parameters when the Agent is reset.
        SetResetParameters();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Jump");
        continuousActionsOut[1] = Input.GetAxis("Jump");
        continuousActionsOut[2] = Input.GetAxis("Jump");
        continuousActionsOut[3] = Input.GetAxis("Jump");
    }

    public void SetResetParameters()
    {
        var range = 18;
        transform.localPosition = new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range));
        finishTransform.localPosition = new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range));
        transform.rotation = startQuaternion;
        rigBody.velocity = Vector3.zero;
        rigBody.angularVelocity = Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = areaColor;
        Gizmos.DrawCube(transform.parent.position, new Vector3(40, 40, 40));
    }
}
