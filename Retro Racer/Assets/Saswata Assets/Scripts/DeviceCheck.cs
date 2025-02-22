using UnityEngine;
using Fusion;

public class DeviceCheck : NetworkBehaviour
{
    public GameObject mobileCanvas;
    public GameObject inputManagerObject;

    // Store useMobileInput per player
    [Networked] public bool useMobileInput { get; set; }

    void Start()
    {
        if (!HasStateAuthority) return; // Only execute for the local player

        bool isMobile = false;

        #if UNITY_WEBGL && !UNITY_EDITOR
        isMobile = IsMobile() == 1; // Call JavaScript function for WebGL
        Debug.Log("Detected via JS (WebGL): " + (isMobile ? "Mobile" : "PC"));
        #else
        isMobile = SystemInfo.deviceType == DeviceType.Handheld;
        Debug.Log("Detected via Unity: " + (isMobile ? "Mobile" : "PC"));
        #endif

        // Store input setting per player
        useMobileInput = isMobile;

        // Apply UI changes only for the local player
        if (mobileCanvas != null)
        {
            mobileCanvas.SetActive(useMobileInput);
            Debug.Log("Mobile canvas set to: " + useMobileInput);
        }
        else
        {
            Debug.LogWarning("Mobile canvas not assigned!");
        }

        // Ensure InputManager_SVP is assigned and updated
        if (inputManagerObject != null)
        {
            var inputManager = inputManagerObject.GetComponent("InputManager_SVP");

            if (inputManager != null)
            {
                var type = inputManager.GetType();
                var field = type.GetField("useMobileInput");

                if (field != null)
                {
                    field.SetValue(inputManager, useMobileInput);
                    Debug.Log("useMobileInput set to: " + useMobileInput);
                }
                else
                {
                    Debug.LogWarning("useMobileInput field not found in InputManager_SVP!");
                }
            }
            else
            {
                Debug.LogError("InputManager_SVP component NOT found!");
            }
        }
        else
        {
            Debug.LogError("InputManager GameObject not assigned!");
        }
    }
}