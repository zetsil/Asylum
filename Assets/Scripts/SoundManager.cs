using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour, IObserver
{
    public static SoundManager Instance { get; private set; }

    [System.Serializable]
    public class SoundEvent
    {
        public string eventName;
        public AudioClip soundClip;
        [Range(0f, 1f)] public float volume = 1f;
        public bool playOnlyOnce = false; // Global - only once per app session
        public bool loop = false;
        
        [HideInInspector] public bool hasPlayed = false;
        [HideInInspector] public AudioSource dedicatedSource;
    }

    public SoundEvent[] soundEvents;

    private AudioSource defaultAudioSource;
    private Emitter registeredEmitter;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            defaultAudioSource = gameObject.AddComponent<AudioSource>();
            InitializeLoopSources();
            SceneManager.sceneLoaded += OnSceneLoaded;
            RegisterWithPlayerEmitter();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeLoopSources()
    {
        foreach (var soundEvent in soundEvents)
        {
            if (soundEvent.loop)
            {
                soundEvent.dedicatedSource = gameObject.AddComponent<AudioSource>();
                soundEvent.dedicatedSource.clip = soundEvent.soundClip;
                soundEvent.dedicatedSource.volume = soundEvent.volume;
                soundEvent.dedicatedSource.loop = true;
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // No need for scene-specific handling anymore
        RegisterWithPlayerEmitter();
    }

    private void RegisterWithPlayerEmitter()
    {
        if (registeredEmitter != null)
        {
            registeredEmitter.RemoveObserver(this);
            registeredEmitter = null;
        }

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Emitter emitter = player.GetComponent<Emitter>();
            if (emitter == null) emitter = player.AddComponent<Emitter>();
            emitter.AddObserver(this);
            registeredEmitter = emitter;
        }
        else
        {
            Invoke(nameof(RegisterWithPlayerEmitter), 0.5f);
        }
    }

    public void HandleEvent(string eventName)
    {
        foreach (var soundEvent in soundEvents)
        {
            if (soundEvent.eventName == eventName)
            {
                if (soundEvent.playOnlyOnce && soundEvent.hasPlayed) return;

                if (soundEvent.loop)
                {
                    HandleLoopingSound(soundEvent);
                }
                else
                {
                    defaultAudioSource.PlayOneShot(soundEvent.soundClip, soundEvent.volume);
                }

                soundEvent.hasPlayed = true;
                return;
            }
        }
    }

    private void HandleLoopingSound(SoundEvent soundEvent)
    {
        if (soundEvent.dedicatedSource == null) return;

        if (!soundEvent.dedicatedSource.isPlaying)
        {
            soundEvent.dedicatedSource.Play();
        }
    }

    public static void PlayEventSound(string eventName)
    {
        if (Instance != null)
        {
            Instance.HandleEvent(eventName);
        }
    }

    public static void StopLoopingSound(string eventName)
    {
        if (Instance == null) return;

        foreach (var soundEvent in Instance.soundEvents)
        {
            if (soundEvent.eventName == eventName && soundEvent.loop && soundEvent.dedicatedSource != null)
            {
                soundEvent.dedicatedSource.Stop();
                return;
            }
        }
    }

    public static void ResetSoundEvent(string eventName)
    {
        if (Instance == null) return;

        foreach (var soundEvent in Instance.soundEvents)
        {
            if (soundEvent.eventName == eventName)
            {
                soundEvent.hasPlayed = false;
                return;
            }
        }
    }

    private void OnDestroy()
    {
        if (registeredEmitter != null)
        {
            registeredEmitter.RemoveObserver(this);
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}