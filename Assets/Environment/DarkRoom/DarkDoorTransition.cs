using UnityEngine;
using UnityEngine.SceneManagement;

public class DarkDoorTransition : MonoBehaviour, IObserver
{
    [Header("Door Settings")]
    [SerializeField] private string doorID = "StrangeDoor";
    [SerializeField] private string targetSceneName = "TheVoidScene"; // Numele scenei Vidului Negru
    [SerializeField] private int requiredEntryCount = 5; // Numarul de intrari necesare
    [SerializeField] private string progressKey = "VoidEntryCount"; // Cheia GameStateManager

    private Emitter playerEmitter;
    private bool isPlayerInTrigger;

    private void Start()
    {

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerEmitter = player.GetComponent<Emitter>();
            if (playerEmitter != null) playerEmitter.AddObserver(this);
        }
    }

    public void HandleEvent(string message)
    {   
        if (SceneTransitionManager.Instance.isTransitioning) return;
        if (message == "E" && isPlayerInTrigger)
        {
            AttemptDarkDoorTransition();
        }
    }

    private void AttemptDarkDoorTransition()
    {
        if (GameStateManager.Instance == null || SceneTransitionManager.Instance == null)
        {
            Debug.LogError("Managerii nu sunt incarcati.", this);
            return;
        }

        // 1. Inregistreaza tranzitia
        int currentCount = GameStateManager.Instance.GetCounterDarkRoom();
        

        

        // Tranzitia catre Vidul Negru
        Debug.Log($"Intrare in Vid: {currentCount}/{requiredEntryCount}. Un punct alb a aparut.");
        GameStateManager.Instance.incrementDarkRoom();

        // 3. Setarea datelor de tranzitie
        SceneTransitionManager.Instance.SetTransitionData(
            new DoorData(
                SceneManager.GetActiveScene().name,
                doorID,
                targetSceneName
            )
        );

        // 4. Apelarea FUNCTIEI SPECIALE
        SceneTransitionManager.Instance.LoadStrangeDoorScene(targetSceneName);
        
        
        SoundManager.PlayEventSound("Open_Door"); // Sunetul usii
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerInTrigger = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerInTrigger = false;
    }

    private void OnDestroy()
    {
        if (playerEmitter != null) playerEmitter.RemoveObserver(this);
    }
}