using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TransitionController : MonoBehaviour
{
    public Image fadeImage;
    public Camera mainCamera;

    // --- Schimbă instant culoarea imaginii (menține alpha) ---
    public void SetFadeColor(Color color)
    {
        color.a = fadeImage.color.a;
        fadeImage.color = color;
    }

    // --- FLASH ALB ---
    public IEnumerator FlashWhite(float duration = 0.15f)
    {
        // 1. Setează imaginea complet albă, opacă
        fadeImage.color = new Color(1f, 1f, 1f, 1f);

        // 2. (Opțional) Redă sunetul de șoc/zap
        SoundManager.PlayEventSound("UI_Flash");

        // 3. Menține pentru o scurtă durată
        yield return new WaitForSeconds(duration);

        // 4. Lerp de la alb la negru
        float fadeOutDuration = 0.2f;
        Color startColor = fadeImage.color;              // Alb
        Color endColor = new Color(0f, 0f, 0f, 1f);      // Negru opac

        for (float t = 0; t < fadeOutDuration; t += Time.deltaTime)
        {
            fadeImage.color = Color.Lerp(startColor, endColor, t / fadeOutDuration);
            yield return null;
        }

        fadeImage.color = endColor; // Rămâne negru opac
    }

    // --- FADE IN: Transparent → Negru ---
    public IEnumerator FadeIn(float duration = 1f)
    {
        // Începe de la negru complet transparent (alpha 0)
        Color c = new Color(0f, 0f, 0f, 0f);
        fadeImage.color = c;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            // Crește alpha spre 1
            c.a = Mathf.Lerp(0f, 1f, t / duration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = 1f;
        fadeImage.color = c; // Negru complet
    }

    // --- FADE OUT: Negru → Transparent ---
    public IEnumerator FadeOut(float duration = 1f)
    {
        // Începe de la negru complet opac
        Color c = new Color(0f, 0f, 0f, 1f);
        fadeImage.color = c;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            // Scade alpha spre 0
            c.a = Mathf.Lerp(1f, 0f, t / duration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = 0f;
        fadeImage.color = c; // Transparent (scena vizibilă)
    }
}
