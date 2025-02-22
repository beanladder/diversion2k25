using UnityEngine;
using System.Runtime.InteropServices; // Required for JavaScript calls

public class DeviceCheck : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern int IsMobile(); // Import JavaScript function

    public GameObject mobileCanvas;
    public GameObject inputManagerObject; // Drag & Drop the GameObject with InputManager_SVP in Inspector

    void Start()
    {
        bool isMobile;

        #if UNITY_WEBGL && !UNITY_EDITOR
        isMobile = IsMobile() == 1; // Call JavaScript function for WebGL
        #else
        isMobile = SystemInfo.deviceType == DeviceType.Handheld; // Fallback for non-WebGL
        #endif

        // Log detected platform
        Debug.Log("Detected Platform: " + (isMobile ? "Mobile" : "PC"));

        // Enable or disable mobile input canvas
        if (mobileCanvas != null)
        {
            mobileCanvas.SetActive(isMobile);
            Debug.Log("Mobile canvas set to: " + isMobile);
        }
        else
        {
            Debug.LogWarning("Mobile canvas GameObject not assigned!");
        }

        // Check if InputManager_SVP is assigned and found
        if (inputManagerObject != null)
        {
            var inputManager = inputManagerObject.GetComponent("InputManager_SVP");

            if (inputManager != null)
            {
                Debug.Log("InputManager_SVP script found on assigned GameObject!");

                var type = inputManager.GetType();
                var field = type.GetField("useMobileInput"); // Ensure this field exists

                if (field != null)
                {
                    field.SetValue(inputManager, isMobile);
                    Debug.Log("useMobileInput set to: " + isMobile);
                }
                else
                {
                    Debug.LogWarning("useMobileInput field not found in InputManager_SVP!");
                }
            }
            else
            {
                Debug.LogError("InputManager_SVP component NOT found on assigned GameObject!");
            }
        }
        else
        {
            Debug.LogError("InputManager GameObject not assigned! Drag it into the Inspector.");
        }
    }
}