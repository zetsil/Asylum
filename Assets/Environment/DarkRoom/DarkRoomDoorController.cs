using UnityEngine;
using System.Collections;


public class DarkRoomDoorController : MonoBehaviour
{
    // Obiecte de referință (se setează din Inspectorul Unity)
    [Header("Obiecte de Scenă")]
    [Tooltip("Cheia fizică care va deveni vizibilă/activă.")]
    public GameObject keyObject;
    public GameObject parentKey;
    
    [Tooltip("Ușa fizică ce va deveni invizibilă/inactivă.")]
    public GameObject doorObject;
    public GameObject coliderKey;

    // Se apelează o dată la începutul jocului.
    void Start()
    {
        // Ne asigurăm că există GameStateManager înainte de a continua.
        if (GameStateManager.Instance == null)
        {
            Debug.LogError("FATAL: GameStateManager nu a fost găsit în scenă!");
            return;
        }

        // Inițial, presupunem că ușa este activă și cheia este ascunsă.
        // Totuși, verificăm starea curentă în caz că se face un Load Game.
        CheckPuzzleState();
    }

    // Această metodă verifică starea și aplică logica.
    public void CheckPuzzleState()
    {
        // 1. Verifică starea puzzle-ului din Singleton
        bool isResolved = GameStateManager.Instance.isDarkRoomPuzzleResolved;

        if (isResolved)
        {
            // Puzzle-ul ESTE rezolvat:
            
            // Set Cheia la activ (vizibilă și interacționabilă)
            if (keyObject != null)
            {
                parentKey.SetActive(true);
                keyObject.SetActive(true); // add check
                coliderKey.SetActive(false); // add chek
                Debug.Log("Dark Room Puzzle Rezolvat: Cheia a fost activată.");
            }
            
            // Set Ușa la inactiv (dispărută sau deschisă)
            if (doorObject != null)
            {
                doorObject.SetActive(false);
                Debug.Log("Dark Room Puzzle Rezolvat: Ușa a fost dezactivată.");
            }
        }
        else
        {
            // Puzzle-ul NU este rezolvat:
            
            // Cheia rămâne ascunsă
            if (keyObject != null)
            {
                keyObject.SetActive(false);
            }
            
            // Ușa rămâne activă/blocată
            if (doorObject != null)
            {
                doorObject.SetActive(true);
            }
        }
    }

    // Metodă de apelat când puzzle-ul se rezolvă în timpul jocului (opțional)
    public void OnPuzzleResolvedExternally()
    {
        // Setează starea în manager (dacă nu e deja setată de alt script)
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.isDarkRoomPuzzleResolved = true;
        }
        
        // Aplică imediat modificările în scenă
        CheckPuzzleState();
    }
}