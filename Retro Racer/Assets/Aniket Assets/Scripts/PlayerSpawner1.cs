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
            GameObject selectedCarPrefab = carPrefabs[SelectedCarIndex];
            Transform spawnPoint = spawnPoints[0]; // You can adjust this logic to select a spawn point
            NetworkObject playerCar = Runner.Spawn(selectedCarPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
