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
                foreach (var leg in spiderLeg.LegsList)
                {
                    sensor.AddObservation(leg.Position);
                    sensor.AddObservation(leg.Rotation.eulerAngles);
                    sensor.AddObservation(leg.Rigidbody.velocity);
                }
            }
            sensor.AddObservation(spider.Position);
            sensor.AddObservation(spider.Rotation.eulerAngles);
            sensor.AddObservation(spider.RigBody.velocity);
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        for (int i = 0; i < 4; i++)
        {
            var actionX = actionBuffers.ContinuousActions[i * 3 + 0];
            var actionY = actionBuffers.ContinuousActions[i * 3 + 1];
            var actionZ = actionBuffers.ContinuousActions[i * 3 + 2];

            spider.SpiderLegs[i].LegsList[0].SetMotorVelocityAndForce(actionX * 1000, 100);
            spider.SpiderLegs[i].LegsList[1].SetMotorVelocityAndForce(actionY * 1000, 100);
            spider.SpiderLegs[i].LegsList[2].SetMotorVelocityAndForce(actionZ * 1000, 100);
        }

        bool isLegOnFloor = false;
        foreach (var leg in spider.SpiderLegs)
        {
            int counter = 0;
            foreach (var legElement in leg.LegsList)
            {
                if (counter == 2) continue;
                isLegOnFloor = isLegOnFloor || legElement.OnFloor;
                counter++;
            }
        }

        var curDistance = Vector3.Distance(spider.Position, finishTransform.position);
        var maxDistance = Vector3.Distance(spider.StartPosition, finishTransform.position);
        var progress = 1 - (curDistance / maxDistance);
        if (progress <= -1 || spider.OnFloor || isLegOnFloor)
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
