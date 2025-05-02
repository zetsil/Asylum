using UnityEngine;

public class CandelOnOff : MonoBehaviour, IObserver
{
    private bool isPlayerInTrigger = false;
    private Emitter playerEmitter;
    private CandleFlicker candleLight;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        playerEmitter = GetPlayerEmitter();
        playerEmitter.AddObserver(this);
        candleLight = GetComponent<CandleFlicker>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void HandleEvent(string message)
    {
        // Handle events based on the door's current state
        if (message == "E" && isPlayerInTrigger)
        {
            candleLight.isLit = !candleLight.isLit;
            playerEmitter.NotifyObservers("candelFlip");
        }
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

    private Emitter GetPlayerEmitter()
    {
        // Find the player GameObject by tag
        GameObject player = GameObject.FindWithTag("Player");
        
        if (player == null)
        {
            Debug.LogError("Player GameObject not found!");
            return null;
        }

        // Get the Emitter component from the player
        Emitter emitter = player.GetComponent<Emitter>();
        
        if (emitter == null)
        {
            Debug.LogError("Emitter component not found on the player!");
            return null;
        }

        return emitter;
    }

    private void OnDestroy()
    {
        if (playerEmitter != null)
        {
            playerEmitter.RemoveObserver(this);
        }
    }
}
