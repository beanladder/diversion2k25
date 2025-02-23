using UnityEngine;
using UnityEngine.SceneManagement;

public class CarAnimationLT : MonoBehaviour
{
    [Header("References")]
    public Transform car;                       // Car transform; if null, the script's GameObject is used.
    public Transform frontLeftWheel;
    public Transform frontRightWheel;
    public Transform rearLeftWheel;
    public Transform rearRightWheel;
    
    [Header("Animation Durations")]
    public float driveDuration = 5f;            // Time for drive phase.
    public float decelerationDuration = 1f;     // Time for deceleration/stopping phase.
    
    [Header("Movement & Tilt Settings")]
    public float driveDistance = 10f;           // Distance the car moves forward.
    public float carTiltForward = -5f;          // Car tilts (around X) while driving.
    public float carTiltStop = 5f;              // Car tilts (around X) when stopping.
    
    [Header("Wheel Settings")]
    public float rightTurnAngle = 30f;          // Front wheels turn right during drive.
    public float leftTurnAngle = -30f;          // Front wheels turn left during deceleration.
    public float wheelRollDegrees = 720f;       // Wheels roll this many degrees during drive.
    
    [Header("UI Settings")]
    public GameObject popInButton;              // UI Button that pops in near the end (initially inactive).
    
    [Header("UI Fade Image")]
    public CanvasGroup fadeImageGroup;          // UI image with CanvasGroup; initial alpha should be 0.
    public float fadeImageDuration = 1f;        // Duration over which the image fades in.
    
    // Internal variables for wheel animation.
    private float frontWheelSteer = 0f;
    private float frontWheelRoll = 0f;
    private float rearWheelRoll = 0f;
    
    void Start()
    {
        if (car == null)
            car = transform;
        
        // Ensure the pop-in button is hidden initially.
        if (popInButton != null)
            popInButton.SetActive(false);
        
        // Ensure the fade image is hidden initially.
        if (fadeImageGroup != null)
            fadeImageGroup.alpha = 0f;
        
        // --- Car Movement & Tilt ---
        Vector3 startPos = car.localPosition;
        Vector3 endPos = startPos + new Vector3(0, 0, driveDistance);
        
        // Move the car forward over driveDuration.
        LeanTween.moveLocalZ(car.gameObject, endPos.z, driveDuration)
                 .setEase(LeanTweenType.easeInOutSine);
        
        // Tilt the car forward during the drive phase.
        LeanTween.rotateLocal(car.gameObject, new Vector3(carTiltForward, 0, 0), driveDuration)
                 .setEase(LeanTweenType.easeInOutSine)
                 .setOnComplete(() =>
                 {
                     // During deceleration, tilt the car into its stopping pose.
                     LeanTween.rotateLocal(car.gameObject, new Vector3(carTiltStop, 0, 0), decelerationDuration)
                              .setEase(LeanTweenType.easeInOutSine);
                 });
        
        // --- Front Wheels: Steering & Rolling ---
        LeanTween.value(gameObject, 0f, rightTurnAngle, driveDuration)
                 .setEase(LeanTweenType.easeInOutSine)
                 .setOnUpdate((float val) =>
                 {
                     frontWheelSteer = val;
                     UpdateFrontWheels();
                 })
                 .setOnComplete(() =>
                 {
                     // Change front wheel steering during deceleration.
                     LeanTween.value(gameObject, frontWheelSteer, leftTurnAngle, decelerationDuration)
                              .setEase(LeanTweenType.easeInOutSine)
                              .setOnUpdate((float val) =>
                              {
                                  frontWheelSteer = val;
                                  UpdateFrontWheels();
                              });
                 });
        
        LeanTween.value(gameObject, 0f, wheelRollDegrees, driveDuration)
                 .setEase(LeanTweenType.linear)
                 .setOnUpdate((float val) =>
                 {
                     frontWheelRoll = val;
                     UpdateFrontWheels();
                 });
        
        // --- Rear Wheels: Rolling Only ---
        LeanTween.value(gameObject, 0f, wheelRollDegrees, driveDuration)
                 .setEase(LeanTweenType.linear)
                 .setOnUpdate((float val) =>
                 {
                     rearWheelRoll = val;
                     UpdateRearWheels();
                 });
        
        // --- UI Button: Pop in 10ms before the car animation ends ---
        float totalAnimationTime = driveDuration + decelerationDuration;
        LeanTween.delayedCall(totalAnimationTime - 0.01f, ShowButton);
        
        // --- UI Fade Image: Start fade so that it finishes 50ms before the car animation ends ---
        if (fadeImageGroup != null)
        {
            // Calculate the delay so the fade finishes 50ms before totalAnimationTime.
            float fadeStartTime = totalAnimationTime - 0.05f - fadeImageDuration;
            LeanTween.delayedCall(fadeStartTime, () =>
            {
                LeanTween.alphaCanvas(fadeImageGroup, 1f, fadeImageDuration)
                         .setEase(LeanTweenType.linear);
            });
        }
    }
    
    // Update front wheels by combining steering (Y rotation) and rolling (X rotation).
    void UpdateFrontWheels()
    {
        Vector3 combinedRotation = new Vector3(frontWheelRoll, frontWheelSteer, 0);
        if (frontLeftWheel != null)
            frontLeftWheel.localEulerAngles = combinedRotation;
        if (frontRightWheel != null)
            frontRightWheel.localEulerAngles = combinedRotation;
    }
    
    // Update rear wheels with rolling only.
    void UpdateRearWheels()
    {
        Vector3 rearRotation = new Vector3(rearWheelRoll, 0, 0);
        if (rearLeftWheel != null)
            rearLeftWheel.localEulerAngles = rearRotation;
        if (rearRightWheel != null)
            rearRightWheel.localEulerAngles = rearRotation;
    }
    
    // Called 10ms before the car animation ends.
    // Activates the pop-in button and animates its scale.
    void ShowButton()
    {
        if (popInButton != null)
        {
            popInButton.SetActive(true);
            popInButton.transform.localScale = Vector3.zero;
            LeanTween.scale(popInButton, Vector3.one, 0.3f)
                     .setEase(LeanTweenType.easeOutBack);
        }
    }
}
