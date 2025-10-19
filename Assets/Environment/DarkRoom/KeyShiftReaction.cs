using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KeyShiftReaction : MonoBehaviour, IObserver
{
    [Header("Reacție la SHIFT-uri")]
    [SerializeField] private float pullDistance = 1.5f;      // cât de departe se mișcă spre stânga la fiecare shift
    [SerializeField] private float pullSpeed = 6f;           // cât de repede e trasă
    [SerializeField] private float shakeIntensity = 0.05f;   // cât de mult vibrează în timpul tragerii

    [SerializeField] private string[] shiftMessages = { "Shift1", "Shift2", "Shift3" }; // cele 3 semnale permise

    private HashSet<string> usedShifts = new HashSet<string>(); // pentru a evita duplicatele

    private Emitter playerEmitter;
    private Vector3 originalPosition;
    private Coroutine currentRoutine;
    private bool isBeingPulled = false;

    void Start()
    {
        originalPosition = transform.localPosition;

        // Găsim playerul și ne abonăm ca observer
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerEmitter = player.GetComponent<Emitter>();
            if (playerEmitter != null)
                playerEmitter.AddObserver(this);
            else
                Debug.LogError("KeyShiftReaction: Player found, but missing Emitter component!");
        }
        else
        {
            Debug.LogError("KeyShiftReaction: Player not found in scene!");
        }
    }

    public void HandleEvent(string message)
    {
        // ignoră semnale necunoscute
        if (System.Array.IndexOf(shiftMessages, message) == -1)
            return;

        // ignoră dacă shiftul ăsta s-a mai întâmplat
        if (usedShifts.Contains(message))
            return;

        usedShifts.Add(message); // marchează shiftul ca folosit

        // pornește efectul
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(PullLeftEffect());
    }

    private IEnumerator PullLeftEffect()
    {
        isBeingPulled = true;

        Vector3 startPos = transform.localPosition;
        Vector3 targetPos = startPos + Vector3.left * pullDistance;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * pullSpeed;

            Vector3 shake = new Vector3(
                Random.Range(-shakeIntensity, shakeIntensity),
                Random.Range(-shakeIntensity, shakeIntensity),
                0f
            );

            transform.localPosition = Vector3.Lerp(startPos, targetPos, Mathf.SmoothStep(0f, 1f, t)) + shake;
            yield return null;
        }

        transform.localPosition = targetPos;
        isBeingPulled = false;
    }

    private void OnDestroy()
    {
        if (playerEmitter != null)
            playerEmitter.RemoveObserver(this);
    }
}
