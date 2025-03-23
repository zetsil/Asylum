using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    private Emitter emitter; // Reference to the Emitter component

    private void Start()
    {
        // Get the Emitter component attached to the player
        emitter = GetComponent<Emitter>();

        if (emitter == null)
        {
            Debug.LogError("Emitter component not found on the player!");
        }
    }

    private void Update()
    {
        // Handle action input
        if (Input.GetKeyDown(KeyCode.E))
        {
            PerformAction();
        }
    }

    private void PerformAction()
    {
        // Emit an event when the player performs an action
        if (emitter != null)
        {
            emitter.NotifyObservers("E");
        }

        Debug.Log("Player pressed E to perform an action.");
    }
}