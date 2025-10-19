using UnityEngine;
using System.Collections.Generic;

public class FireflyScatterManager : MonoBehaviour
{
    // Adaugă referința la toți licuricii din scenă care au scriptul FireflyOrbitTracker.
    [Header("Referință: Toți Licuricii de Scenă")]
    [Tooltip("Lista celor 5 GameObject-uri (licurici) cu scriptul FireflyOrbitTracker.")]
    public List<FireflyOrbitTracker> trackedFireflies;

    [Header("Parametrii de Fuga (Scatter)")]
    [Tooltip("Viteza cu care licuricii fug din poziția centrală.")]
    public float scatterSpeed = 5f;

    [Tooltip("Cât de repede își reduc scara și dispar.")]
    public float fadeOutDuration = 1.5f;

    [Tooltip("Cât timp durează mișcarea haotică înainte de a începe să dispară.")]
    public float scatterDuration = 1.0f;

    private bool hasScattered = false;
    private float scatterTimer = 0f;
    private float fadeTimer = 0f;

    // Poziția centrală de unde vor fugi licuricii.
    private Vector3 scatterCenter;

    // --- Integrează cu Game State ---

    void Start()
    {
        // Ne asigurăm că există GameStateManager și că lista nu este goală.
        if (GameStateManager.Instance == null)
        {
            Debug.LogError("FATAL: GameStateManager nu a fost găsit.");
            return;
        }

        if (trackedFireflies == null || trackedFireflies.Count == 0)
        {
            Debug.LogError("Lista FireflyScatterManager este goală. Adaugă referințele!");
        }

        // Verifică starea inițială. Dacă puzzle-ul e deja rezolvat la start (Load Game), dezactivează-te.
        // if (GameStateManager.Instance.isDarkRoomPuzzleResolved)
        // {
        //     // Dacă se încarcă un joc salvat, nu mai trebuie să fugă. Pur și simplu îi ascundem.
        //     foreach (var firefly in trackedFireflies)
        //     {
        //         if (firefly != null) firefly.gameObject.SetActive(false);
        //     }
        //     gameObject.SetActive(false); // Dezactivează managerul.
        // }
    }

    void Update()
    {
        if (GameStateManager.Instance.isDarkRoomPuzzleResolved && !hasScattered)
        {
            // Acesta este trigger-ul: Puzzle-ul tocmai a fost rezolvat.
            InitializeScatter();
        }

        if (hasScattered)
        {
            HandleScatterMovement();
        }
    }

    // --- Logica de Scatter ---

    private void InitializeScatter()
    {
        // 1. Setează Centrul de fugă (poziția managerului sau a țintei licuriciului)
        scatterCenter = transform.position;

        // 2. Oprește scripturile de orbită pentru fiecare licurici
        foreach (var firefly in trackedFireflies)
        {
            if (firefly != null)
            {
                // Dezactivează scriptul FireflyOrbitTracker
                firefly.DisableOrbitAndPrepareForScatter();

                // Dă-le o direcție aleatorie de fugă, stocată în local position
                // (Vom folosi localPosition pentru a stoca Vectorul de fugă)
                Vector3 direction = (firefly.transform.position - scatterCenter).normalized;
                firefly.transform.localPosition = direction; 
            }
        }
        
        hasScattered = true;
        scatterTimer = scatterDuration;
    }

    private void HandleScatterMovement()
    {
        // 1. Mișcarea haotică (Scatter)
        if (scatterTimer > 0)
        {
            scatterTimer -= Time.deltaTime;

            foreach (var firefly in trackedFireflies)
            {
                if (firefly == null || !firefly.gameObject.activeSelf) continue;

                // localPosition este folosit temporar pentru a stoca vectorul de direcție
                Vector3 direction = firefly.transform.localPosition; 
                
                // Aplică viteza de fugă
                firefly.transform.position += direction * scatterSpeed * Time.deltaTime;
            }
        }
        // 2. Faza de dispariție (Fade Out)
        else
        {
            fadeTimer += Time.deltaTime;
            float t = fadeTimer / fadeOutDuration;
            
            foreach (var firefly in trackedFireflies)
            {
                if (firefly == null || !firefly.gameObject.activeSelf) continue;
                
                // Redu scara (Scale) spre zero
                firefly.transform.localScale = Vector3.Lerp(firefly.initialScale, Vector3.zero, t);
                
                // Fă-i să se miște puțin în continuare
                Vector3 direction = firefly.transform.localPosition; 
                firefly.transform.position += direction * (scatterSpeed * 0.5f) * Time.deltaTime; // Viteză redusă

                if (t >= 1f)
                {
                    // La finalul fading-ului, dezactivează complet obiectul.
                    firefly.gameObject.SetActive(false);
                }
            }
            
            // Oprește managerul după ce toți au dispărut
            if (t >= 1f)
            {
                gameObject.SetActive(false);
            }
        }
    }
}