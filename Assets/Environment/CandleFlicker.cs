using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CandleFlicker : MonoBehaviour
{
    public Light2D candleLight;
    public float minIntensity = 0.5f;
    public float maxIntensity = 1.5f;
    public float flickerSpeed = 5f;
    private float targetIntensity;
    public bool isLit = true;
    private float originalIntensity;

    void Start()
    {
        if (candleLight == null)
        {
            candleLight = GetComponent<Light2D>();
        }
        
        originalIntensity = candleLight.intensity;
        
        if (!isLit)
        {
            candleLight.intensity = 0;
            // enabled = false; // Disable the flicker effect when candle is off
        }
        else
        {
            targetIntensity = candleLight.intensity;
        }
    }

    void Update()
    {

        if (isLit)
        {
            // Flicker effect when candle is lit
            candleLight.intensity = Mathf.Lerp(candleLight.intensity, targetIntensity, Time.deltaTime * flickerSpeed);

            if (Mathf.Abs(candleLight.intensity - targetIntensity) < 0.05f)
            {
                targetIntensity = Random.Range(minIntensity, maxIntensity);
            }
        }
        else
            {
            // Immediately set intensity to 0 when not lit
            candleLight.intensity = 0f;
            }
    }

    // Public method to check if candle is lit
    public bool IsLit()
    {
        return isLit;
    }

    // Public method to light the candle
    public void LightCandle()
    {
        isLit = true;
        candleLight.intensity = originalIntensity;
        // enabled = true; // Enable the flicker effect
        targetIntensity = candleLight.intensity;
    }

    // Public method to extinguish the candle
    public void ExtinguishCandle()
    {
        isLit = false;
        candleLight.intensity = 0;
        // enabled = false; // Disable the flicker effect
    }

    // Public method to toggle candle state
    public void ToggleCandle()
    {
        if (isLit) ExtinguishCandle();
        else LightCandle();
    }
}