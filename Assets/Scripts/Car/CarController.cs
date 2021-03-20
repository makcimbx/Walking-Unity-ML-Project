using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private Transform centerOfMass;
    [SerializeField] private Rigidbody rigidbody;
    
    [Space]
    [SerializeField] private WheelElement[] wheels;
    
    [Space]
    [SerializeField] private float maxTorque;
    [SerializeField] private float maxBrakeTorque;
    [SerializeField] private float maxSteer;
    
    [Space]
    [SerializeField] private float torqueStep;
    [SerializeField] private float steerStep;

    [Space]
    [SerializeField] private AnimationCurve engineCurve;
    [SerializeField] private AnimationCurve steerCurve;
    
    private float targetTorque;
    private float currentTorque;
    private float currentBreakTorque;
    
    private float targetSteer;
    private float currentSteer;

    private void Awake() => SetUpCar();

    private void Update()
    {
        EngineLogic();
        SteerLogic();
        
        CarLogic();
    }

    public void SetEngineValue(float @value)
    {
        targetTorque = maxTorque * @value;
    }

    public void SetSteerValue(float @value)
    {
        targetSteer = maxSteer * @value;
    }

    private void SetUpCar()
    {
        rigidbody.centerOfMass = centerOfMass.localPosition;
        
        SetEngineValue(0);
        SetSteerValue(0);
    }

    private void EngineLogic()
    {
        var breakTorque = 0f;
        
        if (targetTorque == 0)
        {
            breakTorque = maxBrakeTorque;
        }

        currentBreakTorque = breakTorque;
        
        var delta = targetTorque != 0
            ? Mathf.Abs(targetTorque - currentTorque) / targetTorque
            : -currentTorque / maxTorque;

        var delta01 = Mathf.Clamp01(Mathf.Abs(delta));
        var deltaSign = Mathf.Sign(delta);
        var curveValue = steerCurve.Evaluate(delta);

        currentTorque += curveValue * deltaSign * delta01 * torqueStep;
        currentTorque = Mathf.Clamp(currentTorque, -maxTorque, maxTorque);
    }

    private void SteerLogic()
    {
        var cSteer = currentSteer;
        var delta = targetSteer != 0 ? Mathf.Abs(targetSteer - cSteer)/targetSteer : -cSteer/maxSteer;

        var delta01 = Mathf.Clamp01(Mathf.Abs(delta));
        var deltaSign = Mathf.Sign(delta);
        var curveValue = steerCurve.Evaluate(delta);
        
        currentSteer += curveValue * deltaSign * delta01 * steerStep;
        currentSteer = Mathf.Clamp(currentSteer, -maxSteer, maxSteer);
    }

    private void CarLogic()
    {
        foreach (var wheelElement in wheels)
        {
            if (wheelElement.Torque)
            {
                wheelElement.Wheel.motorTorque = currentTorque;
                wheelElement.Wheel.brakeTorque = currentBreakTorque;
            }
            
            if (wheelElement.Rotating)
            {
                wheelElement.Wheel.steerAngle = currentSteer;
            }

            Vector3 pos;
            Quaternion quat;

            wheelElement.Wheel.GetWorldPose(out pos, out quat);
            
            wheelElement.Model.position = pos;
            wheelElement.Model.rotation = quat;
        }
    }

    [System.Serializable]
    public class WheelElement
    {
        [SerializeField] private WheelCollider wheel;
        [SerializeField] private Transform model;
        [SerializeField] private bool rotating;
        [SerializeField] private bool torque;

        public WheelCollider Wheel => wheel;
        public Transform Model => model;
        public bool Rotating => rotating;
        public bool Torque => torque;
    }
}
