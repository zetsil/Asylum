using UnityEngine;
using UnityEngine.SceneManagement;

public class Table : MonoBehaviour, IObserver
{
    private Emitter playerEmitter; // Reference to the player's Emitter

    private bool isPlayerInTrigger = false;

    public string sceneName = "PuzzleTable";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
    {            
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


    // Update is called once per frame
    void Update()
    {
        
    }


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


        // Method to handle the emitted event (implementation of IObserver)
    public void HandleEvent(string message)
    {
        // Handle events based on the door's current state
        if (message == "E" && isPlayerInTrigger)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        }
    }
}
