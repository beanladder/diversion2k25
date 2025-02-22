using Fusion;
using UnityEngine;

public class CameraManager : NetworkBehaviour
{
    [SerializeField] private GameObject cameraObject; // The Camera to be assigned

    private void Start()
    {
        if(!Object.HasStateAuthority){
            cameraObject.SetActive(false);
        }
    }
}
