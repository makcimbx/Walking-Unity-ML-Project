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
    private float stepReward;

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
            // foreach (var spiderLeg in spider.SpiderLegs)
            // {
            //     foreach (var leg in spiderLeg.LegsList)
            //     {
            //         sensor.AddObservation(transform.InverseTransformDirection(leg.Position));
            //         sensor.AddObservation(leg.Rotation);
            //         sensor.AddObservation(transform.InverseTransformDirection(leg.Rigidbody.velocity));
            //         sensor.AddObservation(transform.InverseTransformDirection(leg.Rigidbody.angularVelocity));
            //     }
            // }
            // sensor.AddObservation(transform.InverseTransformDirection(spider.Position));
            // sensor.AddObservation(spider.Rotation);
            // sensor.AddObservation(transform.InverseTransformDirection(spider.RigBody.velocity));
            // sensor.AddObservation(transform.InverseTransformDirection(spider.RigBody.angularVelocity));
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        stepReward -= 0.0001f;
        for (int i = 0; i < 4; i++)
        {
            var actionX = actionBuffers.ContinuousActions[i * 3 + 0];
            var actionY = actionBuffers.ContinuousActions[i * 3 + 1];
            var actionZ = actionBuffers.ContinuousActions[i * 3 + 2];

            spider.SpiderLegs[i].LegsList[0].SetMotorVelocityAndForce(actionX * 1000, 200);
            spider.SpiderLegs[i].LegsList[1].SetMotorVelocityAndForce(actionY * 1000, 200);
            spider.SpiderLegs[i].LegsList[2].SetMotorVelocityAndForce(actionZ * 1000, 200);
        }

        // bool isLegOnFloor = false;
        // foreach (var leg in spider.SpiderLegs)
        // {
        //     foreach (var legElement in leg.LegsList)
        //     {
        //         isLegOnFloor = isLegOnFloor || legElement.OnFloor;
        //     }
        // }

        var curDistance = Vector3.Distance(spider.Position, finishTransform.position);
        var maxDistance = Vector3.Distance(spider.StartPosition, finishTransform.position);
        var progress = 1 - (curDistance / maxDistance);
        if (progress <= -1 || spider.OnFloor || spider.CheckUpsideDown())
        {
            SetReward(-1f + stepReward);
            EndEpisode();
        }
        else if (curDistance < 3)
        {
            SetReward(1f + stepReward);
            EndEpisode();
        }
        else
        {
            var dir = finishTransform.position - spider.transform.position;
            SetReward(progress * Vector3.Dot(spider.transform.forward, dir.normalized) + stepReward);
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
        stepReward = 0;
        spider.Reset();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = areaColor;
        Gizmos.DrawCube(transform.parent.position, new Vector3(40, 40, 40));
    }
}
