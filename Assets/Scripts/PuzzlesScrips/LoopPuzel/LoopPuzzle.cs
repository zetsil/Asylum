using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LoopPuzzle : MonoBehaviour, IObserver
{
    private Emitter playerEmitter;
    public List<GameObject> bloodParticleSystems = new List<GameObject>();
    public bool puzzleStarted = false;
    public GameObject shara;
    private bool isPuzzleTruthful = true;
    private bool puzzleSolved = false;

    [Header("Puzzle Lie Settings")]
    [Range(0f, 1f)] public float truthChance = 0.2f; // 20% chance of being true
    public GameObject creepyHandRef; 
    public GameObject lieWriting;


    [Header("Paintings")]
    public List<GameObject> truthPaintings = new List<GameObject>();
    public List<GameObject> liePaintings = new List<GameObject>();

    [Header("Lie Type System")]
    public bool bloodLie = false;
    public bool paintingLie = false;

    public bool writingLie = false;
    public bool handLie = false;


    void Start()
    {
        // Initialize puzzle state
        puzzleStarted = GameStateManager.Instance?.GetObjectState("stairStart") ?? false;
        puzzleSolved = false; // Initial state

        
        // First time always starts as truthful puzzle
        if (!puzzleStarted)
        {
            isPuzzleTruthful = true;
            // puzzleStarted = true;
            // GameStateManager.Instance?.UpdateObjectState("stairStart", true);
        }
        else
        {
            shara.SetActive(false);
            // Subsequent times: 20% chance of being true (80% chance of being a lie)
            isPuzzleTruthful = Random.value <= truthChance;
            Debug.Log($"Puzzle is truthful: {isPuzzleTruthful} (Rolled {Random.value.ToString("F2")} against {truthChance})");
            
            // Set initial solved state based on truthfulness
            puzzleSolved = !isPuzzleTruthful;
            if(!isPuzzleTruthful)
            {
                SelectRandomLieType();
                if(paintingLie)
                    SetPaintingsActive(isPuzzleTruthful);
                if(writingLie) 
                    lieWriting.SetActive(!isPuzzleTruthful);
                if(handLie) 
                    creepyHandRef.SetActive(!isPuzzleTruthful);
            }

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

    void SelectRandomLieType()
    {
        // Reset all lie types
        bloodLie = false;
        paintingLie = false;
        writingLie = false;
        handLie = false;

        // Available lie types with their corresponding indices
        List<System.Action> lieOptions = new()
        {
            () => bloodLie = true,      // Index 0
            () => paintingLie = true,   // Index 1
            () => writingLie = true,    // Index 2
            () => handLie = true        // Index 3
        };

        string[] lieNames = { "BLOOD", "PAINTINGS", "WRITING", "HAND" };

        // Randomly select one option
        int randomIndex = GameStateManager.Instance.GetNextLieIndex();
        
        // Execute the selected lie
        lieOptions[randomIndex].Invoke();
        
        Debug.Log($"Selected lie type: {lieNames[randomIndex]}");
    }

    void SetPaintingsActive(bool showTruth)
    {
        // Enable/disable truth paintings
        foreach (var painting in truthPaintings)
        {
            if (painting != null) painting.SetActive(showTruth);
        }
        
        // Enable/disable lie paintings (opposite state)
        foreach (var painting in liePaintings)
        {
            if (painting != null) painting.SetActive(!showTruth);
        }
    }

    private void OnSceneUnloadedHandler()
    {
        if(!puzzleStarted) return;
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
                if (!isPuzzleTruthful && bloodLie)
                {
                    ActivateBloodEffects();
                }
                break;
                    
            case "setRight":
                puzzleSolved = isPuzzleTruthful;
                Debug.Log($"Set right - solved: {puzzleSolved}");
                break;
                    
            case "setLeft":
                puzzleSolved = !isPuzzleTruthful;
                Debug.Log($"Set left - solved: {puzzleSolved}");
                break;

            case "HandChase": 
                if (!isPuzzleTruthful && handLie && creepyHandRef != null)
                {
                    var handChaseScript = creepyHandRef.GetComponent<CreepyHandChase>();
                    if (handChaseScript != null)
                    {
                        handChaseScript.enabled = true;
                        Debug.Log("Hand chase started!");
                    }
                }break;
            case "StartPuzzle":
                puzzleStarted = true;
                GameStateManager.Instance?.UpdateObjectState("stairStart", true);
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
