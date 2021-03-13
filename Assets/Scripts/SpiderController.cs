using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class SpiderController : Agent
{
    [SerializeField] private Spider spider;
    [SerializeField] private Transform finishTransform;
    [SerializeField] private bool UseVecObs;
    [SerializeField] private Color areaColor;

    private EnvironmentParameters m_ResetParams;

    private List<Quaternion> startQuaternion = new List<Quaternion>();
    private List<Vector3> startPosition = new List<Vector3>();
    private int impulseCounter;

    public override void Initialize()
    {
        m_ResetParams = Academy.Instance.EnvironmentParameters;
        spider.Initialize();
        SetResetParameters();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (UseVecObs)
        {
            sensor.AddObservation(finishTransform.position);
            foreach (var spiderLeg in spider.SpiderLegs)
            {
                sensor.AddObservation(spiderLeg.BodyLeg.Position);
                sensor.AddObservation(spiderLeg.BodyLeg.Rotation);
                sensor.AddObservation(spiderLeg.LowerLeg.Position);
                sensor.AddObservation(spiderLeg.LowerLeg.Rotation);
                sensor.AddObservation(spiderLeg.MiddleLeg.Position);
                sensor.AddObservation(spiderLeg.MiddleLeg.Rotation);
            }
            sensor.AddObservation(spider.Position);
            sensor.AddObservation(spider.Rotation);
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        for (int i = 0; i < 4; i++)
        {
            var actionX = actionBuffers.ContinuousActions[i * 3 + 0];
            var actionY = actionBuffers.ContinuousActions[i * 3 + 1];
            var actionZ = actionBuffers.ContinuousActions[i * 3 + 2];

            spider.SpiderLegs[i].BodyLeg.SetMotorVelocityAndForce(actionX * 1000, 100);
            spider.SpiderLegs[i].LowerLeg.SetMotorVelocityAndForce(actionY * 1000, 100);
            spider.SpiderLegs[i].MiddleLeg.SetMotorVelocityAndForce(actionZ * 1000, 100);
        }

        var curDistance = Vector3.Distance(spider.Position, finishTransform.position);
        var maxDistance = Vector3.Distance(spider.StartPosition, finishTransform.position);
        var progress = 1 - (curDistance / maxDistance);
        if (progress <= -1)
        {
            SetReward(-1f);
            EndEpisode();
        }
        else if (curDistance < 5)
        {
            SetReward(1f);
            EndEpisode();
        }
        else
        {
            SetReward(progress);
        }
    }

    public override void OnEpisodeBegin()
    {
        //Reset the parameters when the Agent is reset.
        SetResetParameters();
    }

    // public override void Heuristic(in ActionBuffers actionsOut)
    // {
    //     var continuousActionsOut = actionsOut.ContinuousActions;
    //     continuousActionsOut[0] = Input.GetAxis("Jump");
    // }

    public void SetResetParameters()
    {
        spider.Reset();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = areaColor;
        Gizmos.DrawCube(transform.parent.position, new Vector3(40, 40, 40));
    }
}
