using UnityEngine;
 // For Light2D

[RequireComponent(typeof(CandleFlicker))]
public class CandleActivator : MonoBehaviour
{
    public enum ActivationCondition { One = 1, Two = 2, Three = 3 }
    public ActivationCondition activateWhenSolved = ActivationCondition.One;

    [Header("Child Components")]
    [SerializeField] private UnityEngine.Rendering.Universal.Light2D candleLight; // Assign in Inspector
    
    private CandleFlicker candleFlicker;

    void Start()
    {
        // Get components
        candleFlicker = GetComponent<CandleFlicker>();
        
        // Auto-find Light2D in children if not assigned
        if (candleLight == null)
        {
            candleLight = GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>();
        }

        // Initial state - everything disabled
        SetCandleState(false);
        
        // Check activation condition
        CheckSolvedCondition();
    }

    void CheckSolvedCondition()
    {
        if (GameStateManager.Instance == null) return;

        bool shouldActivate = GameStateManager.Instance.GetCurrentLoopCount() >= (int)activateWhenSolved;
        SetCandleState(shouldActivate);

        Debug.Log($"Candle {gameObject.name} - " +
                $"Active: {shouldActivate} " +
                $"(Requires: {(int)activateWhenSolved}, " +
                $"Current: {GameStateManager.Instance.GetCurrentLoopCount()})");
    }

    void SetCandleState(bool active)
    {
        // Handle CandleFlicker
        if (candleFlicker != null)
        {
            candleFlicker.enabled = active;
        }
        else
        {
            Debug.LogWarning("Missing CandleFlicker reference", this);
        }

        // Handle Light2D
        if (candleLight != null)
        {
            candleLight.enabled = active;
        }
#if UNITY_EDITOR
        else
        {
            Debug.LogWarning("Light2D not found on children", this);
        }
#endif
    }

}