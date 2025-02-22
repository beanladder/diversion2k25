using UnityEngine;
using UnityEngine.EventSystems;

public class SteeringWheel : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform wheel; // Assign the Steering Wheel UI image
    private float wheelAngle = 0f; // Current rotation
    private float lastWheelAngle = 0f; // Previous rotation
    private bool wheelBeingHeld = false;

    [Range(0.1f, 5f)]
    public float maxSteerAngle = 200f; // Maximum steering angle

    [Range(1f, 20f)]
    public float returnSpeed = 5f; // Speed at which the wheel returns to center

    public float SteeringInput { get; private set; } // Output steer input (-1 to 1)

    private void Update()
    {
        if (!wheelBeingHeld)
        {
            // Gradually return to center when released
            wheelAngle = Mathf.Lerp(wheelAngle, 0, returnSpeed * Time.deltaTime);
        }

        // Normalize the steering input
        SteeringInput = Mathf.Clamp(wheelAngle / maxSteerAngle, -1f, 1f);

        // Apply rotation to UI element
        wheel.localEulerAngles = new Vector3(0, 0, -wheelAngle);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        wheelBeingHeld = true;
        lastWheelAngle = GetAngle(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        float newAngle = GetAngle(eventData.position);
        float deltaAngle = newAngle - lastWheelAngle;

        // Prevent angle flipping
        if (deltaAngle > 180) deltaAngle -= 360;
        if (deltaAngle < -180) deltaAngle += 360;

        // Update wheel rotation
        wheelAngle = Mathf.Clamp(wheelAngle + deltaAngle, -maxSteerAngle, maxSteerAngle);
        lastWheelAngle = newAngle;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        wheelBeingHeld = false;
    }

    private float GetAngle(Vector2 position)
    {
        Vector2 dir = position - (Vector2)wheel.position;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }
}
