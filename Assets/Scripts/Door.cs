using UnityEngine;

public class Door : MonoBehaviour, IObserver
{
    private Emitter playerEmitter; // Reference to the player's Emitter
    private IDoorState currentState; // Current state of the door
    
    public string requiredKeyId = "DoorKey123"; // Key required to unlock the door
    private bool isPlayerInTrigger = false; // Track if the player is inside the trigger collide

    private void Start()
    {
        // Initialize the door to the Closed state
        currentState = new LockedState(); // here later this will be geting from an scriptableobject

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

    // Try to open the door
    public void TryOpenDoor()
    {
        if (currentState is LockedState)
        {
            // if player has a key: ( chek player items)
            //   currentState.HandleOpen(this);
            //else
            //  door is locked
            Debug.Log("The door is locked.");
            // Display a message to the player (e.g., UI text)
            DisplayMessage("The door is locked.");
        }
        else
        {
            currentState.HandleOpen(this);
        }
    }

    // Set the current state of the door
    public void SetState(IDoorState newState)
    {
        currentState = newState;
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

    private void OnDestroy()
    {
        // Remove this door as an observer when it is destroyed
        if (playerEmitter != null)
        {
            playerEmitter.RemoveObserver(this);
        }
    }
}