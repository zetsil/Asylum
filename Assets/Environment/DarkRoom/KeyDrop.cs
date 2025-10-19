using UnityEngine;
using System.Collections;

public class KeyDrop : MonoBehaviour
{
    [Header("Setări Cădere")]
    [Tooltip("Obiectul Transform (Empty Object) care marchează poziția finală pe podea.")]
    [SerializeField] private Transform dropTarget; // REFERINȚA LA EMPTY OBJECT

    [Tooltip("Durata totală a animației de cădere, în secunde.")]
    [SerializeField] private float dropDuration = 1.5f;

    [Tooltip("Înălțimea de la care începe căderea (față de poziția țintă).")]
    [SerializeField] private float startHeight = 10f;

    [Header("Efecte Vizuale")]
    [SerializeField] private AnimationCurve dropCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private Vector3 initialPosition;
    private Vector3 finalTargetPosition; // Poziția Vector3 extrasă din Transform
    private bool isDropping = false;

    // Metodă publică pentru a începe căderea
    // Nu mai are nevoie de parametru, folosește direct dropTarget.
    public void StartDrop()
    {
        if (isDropping) return;

        if (dropTarget == null)
        {
            Debug.LogError("Drop Target-ul nu este setat. Animația nu poate porni.", this);
            return;
        }

        // 1. Extrage poziția Vector3 din Transform
        finalTargetPosition = dropTarget.position;

        // 2. Setează poziția inițială (cea mai înaltă)
        // Cheia începe căderea de la înălțimea setată, dar la X și Z-ul țintei.
        initialPosition = new Vector3(
            finalTargetPosition.x, 
            finalTargetPosition.y + startHeight, 
            finalTargetPosition.z
        );
        transform.position = initialPosition;
        
        isDropping = true;
        StartCoroutine(SimulateDrop());
    }

    private IEnumerator SimulateDrop()
    {
        float elapsedTime = 0f;

        while (elapsedTime < dropDuration)
        {
            // Calculează progresul (t) de la 0 la 1
            float t = elapsedTime / dropDuration;

            // Aplică curba de animație (easing)
            float curveValue = dropCurve.Evaluate(t);

            // Interpolarea între poziția inițială și poziția finală
            transform.position = Vector3.Lerp(initialPosition, finalTargetPosition, curveValue);

            elapsedTime += Time.deltaTime;
            yield return null; // Așteaptă următorul cadru
        }

        // Asigură-te că ajunge exact la poziția țintă
        transform.position = finalTargetPosition;
        isDropping = false;
        
        Debug.Log("Cheia a ajuns la destinație.");
    }
    
    // O poți apela în Start() pentru test rapid sau dintr-un alt script
    private void Start()
    {
        // Poți apela StartDrop() dintr-un alt script de trigger, sau aici pentru a vedea efectul imediat
        StartDrop(); 
    }
}