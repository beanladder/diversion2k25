using UnityEngine;

public class CarAnimationLT : MonoBehaviour
{
    [Header("References")]
    // The car transform (if not set, the script’s GameObject will be used)
    public Transform car;
    // Wheel transforms (ensure these are assigned)
    public Transform frontLeftWheel;
    public Transform frontRightWheel;
    public Transform rearLeftWheel;
    public Transform rearRightWheel;

    [Header("Animation Durations")]
    public float driveDuration = 5f;         // Time for drive phase
    public float decelerationDuration = 1f;  // Time for deceleration/stopping phase

    [Header("Movement & Tilt Settings")]
    public float driveDistance = 10f;        // Distance the car moves forward
    public float carTiltForward = -5f;       // Car tilt (X-axis) while driving
    public float carTiltStop = 5f;           // Car tilt (X-axis) when stopping

    [Header("Wheel Settings")]
    public float rightTurnAngle = 30f;       // Front wheel Y-axis rotation during drive phase
    public float leftTurnAngle = -30f;       // Front wheel Y-axis rotation during deceleration
    public float wheelRollDegrees = 720f;    // Total degrees wheels roll during drive phase

    // Internal values to combine tweened values for wheels.
    private float frontWheelSteer = 0f;
    private float frontWheelRoll = 0f;
    private float rearWheelRoll = 0f;

    void Start()
    {
        // If the car reference isn’t set, use this GameObject.
        if (car == null)
            car = transform;

        // --- Car Movement & Tilt ---
        Vector3 startPos = car.localPosition;
        Vector3 endPos = startPos + new Vector3(0, 0, driveDistance);

        // Move the car forward over driveDuration.
        LeanTween.moveLocalZ(car.gameObject, endPos.z, driveDuration)
                 .setEase(LeanTweenType.easeInOutSine);

        // Tilt the car forward during the drive phase.
        LeanTween.rotateLocal(car.gameObject, new Vector3(carTiltForward, 0, 0), driveDuration)
                 .setEase(LeanTweenType.easeInOutSine)
                 .setOnComplete(() => {
                     // When drive phase ends, tilt the car into its stopping pose.
                     LeanTween.rotateLocal(car.gameObject, new Vector3(carTiltStop, 0, 0), decelerationDuration)
                              .setEase(LeanTweenType.easeInOutSine);
                 });

        // --- Front Wheels: Steering and Rolling ---
        // Animate steering: from 0 to rightTurnAngle during drive.
        LeanTween.value(gameObject, 0f, rightTurnAngle, driveDuration)
                 .setEase(LeanTweenType.easeInOutSine)
                 .setOnUpdate((float val) => {
                     frontWheelSteer = val;
                     UpdateFrontWheels();
                 })
                 .setOnComplete(() => {
                     // During deceleration, change steering from rightTurnAngle to leftTurnAngle.
                     LeanTween.value(gameObject, frontWheelSteer, leftTurnAngle, decelerationDuration)
                              .setEase(LeanTweenType.easeInOutSine)
                              .setOnUpdate((float val) => {
                                  frontWheelSteer = val;
                                  UpdateFrontWheels();
                              });
                 });

        // Animate front wheels rolling: from 0 to wheelRollDegrees during drive.
        LeanTween.value(gameObject, 0f, wheelRollDegrees, driveDuration)
                 .setEase(LeanTweenType.linear)
                 .setOnUpdate((float val) => {
                     frontWheelRoll = val;
                     UpdateFrontWheels();
                 });

        // --- Rear Wheels: Only Rolling (no steering) ---
        LeanTween.value(gameObject, 0f, wheelRollDegrees, driveDuration)
                 .setEase(LeanTweenType.linear)
                 .setOnUpdate((float val) => {
                     rearWheelRoll = val;
                     UpdateRearWheels();
                 });
    }

    // Combines front wheel rolling (X axis) and steering (Y axis).
    void UpdateFrontWheels()
    {
        // Create a combined rotation: rolling on X and steering on Y.
        // (Z axis is left at 0; adjust if needed.)
        Vector3 combinedRotation = new Vector3(frontWheelRoll, frontWheelSteer, 0);
        if (frontLeftWheel != null)
            frontLeftWheel.localEulerAngles = combinedRotation;
        if (frontRightWheel != null)
            frontRightWheel.localEulerAngles = combinedRotation;
    }

    // Updates rear wheels with rolling only.
    void UpdateRearWheels()
    {
        Vector3 rearRotation = new Vector3(rearWheelRoll, 0, 0);
        if (rearLeftWheel != null)
            rearLeftWheel.localEulerAngles = rearRotation;
        if (rearRightWheel != null)
            rearRightWheel.localEulerAngles = rearRotation;
    }
}
