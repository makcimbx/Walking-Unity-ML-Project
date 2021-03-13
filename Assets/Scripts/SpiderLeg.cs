using System;
using UnityEngine;

public class SpiderLeg : MonoBehaviour
{
    [SerializeField] private LegElement bodyLeg;
    [SerializeField] private LegElement lowerLeg;
    [SerializeField] private LegElement middleLeg;

    internal LegElement BodyLeg => bodyLeg;
    internal LegElement LowerLeg => lowerLeg;
    internal LegElement MiddleLeg => middleLeg;

    internal void Initialize()
    {
        bodyLeg.OnAwake();
        lowerLeg.OnAwake();
        middleLeg.OnAwake(); 
    }

    internal void Reset()
    {
        bodyLeg.Reset();
        lowerLeg.Reset();
        middleLeg.Reset();   
    }
    
    [System.Serializable]
    public class LegElement
    {
        [SerializeField] private Rigidbody rigidbody;
        [SerializeField] private HingeJoint joint;

        private Vector3 startPosition;
        private Quaternion startQuaternion;
        
        internal Vector3 Position => rigidbody.position;
        internal Quaternion Rotation => rigidbody.rotation;

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
        }
    }
}
