using UnityEngine;
using UnityEngine.UI;

public class BlinkEffect : MonoBehaviour
{
    public Text textToBlink;
    public float blinkSpeed = 1f;

    void Update()
    {
        if (textToBlink != null)
        {
            Color c = textToBlink.color;
            // Mathf.PingPong bounces the alpha value between 0 and 1
            c.a = Mathf.PingPong(Time.time * blinkSpeed, 1f);
            textToBlink.color = c;
        }
    }
}