using UnityEngine;

public class LowFrameBreathing : MonoBehaviour
{
    [Header("Retro Vibe Settings")]
    [Tooltip("Target framerate for the animation. Lower = chunkier.")]
    public int frameRate = 12;

    [Header("Bobbing (Up & Down)")]
    public bool enableBobbing = true;
    public float bobAmplitude = 15f; // How far it moves (UI pixels or world units)
    public float bobSpeed = 1f;      // How fast one full loop takes

    [Header("Breathing (Scaling)")]
    public bool enableBreathing = false;
    public float breathAmplitude = 0.05f; // How much it expands (e.g., 0.05 is a 5% size change)
    public float breathSpeed = 0.5f;

    private Vector3 startPos;
    private Vector3 startScale;

    void Start()
    {
        // Remember where the object started so we don't accidentally move it across the screen
        startPos = transform.localPosition;
        startScale = transform.localScale;
    }

    void Update()
    {
        // The Magic Formula: This makes the time "snap" in steps instead of flowing smoothly
        float steppedTime = Mathf.Floor(Time.time * frameRate) / frameRate;

        if (enableBobbing)
        {
            // Use Sine waves to create a smooth looping value, but feed it our chunky time
            float offsetY = Mathf.Sin(steppedTime * bobSpeed * Mathf.PI * 2f) * bobAmplitude;
            transform.localPosition = startPos + new Vector3(0, offsetY, 0);
        }

        if (enableBreathing)
        {
            // Calculate a scale offset using the same choppy time
            float scaleOffset = Mathf.Sin(steppedTime * breathSpeed * Mathf.PI * 2f) * breathAmplitude;
            transform.localScale = startScale + new Vector3(scaleOffset, scaleOffset, scaleOffset);
        }
    }
}