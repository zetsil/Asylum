using UnityEngine;
using System.Collections;

public class WallScratchTracker : MonoBehaviour
{
    public int currentIndex = 0;
    public float fadeDuration = 2f; // durata fade-in
    public AnimationCurve fadeCurve; // pentru un efect mai "ciudat" și organic
    private Material mat;
    private bool hasFadedIn = false;

    void Start()
    {
        // Obținem materialul
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            mat = rend.material;
            Color c = mat.color;
            c.a = 0f;
            mat.color = c; // pornim complet transparent
        }

        gameObject.SetActive(false);

        int darkRoomCounter = GameStateManager.Instance.GetCounterDarkRoom();

        if (currentIndex < darkRoomCounter)
        {
            // Apare normal
            gameObject.SetActive(true);
            SetAlpha(1f);
        }
        else if (currentIndex == darkRoomCounter)
        {
            // Apare cu efect sinistru
            gameObject.SetActive(true);
            StartCoroutine(FadeInCreepy());
        }
    }

    private IEnumerator FadeInCreepy()
    {
        if (hasFadedIn || mat == null) yield break;
        hasFadedIn = true;

        float elapsed = 0f;

        // Poți adăuga un mic delay random pentru efect "ciudat"
        yield return new WaitForSeconds(Random.Range(0.2f, 1f));

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            float alpha = fadeCurve != null ? fadeCurve.Evaluate(t) : t;

            SetAlpha(alpha);

            // Adaugă o ușoară pulsație random, ca un efect "neregulat"
            transform.localScale = Vector3.one * (1f + Mathf.Sin(Time.time * 10f) * 0.02f);

            yield return null;
        }

        SetAlpha(1f);
    }

    private void SetAlpha(float a)
    {
        if (mat != null)
        {
            Color c = mat.color;
            c.a = a;
            mat.color = c;
        }
    }
}
