using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MusculeController : Agent
{
    [SerializeField] private List<Rigidbody> transformList;
    [SerializeField] private HingeJoint joint;
    [SerializeField] private Transform finishTransform;
    [SerializeField] private bool UseVecObs;
    [SerializeField] private Color areaColor;

    private EnvironmentParameters m_ResetParams;

    private List<Quaternion> startQuaternion = new List<Quaternion>();
    private List<Vector3> startPosition = new List<Vector3>();
    private int impulseCounter;

    public override void Initialize()
    {
        foreach (var item in transformList)
        {
            startPosition.Add(item.transform.position);
            startQuaternion.Add(item.transform.rotation);
        }

        m_ResetParams = Academy.Instance.EnvironmentParameters;
        SetResetParameters();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (UseVecObs)
        {
            sensor.AddObservation(finishTransform.position);
            // foreach (var item in transformList)
            // {
            //     sensor.AddObservation(item.transform.position);
            //     sensor.AddObservation(item.transform.rotation);
            // }
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var actionX = actionBuffers.ContinuousActions[0];

        joint.motor = new JointMotor() { targetVelocity = actionX * 1000, force = 100, freeSpin = false };

        var curDistance = Vector3.Distance(joint.transform.position, finishTransform.position);
        if (curDistance > 25)
        {
            EndEpisode();
        }
        else if (curDistance < 1)
        {
            SetReward(1f);
            EndEpisode();
        }
        else if(curDistance < 10)
        {
            SetReward(0.5f);
        }
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
    }

    public void SetResetParameters()
    {
        joint.motor = new JointMotor() { targetVelocity = 0, force = 100, freeSpin = false };
        int counter = 0;
        foreach (var item in transformList)
        {
            item.transform.position = startPosition[counter];
            item.transform.rotation = startQuaternion[counter];
            item.velocity = Vector3.zero;
            item.angularVelocity = Vector3.zero;

            counter++;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = areaColor;
        Gizmos.DrawCube(transform.parent.position, new Vector3(40, 40, 40));
    }
}
