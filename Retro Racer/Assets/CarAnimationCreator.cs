using UnityEngine;

public class CarAnimationCreator : MonoBehaviour
{
    // References to the car and its wheels.
    public GameObject car;
    public Transform frontLeftWheel;
    public Transform frontRightWheel;
    public Transform rearLeftWheel;
    public Transform rearRightWheel;

    // Timing and distance values.
    public float driveDuration = 5f;
    public float decelerationDuration = 1f;
    public float driveDistance = 10f; // Distance moved during the forward phase.

    // Steering angles for the front wheels.
    public float rightTurnAngle = 30f;  // At driveDuration, wheels are turned right.
    public float leftTurnAngle = -30f;  // At the end of deceleration, wheels turn left.

    // Car tilt values (rotation around the X-axis).
    public float carTiltForward = -5f;  // Tilt forward (nosing down) when moving.
    public float carTiltStop = 5f;      // Tilt back (nosing up) when stopping.

    // Wheel roll (rotation around the X-axis) in degrees.
    // This simulates the wheels spinning as the car moves.
    public float wheelRollDegrees = 720f; // Two full rotations during the forward phase.

    void Start()
    {
        // Create a new AnimationClip.
        AnimationClip clip = new AnimationClip();
        clip.frameRate = 60; // Higher frame rate for smoother interpolation.

        // === Animate the Car's Forward Movement ===
        Vector3 startPos = car.transform.localPosition;
        Vector3 drivePos = startPos + new Vector3(0, 0, driveDistance);
        Keyframe[] posZKeys = new Keyframe[3];
        posZKeys[0] = new Keyframe(0f, startPos.z);
        posZKeys[1] = new Keyframe(driveDuration, drivePos.z);
        posZKeys[2] = new Keyframe(driveDuration + decelerationDuration, drivePos.z);
        AnimationCurve posZCurve = new AnimationCurve(posZKeys);
        posZCurve.SmoothTangents(1, 0.5f);
        clip.SetCurve("", typeof(Transform), "localPosition.z", posZCurve);

        // === Animate the Car's Tilt (Rotation around X-axis) ===
        Keyframe[] carTiltKeys = new Keyframe[3];
        carTiltKeys[0] = new Keyframe(0f, 0f); // Start with no tilt.
        carTiltKeys[1] = new Keyframe(driveDuration, carTiltForward); // Tilt forward when moving.
        carTiltKeys[2] = new Keyframe(driveDuration + decelerationDuration, carTiltStop); // Tilt back when stopping.
        AnimationCurve carTiltCurve = new AnimationCurve(carTiltKeys);
        carTiltCurve.SmoothTangents(1, 0.5f);
        clip.SetCurve("", typeof(Transform), "localEulerAngles.x", carTiltCurve);

        // === Animate the Front Wheels' Steering (Rotation around Y-axis) ===
        Keyframe[] wheelTurnKeys = new Keyframe[3];
        wheelTurnKeys[0] = new Keyframe(0f, 0f); // Wheels start straight.
        wheelTurnKeys[1] = new Keyframe(driveDuration, rightTurnAngle); // Turn right while driving.
        wheelTurnKeys[2] = new Keyframe(driveDuration + decelerationDuration, leftTurnAngle); // Turn left when stopping.
        AnimationCurve frontWheelTurnCurve = new AnimationCurve(wheelTurnKeys);
        frontWheelTurnCurve.SmoothTangents(1, 0.5f);
        clip.SetCurve(frontLeftWheel.name, typeof(Transform), "localEulerAngles.y", frontWheelTurnCurve);
        clip.SetCurve(frontRightWheel.name, typeof(Transform), "localEulerAngles.y", frontWheelTurnCurve);

        // === Animate the Rear Wheels' Steering (Stay Straight) ===
        Keyframe[] rearTurnKeys = new Keyframe[2];
        rearTurnKeys[0] = new Keyframe(0f, 0f);
        rearTurnKeys[1] = new Keyframe(driveDuration + decelerationDuration, 0f);
        AnimationCurve rearWheelTurnCurve = new AnimationCurve(rearTurnKeys);
        clip.SetCurve(rearLeftWheel.name, typeof(Transform), "localEulerAngles.y", rearWheelTurnCurve);
        clip.SetCurve(rearRightWheel.name, typeof(Transform), "localEulerAngles.y", rearWheelTurnCurve);

        // === Animate the Wheels' Rolling (Rotation around X-axis) ===
        // Both front and rear wheels roll as the car moves.
        Keyframe[] wheelRollKeys = new Keyframe[3];
        wheelRollKeys[0] = new Keyframe(0f, 0f);
        wheelRollKeys[1] = new Keyframe(driveDuration, wheelRollDegrees);
        // Once the car stops, the wheels stop rolling.
        wheelRollKeys[2] = new Keyframe(driveDuration + decelerationDuration, wheelRollDegrees);
        AnimationCurve wheelRollCurve = new AnimationCurve(wheelRollKeys);
        wheelRollCurve.SmoothTangents(1, 0.5f);
        // Front wheels rolling.
        clip.SetCurve(frontLeftWheel.name, typeof(Transform), "localEulerAngles.x", wheelRollCurve);
        clip.SetCurve(frontRightWheel.name, typeof(Transform), "localEulerAngles.x", wheelRollCurve);
        // Rear wheels rolling.
        clip.SetCurve(rearLeftWheel.name, typeof(Transform), "localEulerAngles.x", wheelRollCurve);
        clip.SetCurve(rearRightWheel.name, typeof(Transform), "localEulerAngles.x", wheelRollCurve);

        // === Attach and Play the Animation ===
        Animation anim = car.GetComponent<Animation>();
        if (anim == null)
        {
            anim = car.AddComponent<Animation>();
        }
        anim.AddClip(clip, "CarAnim");
        anim.Play("CarAnim");
    }
}
