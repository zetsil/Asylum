using UnityEngine;
using System.Collections;

public class HorrorCloseUp : MonoBehaviour, IObserver
{
    [Header("Canvas Settings")]
    [Tooltip("The Canvas that will be made visible")]
    public Canvas targetCanvas;

    [Tooltip("Duration of the fade-in effect in seconds")]
    public float fadeDuration = 0.5f;

    [Tooltip("Whether to hide the canvas when the game starts")]
    public bool startHidden = true;

    private CanvasGroup canvasGroup;
    private float currentAlpha = 0f;
    private bool isShowing = false;
    private bool isPlayerInRange = false;
    private bool isHiding = false;
    private Emitter playerEmitter;
    [Tooltip("Dialog event ")]
    public string dialogEvent = "empty";

    void Start()
    {
        InitializeCanvas();
        FindPlayerEmitter();
    }

    private void InitializeCanvas()
    {
        if (targetCanvas != null)
        {
            canvasGroup = targetCanvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = targetCanvas.gameObject.AddComponent<CanvasGroup>();
            }

            if (startHidden)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            else
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
        }
        else
        {
            Debug.LogWarning("Target Canvas is not assigned in the inspector!");
        }
    }

    private void FindPlayerEmitter()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerEmitter = player.GetComponent<Emitter>();
            if (playerEmitter != null)
            {
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

    void Update()
    {
        UpdateCanvasVisibility();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    private void UpdateCanvasVisibility()
    {
         if (isShowing && !isHiding)
        {
            // Fade in
            currentAlpha = Mathf.MoveTowards(currentAlpha, 1f, Time.deltaTime / fadeDuration);
            canvasGroup.alpha = currentAlpha;
            
            if (currentAlpha >= 1f)
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
        }
        else if (isHiding)
        {
            // Fade out
            currentAlpha = Mathf.MoveTowards(currentAlpha, 0f, Time.deltaTime / fadeDuration);
            canvasGroup.alpha = currentAlpha;
            
            if (currentAlpha <= 0f)
            {
                isShowing = false;
                isHiding = false;
                targetCanvas.enabled = false;
            }
        }
    }

    // IObserver implementation
    public void HandleEvent(string message)
    {
        if (message != "E") return;
        Debug.Log(isShowing);
        
        // Toggle behavior
        if (!isShowing && isPlayerInRange)
        {
            ShowCloseUp();
            playerEmitter.NotifyObservers(dialogEvent);
        }
        else if(isShowing)
        {
            HideCloseUp();
        }
    }

    public void ShowCloseUp()
    {
        if (targetCanvas == null || isShowing || isHiding) return;
        
        isShowing = true;
        isHiding = false;
        currentAlpha = 0f; // Start from fully transparent
        targetCanvas.enabled = true;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void HideCloseUp()
    {
        if (canvasGroup == null || !isShowing || isHiding) return;
        
        isHiding = true;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }


    private void DisplayMessage(string message)
    {
        // Implement your message display logic here
        // Could use a UI text element or debug log
        Debug.Log(message);
    }

    private void OnDestroy()
    {
        if (playerEmitter != null)
        {
            playerEmitter.RemoveObserver(this);
        }
    }
}