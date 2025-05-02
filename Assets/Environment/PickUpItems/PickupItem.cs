using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class PickupItem : MonoBehaviour, IObserver
{
    [Header("Item Configuration")]
    public PickableItem itemData; // Assign your ScriptableObject in inspector
    public bool requiresKey = false;
    public string requiredKeySignal = "key_acquired";
    private bool canBePickedUp = false;
    private bool isPickedUp = false; // Track pickup state

    private bool youCanReachIt = false;
    private bool IsPicked = false;

    [Header("Visual Feedback")]
    public GameObject visualRepresentation;
    public ParticleSystem pickupParticles;
    public AudioClip pickupSound;

    private Emitter playerEmitter;
    private AudioSource audioSource;
    private Collider2D itemCollider;
    private string ID = "";

    [Tooltip("Events to emit when this item is picked up")]
    public List<string> eventsToEmitOnPickup = new List<string>();

    private void Awake()
    {
        itemCollider = GetComponent<Collider2D>();
        itemCollider.isTrigger = true;
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        ID = itemData.getID();
    }

    private void Start()
    {
        IsPicked = GameStateManager.Instance?.GetOrRegisterObjectState(this.ID, itemData.IsPicked) ??  itemData.IsPicked;
        if (IsPicked) // this need to change
        {
            // Destroy the GameObject if it's already picked.
            Destroy(gameObject);
            return; // Exit the function to prevent further initialization.
        }

        // Initialize pickup availability
        canBePickedUp = !requiresKey;

        // Set up signal listening with player
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerEmitter = player.GetComponent<Emitter>();
            if (playerEmitter != null)
            {
                playerEmitter.AddObserver(this);
            }
            else
            {
                Debug.LogError("Emitter component not found on player!", this);
            }
        }
        else
        {
            Debug.LogError("Player GameObject not found!", this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && canBePickedUp)
        {
            youCanReachIt = true;
        }
        // else if (other.CompareTag("Player") && requiresKey)
        // {
        //     youCanReachIt = false;
        // }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        youCanReachIt = false;
    }

    public void OnSignalReceived(string signal)
    {
        if (requiresKey && signal == requiredKeySignal)
        {
            canBePickedUp = true;
            Debug.Log("Item unlocked and ready to be picked up!");
        }
    }

    public void HandleEvent(string signal){
        if (youCanReachIt && canBePickedUp && signal == requiredKeySignal){
            this.Pickup();
        }

    }

    private void Pickup()
    {
        if (isPickedUp) return; // Prevent multiple pickups
            isPickedUp = true;
        // Play effects
        if (pickupParticles != null) 
        {
            pickupParticles.Play();
            pickupParticles.transform.parent = null; // Detach particles so they can finish
        }
        
        if (pickupSound != null) 
        {
            audioSource.PlayOneShot(pickupSound);
            Debug.Log("Play");

        }
        
        // Disable visuals and collider
        if (visualRepresentation != null) 
        {
            visualRepresentation.SetActive(false);
        }
        itemCollider.enabled = false;

        foreach (string eventName in eventsToEmitOnPickup)
        {
            if (!string.IsNullOrEmpty(eventName))
            {
                playerEmitter.NotifyObservers(eventName);
                Debug.Log($"Emitted event: {eventName}");
            }
        }
        
        // Mark as picked in the ScriptableObject
        // itemData.SetPickedState(true);
        
        // Add to inventory (you would call your inventory system here)
        // InventoryManager.Instance.AddItem(itemData);
        
        // Destroy after sound finishes (or immediately if no sound)
       // Get appropriate delay based on sound
        float disableDelay = pickupSound != null ? pickupSound.length : 0.1f;

        // Update game state and unsubscribe immediately
        GameStateManager.Instance.UpdateObjectState(this.ID, true);
        UnsubscribeFromEmitter();

        // Add to ItemManager before disabling
        ItemManager.Instance.CollectItem(gameObject);

        // Immediately move object far off-screen
        gameObject.transform.position = new Vector3(-1000f, -1000f, -1000f);

        // Schedule the disabling of the item object after delay
        StartCoroutine(DisableAfterDelay(gameObject, disableDelay));
    }

    private IEnumerator DisableAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }

    private void UnsubscribeFromEmitter()
    {
        if (playerEmitter != null)
        {
            // Use a coroutine to unsubscribe on the next frame
            StartCoroutine(UnsubscribeNextFrame());
        }
    }

    private IEnumerator UnsubscribeNextFrame()
    {
        // Wait until the end of the frame
        yield return new WaitForEndOfFrame();
        
        // Now it's safe to unsubscribe
        if (playerEmitter != null)
        {
            playerEmitter.RemoveObserver(this);
            playerEmitter = null;
        }
    }

    private void OnDestroy()
    {
        if (playerEmitter != null)
        {
            playerEmitter.RemoveObserver(this);
        }
    }
}