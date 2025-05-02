using UnityEngine;

public class ThunderFlash2D : MonoBehaviour
{
    [Header("Flash Settings")]
    public SpriteRenderer flashSprite; // Assign a white sprite (or UI Image)
    public float minFlashDelay = 5f;
    public float maxFlashDelay = 15f;
    public float flashDuration = 0.2f;
    public float maxBrightness = 0.8f; // Alpha value (0-1)

    [Header("Audio (Optional)")]
    public AudioClip thunderSound;
    private AudioSource audioSource;

    void Start()
    {
        // Get or add AudioSource (optional)
        if (thunderSound != null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) 
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // Initialize flash
        if (flashSprite != null)
        {
            flashSprite.color = new Color(1f, 1f, 1f, 0f); // Start invisible
        }

        // Trigger first flash
        Invoke("TriggerFlash", Random.Range(minFlashDelay, maxFlashDelay));
    }

    void TriggerFlash()
    {
        StartCoroutine(FlashRoutine());
        // Schedule next flash
        Invoke("TriggerFlash", Random.Range(minFlashDelay, maxFlashDelay));
    }

    System.Collections.IEnumerator FlashRoutine()
    {
        // Play sound (if available)
        if (thunderSound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f); // Slight pitch variation
            audioSource.PlayOneShot(thunderSound);
        }

        if (flashSprite == null) yield break; // Exit if no sprite

        // Quick flash ON
        flashSprite.color = new Color(1f, 1f, 1f, maxBrightness);
        yield return new WaitForSeconds(flashDuration);

        // Smooth fade OUT
        float fadeTime = 0.5f;
        float elapsed = 0f;
        while (elapsed < fadeTime)
        {
            float alpha = Mathf.Lerp(maxBrightness, 0f, elapsed / fadeTime);
            flashSprite.color = new Color(1f, 1f, 1f, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure fully invisible
        flashSprite.color = new Color(1f, 1f, 1f, 0f);
    }
}