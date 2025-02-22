using Ashsvp;

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SimcadeVehicleController))]
public class SimcadeVehicleControllerEditor : Editor
{
    
    private void OnEnable()
    {
        DrawDefaultInspector();
    }

}
