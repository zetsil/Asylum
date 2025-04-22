using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialog
{
    public string eventName; // This must be unique
    public List<string> messages;
    public bool shouldLoop;
    
    [SerializeField, HideInInspector] 
    private int currentIndex = 0;

    // Get current message without advancing
    public string CurrentMessage => 
        messages.Count > 0 ? messages[currentIndex] : string.Empty;

    // Advance to next message and return it
    public string Next()
    {
        if (messages.Count == 0)
            return string.Empty;

        string message = messages[currentIndex];
        
        currentIndex++;
        if (currentIndex >= messages.Count && shouldLoop)
        {
            currentIndex = 0;
        }
        
        return message;
    }
}

public class ChatManager : MonoBehaviour, IObserver
{
    [SerializeField] 
    private List<Dialog> _editorDialogs = new List<Dialog>();
    private Dictionary<string, Dialog> _runtimeDialogs = new Dictionary<string, Dialog>();
    private Emitter playerEmitter;


    private void Awake()
    {
        // Initialize runtime dictionary
        foreach (var dialog in _editorDialogs)
        {
            _runtimeDialogs[dialog.eventName] = dialog;
            // dialog.currentIndex = 0; // Reset state
        }
    }

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

    public void HandleEvent(string eventName)
    {
        if (_runtimeDialogs.TryGetValue(eventName, out Dialog dialog))
        {
            ProcessDialog(dialog);
        }
    }

    private void ProcessDialog(Dialog dialog)
    {
        // Assuming your Dialog class has a 'message' property
        string message = dialog.CurrentMessage;
        dialog.Next();
        
        // Call the singleton TextWriter to display with typewriter effect
        TextWriter.Instance.TypeTextToCanvas(message);
        
        // Or use the static shortcut:
        // TextWriter.Type(message);
    }

    private void DisplayMessage(string message)
    {
        // You can use either approach here:
        TextWriter.Type(message); // Instant display
    }


}
