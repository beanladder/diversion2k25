using UnityEngine;
using System.Runtime.InteropServices; // Required for JavaScript calls

public class DeviceCheck : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern int IsMobile(); // Import JavaScript function for WebGL detection

    public GameObject mobileCanvas; // Drag & Drop Mobile UI Canvas in Inspector
    public GameObject inputManagerObject; // Drag & Drop GameObject containing InputManager_SVP

    void Start()
    {
        bool isMobile = false;

#if UNITY_WEBGL && !UNITY_EDITOR
        // WebGL - Use JavaScript function
        isMobile = IsMobile() == 1;
        Debug.Log("Detected via JS (WebGL): " + (isMobile ? "Mobile" : "PC"));
#else
        // Normal Unity detection (Editor, Windows, Android, iOS)
        isMobile = SystemInfo.deviceType == DeviceType.Handheld;
        Debug.Log("Detected via Unity: " + (isMobile ? "Mobile" : "PC"));
#endif

        // Enable or disable the mobile UI
        if (mobileCanvas != null)
        {
            mobileCanvas.SetActive(isMobile);
            Debug.Log("Mobile canvas set to: " + isMobile);
        }
        else
        {
            Debug.LogWarning("Mobile canvas GameObject not assigned!");
        }

        // Ensure InputManager_SVP is manually assigned
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
