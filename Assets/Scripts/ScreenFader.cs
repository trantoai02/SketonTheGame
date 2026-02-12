using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;

    [SerializeField] private Image fadeImage;
    [SerializeField] private float defaultFadeDuration = 0.5f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        if (fadeImage == null) fadeImage = GetComponent<Image>();
    }

    public IEnumerator FadeOut(float duration = -1f)
    {
        if (duration < 0) duration = defaultFadeDuration;

        Color c = fadeImage.color;
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Clamp01(t / duration);
            fadeImage.color = c;
            yield return null;
        }
        c.a = 1f;
        fadeImage.color = c;
    }

    public IEnumerator FadeIn(float duration = -1f)
    {
        if (duration < 0) duration = defaultFadeDuration;

        Color c = fadeImage.color;
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = 1f - Mathf.Clamp01(t / duration);
            fadeImage.color = c;
            yield return null;
        }
        c.a = 0f;
        fadeImage.color = c;
    }
}
