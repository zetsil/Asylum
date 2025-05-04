using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TransitionController : MonoBehaviour
{
    public Image fadeImage; // Assign in inspector
    public Camera mainCamera;

    public IEnumerator FadeIn(float duration = 1f)
    {
        Color c = fadeImage.color;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            c.a = Mathf.Lerp(0f, 1f, t / duration);
            fadeImage.color = c;
            yield return null;
        }
        c.a = 1f;
        fadeImage.color = c;
    }

    public IEnumerator FadeOut(float duration = 1f)
    {
        Color c = fadeImage.color;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            c.a = Mathf.Lerp(1f, 0f, t / duration);
            fadeImage.color = c;
            yield return null;
        }
        c.a = 0f;
        fadeImage.color = c;
    }
}
