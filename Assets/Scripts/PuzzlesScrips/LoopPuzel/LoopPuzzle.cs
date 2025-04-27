using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LoopPuzzle : MonoBehaviour, IObserver
{
    private Emitter playerEmitter;
    public List<GameObject> bloodParticleSystems = new List<GameObject>();
    public bool puzzleStarted = false;
    private bool isPuzzleTruthful = true;
    private bool puzzleSolved = false;

    [Header("Puzzle Lie Settings")]
    [Range(0f, 1f)] public float truthChance = 0.2f; // 20% chance of being true

    void Start()
    {
        // Initialize puzzle state
        puzzleStarted = GameStateManager.Instance?.GetObjectState("stairStart") ?? false;
        puzzleSolved = false; // Initial state
        
        // First time always starts as truthful puzzle
        if (!puzzleStarted)
        {
            isPuzzleTruthful = true;
            puzzleStarted = true;
            GameStateManager.Instance?.UpdateObjectState("stairStart", true);
        }
        else
        {
            // Subsequent times: 20% chance of being true (80% chance of being a lie)
            isPuzzleTruthful = Random.value <= truthChance;
            Debug.Log($"Puzzle is truthful: {isPuzzleTruthful} (Rolled {Random.value.ToString("F2")} against {truthChance})");
            
            // Set initial solved state based on truthfulness
            puzzleSolved = !isPuzzleTruthful;
        }

        // Find and observe player emitter
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerEmitter = player.GetComponent<Emitter>();
            if (playerEmitter != null)
            {
                playerEmitter.AddObserver(this);
            }
        }
    }


    private void OnSceneUnloadedHandler()
    {
        if (GameStateManager.Instance == null) return;
        
        if (puzzleSolved)
        {
            GameStateManager.Instance.IncrementInfiniteLoopSolved();
            Debug.Log("Puzzle solved - incrementing loop count");
        }
        else
        {
            GameStateManager.Instance.DecrementInfiniteLoopSolved();
            Debug.Log("Puzzle failed - decrementing loop count");
        }
    }

    public void HandleEvent(string message)
    {
        switch (message)
        {
            case "bloodRain":
                if (!isPuzzleTruthful) // Only show blood if it's a lie
                {
                    ActivateBloodEffects();
                }
                break;
                
            case "setRight":
                puzzleSolved = isPuzzleTruthful; // Right is correct when truthful
                Debug.Log($"Set right - solved: {puzzleSolved}");
                break;
                
            case "setLeft":
                puzzleSolved = !isPuzzleTruthful; // Left is correct when lying
                Debug.Log($"Set left - solved: {puzzleSolved}");
                break;
        }
    }

    private void ActivateBloodEffects()
    {
        foreach (GameObject bloodParticle in bloodParticleSystems)
        {
            if (bloodParticle == null) continue;
            
            var timedBlood = bloodParticle.GetComponent<TimedBloodParticles>();
            if (timedBlood != null)
            {
                timedBlood.enabled = true;
            }
        }
    }

    private void OnDestroy()
    {
        OnSceneUnloadedHandler();
        Debug.Log($"Current solved: {GameStateManager.Instance.GetCurrentLoopCount()}");
    }
}
