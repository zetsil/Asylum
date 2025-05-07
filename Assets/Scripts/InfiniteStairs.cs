using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class InfiniteStairs : MonoBehaviour, IObserver
{
    [Header("Audio")]
    [SerializeField] private AudioClip loopSound;
    [SerializeField] private AudioClip progressSound;
    
    [Header("Puzzle Settings")]
    [SerializeField] private string puzzleID = "StairsPuzzle";
    
    [Header("Door Connections")]
    public DoorConnection solvedConnection; // Connection when puzzle is solved
    public DoorConnection unsolvedConnection; // Connection for infinite loop
    
    private bool isPlayerInTrigger;
    private Emitter playerEmitter;
     private string stairID; // Must be unique within this scene

    private void Awake()
    {

    }

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerEmitter = player.GetComponent<Emitter>();
            if (playerEmitter != null) playerEmitter.AddObserver(this);
        }

        // Validate connections
        if (solvedConnection == null || unsolvedConnection == null)
        {
            Debug.LogError("Missing door connections!", this);
        }else{
            stairID = unsolvedConnection.doorID;
        }
    }

    public void HandleEvent(string message)
    {
        if (message == "E" && isPlayerInTrigger) 
        {
            AttemptStairsTransition();
        }
    }

    private void AttemptStairsTransition()
    {
        bool puzzleSolved = GameStateManager.Instance?.GetObjectState(puzzleID) ?? false;
        DoorConnection connectionToUse = puzzleSolved ? solvedConnection : unsolvedConnection;

        if (connectionToUse == null)
        {
            Debug.LogError("No valid door connection!", this);
            return;
        }

        // Play appropriate sound
        SoundManager.PlayEventSound("Walk_Stairs"); // Play open sound

        // Set transition data with proper door information
        SceneTransitionManager.Instance.SetTransitionData(
                new DoorData(
                    SceneManager.GetActiveScene().name,
                    stairID,
                    connectionToUse.toDoor.sceneName
                )
            );
            
        // Load the target scene
        SceneTransitionManager.Instance.LoadScene(connectionToUse.toDoor.sceneName);

        // Debug message
        if (!puzzleSolved)
        {
            Debug.Log("The stairs loop back endlessly... something must be missing");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerInTrigger = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerInTrigger = false;
    }

    private void OnDestroy()
    {
        if (playerEmitter != null) playerEmitter.RemoveObserver(this);
    }
}