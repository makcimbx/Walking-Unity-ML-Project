using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private float maxSteerAngle;
    [SerializeField] private float maxTorque;
    
    [Space]
    [SerializeField] private Wheel[] wheels;

    private void Update()
    {
        Quaternion q;
        Vector3 p;

        var wheelRotation = Input.GetAxis("Horizontal") * maxSteerAngle;
        var wheelTorque = Input.GetAxis("Vertical") * maxTorque;
        
        foreach (var wheel in wheels)
        {
            wheel.Collider.GetWorldPose(out p, out q);
 
            wheel.Model.position = p;
            wheel.Model.rotation = q;

            if (wheel.Forward) wheel.Collider.steerAngle = wheelRotation;
            
            if(wheel.Torque) wheel.Collider.motorTorque = wheelTorque;
        }
    }
        
    [System.Serializable]
    internal class Wheel
    {
        [SerializeField] private WheelCollider wheel;
        [SerializeField] private Transform model;
        [SerializeField] private bool forward;
        [SerializeField] private bool torque;
        
        internal WheelCollider Collider => wheel;
        internal Transform Model => model;
        internal bool Forward => forward;
        internal bool Torque => torque;
    }
}
