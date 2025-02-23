using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        CarController car = other.GetComponent<CarController>();
        if (car != null)
        {
            Debug.Log($"{car.gameObject.name} hit the death zone at {transform.position}");
            car.Respawn();
        }
    }
}
