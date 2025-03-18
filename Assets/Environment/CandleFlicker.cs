using UnityEngine;


public class CandleFlicker : MonoBehaviour
{
    public UnityEngine.Rendering.Universal.Light2D candleLight; // Referință la componenta Light2D
    public float minIntensity = 0.5f; // Intensitatea minimă a luminii
    public float maxIntensity = 1.5f; // Intensitatea maximă a luminii
    public float flickerSpeed = 5f; // Viteza de palpaire

    private float targetIntensity;

    void Start()
    {
        if (candleLight == null)
        {
            candleLight = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        }
        targetIntensity = candleLight.intensity;
    }

    void Update()
    {
        // Schimbă intensitatea luminii între minIntensity și maxIntensity
        candleLight.intensity = Mathf.Lerp(candleLight.intensity, targetIntensity, Time.deltaTime * flickerSpeed);

        // Dacă intensitatea este aproape de targetIntensity, alege un nou target
        if (Mathf.Abs(candleLight.intensity - targetIntensity) < 0.05f)
        {
            targetIntensity = Random.Range(minIntensity, maxIntensity);
        }
    }
}