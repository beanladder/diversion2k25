using UnityEngine;
using Fusion;
using System.Collections.Generic;
public class GameMode1 : NetworkBehaviour
{
    public Dictionary<NetworkId, float> carLapTimes = new Dictionary<NetworkId, float>();
    public void StoreTimes(NetworkId networkId, float lapTime){
        carLapTimes.Add(networkId,lapTime);
    }
    public void ShowDictionary(){
        foreach(var kvp in carLapTimes){
            Debug.Log($"Player Id {kvp.Key}, laptime {kvp.Value}");
        }
    }
}   
