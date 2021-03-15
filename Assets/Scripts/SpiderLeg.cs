using System.Collections.Generic;
using UnityEngine;

public class SpiderLeg : MonoBehaviour
{
    [SerializeField] private List<LegElement> legsList;

    internal List<LegElement> LegsList => legsList;

    internal void Initialize()
    {
        foreach (var legElement in legsList)
        {
            legElement.OnAwake();
        }
    }

    internal void Reset()
    {
        foreach (var legElement in legsList)
        {
            legElement.Reset();
        }
    }
    
    [System.Serializable]
    public class LegElement
    {
        [SerializeField] private Rigidbody rigidbody;
        [SerializeField] private HingeJoint joint;
        [SerializeField] private SpiderLegHelper helper;

        private Vector3 startPosition;
        private Quaternion startQuaternion;
        
        internal Vector3 Position => rigidbody.position;
        internal Quaternion Rotation => rigidbody.rotation;
        internal bool OnFloor => helper?.OnFloor ?? false;
        internal Rigidbody Rigidbody => rigidbody;

        internal void OnAwake()
        {
            startPosition = rigidbody.transform.position;
            startQuaternion = rigidbody.transform.rotation;
        }
        
        internal void SetMotorVelocityAndForce(float velocity, float force)
        {
            var motor = new JointMotor();

            motor.targetVelocity = velocity;
            motor.force = force;
            
            joint.motor = motor;
        }
        
        internal void Reset()
        {
            joint.motor = new JointMotor();
            
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;

            rigidbody.transform.position = startPosition;
            rigidbody.transform.rotation = startQuaternion;
            
            if(helper != null) helper.Reset();
        }
    }
}
