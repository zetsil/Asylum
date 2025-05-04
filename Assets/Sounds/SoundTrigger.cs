using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    [Header("Sound Settings")]
    [Tooltip("The event name defined in SoundManager")]
    public string soundEvent = "UI_Click"; // Default sound
    
    [Header("Trigger Options")]
    public bool playOnStart = false;
    public bool playOnEnable = false;
    public bool playOnDestroy = false;

    void Start()
    {
        if (playOnStart)
        {
            TriggerSound();
        }
    }

    void OnEnable()
    {
        if (playOnEnable)
        {
            TriggerSound();
        }
    }

    void OnDestroy()
    {
        if (playOnDestroy && gameObject.scene.isLoaded) // Prevent errors when quitting
        {
            TriggerSound();
        }
    }

    // Call this method manually if you want to trigger the sound
    public void TriggerSound()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.HandleEvent(soundEvent);
        }
        else
        {
            Debug.LogWarning($"SoundManager instance not found when trying to play: {soundEvent}");
        }
    }

    // Editor helper - triggers sound when testing in Play Mode
    [ContextMenu("Test Sound")]
    private void TestSound()
    {
        TriggerSound();
    }
}