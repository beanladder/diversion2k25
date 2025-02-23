using UnityEngine;

public class CarController : MonoBehaviour
{
    public Transform lastCheckpoint; // Stores the last crossed checkpoint

    private void Start()
    {
        lastCheckpoint = transform; // Default to the car's starting position
        Debug.Log($"{gameObject.name} initialized at {transform.position}");
    }

    private void OnTriggerEnter(Collider other)
    {
        // ✅ Checkpoint Logic
        if (other.CompareTag("Checkpoint"))
        {
            lastCheckpoint = other.transform;
            Debug.Log($"{gameObject.name} crossed checkpoint {other.gameObject.name} at {other.transform.position}");
        }

        // ☠ Death Zone Logic
        if (other.CompareTag("DeathZone"))
        {
            Debug.Log($"{gameObject.name} hit the death zone at {other.transform.position}");
            Respawn();
        }
    }

    public void Respawn()
    {
        if (lastCheckpoint != null)
        {
            Debug.Log($"{gameObject.name} respawning to last checkpoint at {lastCheckpoint.position}");
            transform.position = lastCheckpoint.position;
            transform.rotation = lastCheckpoint.rotation;
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero; // Stop momentum
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} has no checkpoint set! Respawn failed.");
        }
    }
}
