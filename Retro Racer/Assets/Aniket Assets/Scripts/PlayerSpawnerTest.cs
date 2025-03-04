using UnityEngine;
using Fusion;

public class PlayerSpawnerTest : SimulationBehaviour, IPlayerJoined
{
    public NetworkTransform[] SpawnPoints=new NetworkTransform[4];
    [Networked]
    public NetworkTransform spawnPoint {get;set;}
    public GameObject[] carPrefabs;
    [Networked, Capacity(4)]public NetworkArray<bool> OccupiedPoints{get;set;}
    int availablespawnpoint;
    public void PlayerJoined(PlayerRef player){
        if(player==Runner.LocalPlayer){
            Debug.Log("Player joined, attempting to spawn");
            int SelectedCarIndex = PlayerPrefs.GetInt("SelectedCarIndex",0);
            GameObject selectedCarPrefab = carPrefabs[SelectedCarIndex];
            CheckAvailablePoint();
            Debug.Log($"Spawnpoint returned is {availablespawnpoint}");
;           spawnPoint = SpawnPoints[availablespawnpoint];
            NetworkObject playerCar = Runner.Spawn(selectedCarPrefab,spawnPoint.transform.position, spawnPoint.transform.rotation);
            OccupiedPoints.Set(availablespawnpoint,true);
        }
    }
    public void CheckAvailablePoint(){
        Debug.Log($"Check method called");
        for(int i=0;i<OccupiedPoints.Length;i++){
            bool isOccupied = OccupiedPoints.Get(i);
            Debug.Log($"SpawnPoint {i} is {isOccupied}");
            if(!isOccupied){
                availablespawnpoint = i;
            }
        }
    }
}