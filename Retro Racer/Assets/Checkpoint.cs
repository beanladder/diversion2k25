using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        CarController car = other.GetComponent<CarController>();
        if (car != null)
        {
            
            Debug.Log($"{car.gameObject.name} reached checkpoint {gameObject.name} at {transform.position}");
        }
    }
}
