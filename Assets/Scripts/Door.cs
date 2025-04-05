using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour, IObserver
{
    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitude = 0.1f;
    [SerializeField] private AudioClip lockedSound;
    [SerializeField] private AudioClip openSound;

    [Header("Scene Transition")]
    public DoorConnection connection;
    private string doorID; // Must be unique within this scene

    private bool doorIsLocked = false;


    private Emitter playerEmitter; // Reference to the player's Emitter
    private IDoorState currentState; // Current state of the door
    private Vector3 originalPosition;
    private AudioSource audioSource;
    
    public string requiredKeyId = "DoorKey1"; // Key required to unlock the door
    private bool isPlayerInTrigger = false; // Track if the player is inside the trigger collide


    private void Awake()
    {
        originalPosition = transform.position;

        audioSource = GetComponent<AudioSource>();

        if (connection == null || string.IsNullOrEmpty(connection.doorID))
        {
            Debug.LogError("Connection or doorID is null!");
            // Handle the error case - maybe generate a random ID or disable the component
            // doorID = GenerateRandomDoorID();
            // or throw an exception if this should never happen
            // throw new System.Exception("Invalid door connection");
        }
        else
        {
            doorID = connection.doorID;
        }


        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start()
    {
        doorIsLocked = GameStateManager.Instance?.GetOrRegisterObjectState(doorID, connection.locked) ?? connection.locked; // This will now work safely in the editor:
            
        // Find the player GameObject by tag
        GameObject player = GameObject.FindWithTag("Player");

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

        if(doorIsLocked)
        {
            this.SetState(new LockedState());
        }
        else
        {
            this.SetState(new OpenState());
        }
    }

    

    // Method to handle the emitted event (implementation of IObserver)
    public void HandleEvent(string message)
    {
        // Handle events based on the door's current state
        if (message == "E" && isPlayerInTrigger)
        {
            TryOpenDoor();
        }
    }

    public void TryOpenDoor()
    {
        if (currentState is LockedState) // no key but you  try to open 
        {
            if (ItemManager.Instance.HasItemWithID(requiredKeyId))
            {
                currentState.HandleUnlock(this);
                Debug.Log("Door unlocked with key!");
                doorIsLocked = false;
                GameStateManager.Instance.UpdateObjectState(doorID, doorIsLocked); // change game state 
                ItemManager.Instance.RemoveItemWithID(requiredKeyId);
                return;
            }
            currentState.HandleOpen(this);
        }
        else if(currentState is OpenState) // the door is already unlocked 
        {
            currentState.HandleOpen(this);
        }
    }

    // Set the current state of the door
    public void SetState(IDoorState newState)
    {
        currentState = newState;
    }

    public void OpenDoor()
    {
        if (connection != null)
        {
            SceneTransitionManager.Instance.SetTransitionData(
                new DoorData(
                    SceneManager.GetActiveScene().name,
                    doorID,
                    connection.toDoor.sceneName
                )
            );
            SceneTransitionManager.Instance.LoadScene(connection.toDoor.sceneName);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }


    public void HandleLockedState()
    {
        StartCoroutine(Shake());
        PlaySound(lockedSound);
        DisplayMessage("The door is locked. Find a key!");
    }

    // Trigger collider methods
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
        }
    }

    // Display a message to the player (e.g., UI text)
    private void DisplayMessage(string message)
    {
        // Example: Display the message in the console
        // Debug.Log(message);

        // You can also display the message in the UI (e.g., a Text component)
        // Example:
        // GameObject.Find("MessageText").GetComponent<Text>().text = message;
    }

    private IEnumerator Shake()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            
            // Calculate shake offset (2D only - x and y axes)
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;
            
            transform.position = originalPosition + new Vector3(x, y, 0);
            yield return null;
        }

        transform.position = originalPosition;
    }

    private void OnDestroy()
    {
        // Remove this door as an observer when it is destroyed
        if (playerEmitter != null)
        {
            playerEmitter.RemoveObserver(this);
        }
    }
}