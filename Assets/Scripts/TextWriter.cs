using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TextWriter : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Drag your Canvas Text component here")]
    public TextMeshProUGUI targetText;

    [Header("Typewriter Settings")]
    [Tooltip("Characters per second")]
    public float typingSpeed = 20f;
    [Tooltip("Time in seconds before clearing text after completion")]
    public float clearDelay = 3f;
    [Tooltip("Sound to play for each character (optional)")]
    public AudioClip typingSound;
    [Tooltip("AudioSource to play typing sounds (optional)")]
    public AudioSource audioSource;

    private Coroutine typingCoroutine;
    private Coroutine clearCoroutine;
    private bool isTyping = false;

    private static TextWriter _instance;
    public static TextWriter Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<TextWriter>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("TextWriter");
                    _instance = obj.AddComponent<TextWriter>();
                    DontDestroyOnLoad(obj);
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void WriteTextToCanvas(string message)
    {
        if (targetText != null)
        {

            StopTyping();
            targetText.text = message;
            StartClearTimer();
        }
        else
        {
            Debug.LogWarning("No Text component reference set in TextWriter script!");
        }
    }

    public void TypeTextToCanvas(string message)
    {
        if (targetText == null)
        {
            Debug.LogWarning("No Text component reference set in TextWriter script!");
            return;
        }


        StopTyping();
        typingCoroutine = StartCoroutine(TypeTextCoroutine(message));
    }

    private IEnumerator TypeTextCoroutine(string message)
    {
        isTyping = true;
        targetText.text = "";
        float delay = 1f / typingSpeed;

        foreach (char c in message)
        {
            targetText.text += c;

            if (typingSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(typingSound);
            }

            yield return new WaitForSeconds(delay);
        }

        isTyping = false;
        StartClearTimer();
    }

    private void StartClearTimer()
    {
        if (clearCoroutine != null)
        {
            StopCoroutine(clearCoroutine);
        }
        clearCoroutine = StartCoroutine(ClearAfterDelay());
    }

    private IEnumerator ClearAfterDelay()
    {
        yield return new WaitForSeconds(clearDelay);
        ClearText();
    }

    private void StopTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        if (clearCoroutine != null)
        {
            StopCoroutine(clearCoroutine);
            clearCoroutine = null;
        }
        isTyping = false;
    }

    public void FinishTyping()
    {
        StopTyping();
        StartClearTimer();
    }

    public void ClearText()
    {
        StopTyping();
        if (clearCoroutine != null)
        {
            StopCoroutine(clearCoroutine);
            clearCoroutine = null;
        }
        if (targetText != null)
        {
            targetText.text = "";
        }
    }

    public static void Write(string message)
    {
        Instance.WriteTextToCanvas(message);
    }

    public static void Type(string message)
    {
        Instance.TypeTextToCanvas(message);
    }

    public static void Clear()
    {
        Instance.ClearText();
    }

    
}