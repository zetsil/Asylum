using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class CandlePatternChecker : MonoBehaviour , IObserver
{
   [Header("Candle Assignments")]
   [Tooltip("Drag candles here in the order you want them checked")]
    public List<GameObject> candles = new List<GameObject>();

    [Header("Pattern Settings")]
    [Tooltip("Enter pattern as 1s (on) and 0s (off) like '1010'")]
    public string targetPattern = "1010";

    [Header("Visual Changes")]
    public SpriteRenderer targetSpriteRenderer;
    public Sprite angelActiveSprite;
    public Image closeUpCanvasImage; // Reference to UI Image component
    private Sprite originalCloseUpAngelSprite;
    public Sprite closeUpAngelSprite; // New sprite for close-up view

    [Header("Object Visibility")]
    [Tooltip("GameObject to make invisible at start")]
    public GameObject angelKey;

    [Tooltip("Collider2D component of the angel object to deactivate when puzzle is resolved")]
    public Collider2D angelCollider; // New field for the collider


    private List<CandleFlicker> candleScripts = new List<CandleFlicker>();
    private Emitter playerEmitter;
    private Sprite originalSprite;

    private string puzzleTrigeredID = "PT";
    private string puzzleResolvedID = "PR";

    private bool puzzleTrigered = false;
    private bool puzzleResolved = false;
    public CanvasFlasher targetFlasher; // Assign in Inspector

 


    [Tooltip("Flash duration in seconds")] 
    public float flashDuration = 0.5f;
    [Tooltip("Number of flash pulses")] 
    public int flashCount = 3;

    void Start()
    {
        // get the player emmtter used to remove it onDestroy from observer list
        // This should had been an global thing but I 
        // am too lazy to change it now

        GameObject player = GameObject.FindWithTag("Player");
        puzzleTrigered = GameStateManager.Instance.GetOrRegisterObjectState(this.puzzleTrigeredID, false);
        puzzleResolved = GameStateManager.Instance.GetOrRegisterObjectState(this.puzzleResolvedID, false);


        // If the puzzle was previously resolved
        if (puzzleResolved)
        {
            // Check if angelKey still exists in the scene before activating
            if (angelKey != null && !angelKey.activeSelf)
            {
                angelKey.SetActive(true);
            }

            // Disable the collider if assigned
            if (angelCollider != null)
            {
                angelCollider.enabled = false;
            }

        }

         // Hide the specified object at start
        if (angelKey != null && !puzzleResolved)
        {
            angelKey.SetActive(false);
        }

        if (player != null)
        {
            // Get the Emitter component from the player
            playerEmitter = player.GetComponent<Emitter>();

            if (playerEmitter != null)
            {
                // Add this door as an observer to the player's Emitter
                playerEmitter.AddObserver(this);
            }
            else
            {
                Debug.LogError("Emitter component not found on the player!");
            }
        }
        else
        {
            Debug.LogError("Player GameObject not found!");
        }

        // Store the original sprite at start
        if (targetSpriteRenderer != null)
        {
            originalSprite = targetSpriteRenderer.sprite;
        }

        originalCloseUpAngelSprite = closeUpCanvasImage.sprite;

        // Cache all candle scripts at start
        foreach (GameObject candle in candles)
        {
            if (candle != null)
            {
                CandleFlicker script = candle.GetComponent<CandleFlicker>();
                if (script != null)
                {
                    candleScripts.Add(script);
                }
                else
                {
                    Debug.LogWarning("CandleFlicker script missing on: " + candle.name);
                }
            }
        }

        if(puzzleTrigered && !puzzleResolved){

             // Change sprite if references exist
            if (targetSpriteRenderer != null && angelActiveSprite != null)
            {
                targetSpriteRenderer.sprite = angelActiveSprite;
            }

            if (closeUpCanvasImage != null && closeUpAngelSprite != null)
            {
                closeUpCanvasImage.sprite = closeUpAngelSprite;
            }

            foreach (GameObject candle in candles){
                CandelOnOff script = candle.GetComponent<CandelOnOff>();
                script.enabled = true;
            }

            // Flip all candles
            foreach (CandleFlicker candle in candleScripts)
            {
                if (candle != null)
                {
                    candle.ToggleCandle();
                }
            }
        }
    }

    // Get current candle states as binary string
    private string GetCurrentPattern()
    {
        string currentPattern = "";

        foreach (CandleFlicker candle in candleScripts)
        {
            if (candle != null)
            {
                currentPattern += candle.IsLit() ? "1" : "0";
            }
            else
            {
                currentPattern += "0"; // Treat missing candles as off
            }
        }

        return currentPattern;
    }

    // Check if current pattern matches target pattern
    public bool CheckPatternMatch()
    {
        string currentPattern = GetCurrentPattern();
        
        if (currentPattern.Length != targetPattern.Length)
        {
            Debug.LogWarning($"Pattern length mismatch! Current: {currentPattern.Length}, Target: {targetPattern.Length}");
            return false;
        }

        bool matches = currentPattern == targetPattern;
        Debug.Log($"Current: {currentPattern} | Target: {targetPattern} | Match: {matches}");
        return matches;
    }

    // You can call this to see the current pattern in console
    public void LogCurrentPattern()
    {
        Debug.Log("Current candle pattern: " + GetCurrentPattern());
    }


    private void OnDestroy()
    {
        // Remove this door as an observer when it is destroyed
        if (playerEmitter != null)
        {
            playerEmitter.RemoveObserver(this);
        }
    }


    public void HandleEvent(string message)
    {
        
     if (message == "TriggerAngel" && !puzzleTrigered)
        {
            puzzleTrigered = true;
            GameStateManager.Instance.UpdateObjectState(this.puzzleTrigeredID, true); // set in game state manager
            // foreach (GameObject candle in candles){
            //     CandelOnOff script = candle.GetComponent<CandelOnOff>();
            //     script.enabled = true;
            // }
            // Flip all candles
            foreach (CandleFlicker candle in candleScripts)
            {
                if (candle != null)
                {
                    candle.ExtinguishCandle();
                }
            }

            // Change sprite if references exist
            if (targetSpriteRenderer != null && angelActiveSprite != null)
            {
                targetSpriteRenderer.sprite = angelActiveSprite;
            }

            if (closeUpCanvasImage != null && closeUpAngelSprite != null)
            {
                closeUpCanvasImage.sprite = closeUpAngelSprite;
            }

            // Trigger flash
            if (targetFlasher != null)
            {
                targetFlasher.TriggerFlash();
            }

        }
        else if (message == "candelFlip")
        {
            if (CheckPatternMatch() && puzzleTrigered && !puzzleResolved)
                {
                    puzzleResolved = true;
                    GameStateManager.Instance.UpdateObjectState(this.puzzleResolvedID, true); // set in game state manager

                    // Revert to original sprite
                    if (targetSpriteRenderer != null && originalSprite != null)
                    {
                        targetSpriteRenderer.sprite = originalSprite;
                    }
                                        
                    // Trigger flash again
                    if (targetFlasher != null)
                    {
                        targetFlasher.TriggerFlash();
                    }
                    
                    // Make the hidden object visible
                    if (angelKey != null)
                    {
                        angelKey.SetActive(true);
                    }

                    // Disable collider when puzzle is triggered
                    if (angelCollider != null)
                    {
                        angelCollider.enabled = false;
                    }

                    closeUpCanvasImage.sprite = originalCloseUpAngelSprite;

                    // Optional: Disable candle interaction after puzzle is solved
                    foreach (GameObject candle in candles)
                    {
                        CandelOnOff script = candle.GetComponent<CandelOnOff>();
                        if (script != null) script.enabled = false;
                    }
                }
            }else if (message == "ActivateColider"){
                if (angelCollider != null)
                    {
                        angelCollider.enabled = true;
                    }
            }
        
    }


   


}