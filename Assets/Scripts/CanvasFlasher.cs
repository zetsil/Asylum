using UnityEngine;
using System.Collections;

public class CanvasFlasher : MonoBehaviour
{
    [Header("Flash Settings")]
    public Canvas targetCanvas;
    public float flashDuration = 1f;
    public int flashCount = 3;
    
    [Header("Advanced Settings")]
    public AnimationCurve fadeCurve = AnimationCurve.Linear(0, 0, 1, 1);
    
    private CanvasGroup canvasGroup;
    private bool isFlashing = false;
    private bool canvasWasEnabled;

    void Start()
    {
        // Auto-flash on start (remove if you want manual triggering only)
        // TriggerFlash();
    }

    public void TriggerFlash()
    {
        if (!isFlashing && targetCanvas != null)
        {
            StartCoroutine(FlashRoutine());
        }
    }

    private IEnumerator FlashRoutine()
    {
        isFlashing = true;
        
        // Get or add CanvasGroup
        canvasGroup = targetCanvas.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = targetCanvas.gameObject.AddComponent<CanvasGroup>();
        }

        // Store original state
        canvasWasEnabled = targetCanvas.enabled;
        targetCanvas.enabled = true;

        float flashInterval = flashDuration / flashCount;
        
        for (int i = 0; i < flashCount; i++)
        {
            // Fade in
            yield return FadeCanvas(0f, 1f, flashInterval/2);

            // Only fade out if not the last flash
            if (i < flashCount - 1)
            {
                yield return FadeCanvas(1f, 0f, flashInterval/2);
            }
        }

        // Final fade out to complete invisibility
        yield return FadeCanvas(1f, 0f, flashInterval/2);
        
        // Ensure canvas is completely hidden
        targetCanvas.enabled = false;
        isFlashing = false;
    }

    private IEnumerator FadeCanvas(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = fadeCurve.Evaluate(elapsed / duration);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }

        // Ensure exact final value
        canvasGroup.alpha = endAlpha;
    }

    void OnDisable()
    {
        // Clean up if disabled during flash
        if (isFlashing)
        {
            StopAllCoroutines();
            if (targetCanvas != null)
            {
                targetCanvas.enabled = false;
            }
            isFlashing = false;
        }
    }
}