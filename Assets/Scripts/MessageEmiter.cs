using UnityEngine;

public class MessageEmiter : MonoBehaviour
{
    private Emitter playerEmitter;
    private bool playerInTrigger = false;
    public string dialog = "empty";

    private void Start()
    {            
        // Find the player GameObject by tag
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            // Get the Emitter component from the player
            playerEmitter = player.GetComponent<Emitter>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("hello");
            EmitSignal();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the entering object is the player
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Check if the exiting object is the player
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
        }
    }

    void EmitSignal()
    {
        
       playerEmitter.NotifyObservers(dialog);
    }
}