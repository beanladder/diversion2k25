using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransitionButton : MonoBehaviour
{
    [Header("Scene Transition Settings")]
    // Name of the scene to load. Ensure the scene is added to your Build Settings.
    public string sceneToLoad = "NextScene";
    // Duration for the scale tween.
    public float scaleDuration = 0.3f;
    // The target scale when the button is clicked (pop in effect).
    public Vector3 targetScale = new Vector3(1.2f, 1.2f, 1);

    private Button btn;
    private AsyncOperation asyncOperation;

    void Start()
    {
        btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(OnButtonClicked);
        }

        // Preload the scene asynchronously and prevent immediate activation.
        asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);
        asyncOperation.allowSceneActivation = false;
    }

    void OnButtonClicked()
    {
        // Disable further clicks.
        btn.interactable = false;

        // Animate the button scaling up.
        LeanTween.scale(gameObject, targetScale, scaleDuration)
                 .setEase(LeanTweenType.easeOutBack)
                 .setOnComplete(() =>
                 {
                     // Activate the preloaded scene immediately after the animation ends.
                     asyncOperation.allowSceneActivation = true;
                 });
    }
}
