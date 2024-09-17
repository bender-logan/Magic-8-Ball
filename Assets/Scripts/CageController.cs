using UnityEngine;

public class CageController : MonoBehaviour
{
    [SerializeField] private float widthScale = 3;

    private float initScaleFactor;

    void Start()
    {
        initScaleFactor = transform.localScale.x;

        // Get the screen width and height
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Calculate the aspect ratio
        float aspectRatio = screenHeight / screenWidth;

        // Apply the aspect ratio to the scale of the cage
        // Assuming you want to adjust only the X and Y scale based on the aspect ratio
        transform.localScale = new Vector3(widthScale * initScaleFactor, widthScale * aspectRatio * initScaleFactor, initScaleFactor);
    }
}
