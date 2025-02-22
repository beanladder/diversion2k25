using UnityEngine;
using UnityEngine.UI;

public class ScrollBackground : MonoBehaviour
{
    public RawImage backgroundImage;
    public float scrollSpeedX = 0.1f;
    public float scrollSpeedY = 0.1f;
    public Vector2 tiling = new Vector2(5, 5); // Increases the repetition

    void Start()
    {
        // Set the tiling so the texture repeats
        backgroundImage.uvRect = new Rect(0, 0, tiling.x, tiling.y);
    }

    void Update()
    {
        // Scroll the tiled texture infinitely
        backgroundImage.uvRect = new Rect(
            backgroundImage.uvRect.x + scrollSpeedX * Time.deltaTime,
            backgroundImage.uvRect.y + scrollSpeedY * Time.deltaTime,
            tiling.x,
            tiling.y
        );
    }
}
