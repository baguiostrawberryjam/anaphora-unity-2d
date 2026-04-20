using UnityEngine;

[RequireComponent(typeof(CanvasGroup))] // Automatically adds a CanvasGroup if missing
public class LightFlicker : MonoBehaviour
{
    [Header("Flicker Settings")]
    public float minAlpha = 0.6f;     // How dim the light gets (0 is invisible, 1 is fully visible)
    public float maxAlpha = 1.0f;     // How bright the light gets
    public float flickerSpeed = 15f;  // How fast it flickers

    private CanvasGroup canvasGroup;
    private float randomOffset;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        // Pick a random starting point for the noise so multiple 
        // flickering objects don't flicker in exact sync
        randomOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        if (canvasGroup != null)
        {
            // Calculate Perlin Noise based on time
            float noise = Mathf.PerlinNoise((Time.time + randomOffset) * flickerSpeed, 0f);

            // Map the noise (which is 0 to 1) to our min and max alpha values
            canvasGroup.alpha = Mathf.Lerp(minAlpha, maxAlpha, noise);
        }
    }
}