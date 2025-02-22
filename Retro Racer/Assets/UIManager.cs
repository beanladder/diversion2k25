using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject UIScreen;
    public GameObject CarSelectionUI;
    public void HideUI(){
        UIScreen.SetActive(false);
        CarSelectionUI.SetActive(true);
    }
}
