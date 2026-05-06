using UnityEngine;
using UnityEngine.UI;

public class Glow : MonoBehaviour
{
    public Image glowImage;
    public float speed = 2.5f;
    public float minAlpha = 0.15f;
    public float maxAlpha = 0.45f;

    void Update()
    {
        float alpha = Mathf.Lerp(minAlpha, maxAlpha,
            (Mathf.Sin(Time.time * speed) + 1f) / 2f);

        Color c = glowImage.color;
        c.a = alpha;
        glowImage.color = c;
    }
}