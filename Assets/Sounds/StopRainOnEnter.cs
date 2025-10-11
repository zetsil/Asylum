using UnityEngine;

public class StopRainOnEnter : MonoBehaviour
{
    // Setează numele evenimentului de ploaie în Inspector.
    // Presupunem că acesta se numește "RainLoop"
    public string rainEventName = "RainLoop"; 

    private void Awake()
    {
        // Oprește sunetul de tip loop imediat ce obiectul este inițializat în scenă.
        // Acest lucru este util dacă ploaia pornește automat la începutul nivelului
        // și vrei ca într-o anumită zonă/cameră să se oprească instantaneu.
        SoundManager.StopLoopingSound(rainEventName);
        
        // Dacă nu mai ai nevoie de acest script după ce sunetul este oprit,
        // poți distruge componenta pentru optimizare:
        // Destroy(this);
    }
    
    // Metoda OnTriggerEnter2D nu mai este necesară pentru a opri sunetul
    // dacă Awake() face acest lucru, dar o las ca referință:

    /*
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Această logică este necesară doar dacă vrei ca oprirea
        // să se facă la contactul cu jucătorul.
        if (other.CompareTag("Player"))
        {
            SoundManager.StopLoopingSound(rainEventName);
        }
    }
    */
}