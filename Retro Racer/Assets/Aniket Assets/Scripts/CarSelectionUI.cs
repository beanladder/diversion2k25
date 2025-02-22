using UnityEngine;
using UnityEngine.SceneManagement;

public class CarSelectionUI : MonoBehaviour
{
    public GameObject[] carPrefabs; // Array of car prefabs
    public Transform spawnPoint;
    private GameObject displayCar;
    private int currentIndex = 0;

    private void Start()
    {
        // Load previously selected car index or default to 0
        //currentIndex = PlayerPrefs.GetInt("SelectedCarIndex", 0);
        ShowCar(currentIndex); // ✅ Show the first car on start
    }

    public void NextCar()
    {
        if(currentIndex >= carPrefabs.Length-1){
            currentIndex = 0;
        }
        else{
            currentIndex++;
        }
        ShowCar(currentIndex);
    }

    public void PrevCar()
    {
        if( currentIndex == 0){
            currentIndex = carPrefabs.Length-1;
        }
        else{
            currentIndex--;
        }
        ShowCar(currentIndex);
    }

    public void SelectCar()
    {
        PlayerPrefs.SetInt("SelectedCarIndex", currentIndex);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Multiplayer");
    }

    private void ShowCar(int index)
    {
        if (displayCar != null)
        {
            Destroy(displayCar);
        }

        displayCar = Instantiate(carPrefabs[index], spawnPoint.position, spawnPoint.rotation);
    }
}
