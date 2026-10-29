using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightPulse : MonoBehaviour
{
    public Light2D light2D;
    public float speed = 2f;
    public float minIntensity = 0.5f;
    public float maxIntensity = 1.2f;

    void Update()
    {
        float t = (Mathf.Sin(Time.time * speed) + 1f) / 2f;
        light2D.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
    }
}