using UnityEngine;

public class CarInput : MonoBehaviour
{
    [SerializeField] private CarController carController;

    private void Update()
    {
        var vertical = Input.GetAxis("Vertical");
        var horizontal = Input.GetAxis("Horizontal");

        carController.SetEngineValue(vertical);
        carController.SetSteerValue(horizontal);
    }
}
