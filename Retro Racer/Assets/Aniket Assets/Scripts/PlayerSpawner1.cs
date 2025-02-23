using Fusion;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject[] carPrefabs;
    //public GameObject PlayerPrefab;
    public Transform[] spawnPoints;
    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            int SelectedCarIndex = PlayerPrefs.GetInt("SelectedCarIndex",0);
            GameObject selectedCarPrefab;
            Transform spawnPoint;
            switch (SelectedCarIndex){
                case 0:
                    selectedCarPrefab = carPrefabs[0];
                    spawnPoint = spawnPoints[0];
                    break;
                case 1:
                    selectedCarPrefab = carPrefabs[1];
                    spawnPoint = spawnPoints[1];
                    break;
                case 2:
                    selectedCarPrefab = carPrefabs[2];
                    spawnPoint = spawnPoints[2];
                    break;
                case 3:
                    selectedCarPrefab = carPrefabs[3];
                    spawnPoint = spawnPoints[3];
                    break;
                default:
                    selectedCarPrefab = carPrefabs[0];
                    spawnPoint = spawnPoints[0];
                    break;
            }
            NetworkObject playerCar = Runner.Spawn(selectedCarPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
