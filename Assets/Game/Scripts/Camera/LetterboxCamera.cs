using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LetterboxCamera : MonoBehaviour
{
    // 16:9 ratio (1920x1080)
    public float targetAspectX = 16f;
    public float targetAspectY = 9f;

    void Start()
    {
        float targetAspect = targetAspectX / targetAspectY;
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        Camera cam = GetComponent<Camera>();

        // If the current screen is wider than 16:9 (Modern phones)
        if (scaleHeight < 1.0f)
        {
            Rect rect = cam.rect;
            rect.width = scaleHeight;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleHeight) / 2.0f;
            rect.y = 0;
            cam.rect = rect;
        }
        else // If the screen is taller (like an iPad)
        {
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = cam.rect;
            rect.width = 1.0f;
            rect.height = scaleWidth;
            rect.x = 0;
            rect.y = (1.0f - scaleWidth) / 2.0f;
            cam.rect = rect;
        }
    }
}