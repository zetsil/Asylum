using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    // Singleton instance with lazy initialization
    private static GameStateManager _instance;
    public static GameStateManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameStateManager>();
                
                #if UNITY_EDITOR
                if (_instance == null && Application.isPlaying)
                {
                    Debug.LogWarning("GameStateManager auto-created for editor testing");
                    GameObject go = new GameObject("GameStateManager (Editor-Temporary)");
                    _instance = go.AddComponent<GameStateManager>();
                    // Optional: mark as don't destroy if you want it to persist
                    // DontDestroyOnLoad(go);
                }
                #endif
                
                if (_instance == null && !Application.isPlaying)
                {
                    Debug.LogError("GameStateManager accessed in edit mode!");
                }
            }
            return _instance;
        }
    }

    private Dictionary<string, bool> _objectStates = new Dictionary<string, bool>();

    private void Awake()
    {
        // Handle duplicate instances
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning($"Duplicate GameStateManager on {gameObject.name} - destroying");
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);
        
        LoadStates(); // Load any saved states
    }

    public bool GetOrRegisterObjectState(string objectId, bool defaultState = false)
    {
        if (string.IsNullOrEmpty(objectId))
        {
            Debug.LogError("Invalid object ID");
            return defaultState;
        }

        // Safe access - will create new entry if needed
        if (!_objectStates.TryGetValue(objectId, out bool currentState))
        {
            _objectStates[objectId] = defaultState;
            return defaultState;
        }
        
        return currentState;
    }

    public void UpdateObjectState(string objectId, bool newState)
    {
        if (string.IsNullOrEmpty(objectId))
        {
            Debug.LogError("Invalid object ID");
            return;
        }
        
        _objectStates[objectId] = newState;
    }

    public void SaveStates()
    {
        foreach (var pair in _objectStates)
        {
            PlayerPrefs.SetInt("ObjectState_" + pair.Key, pair.Value ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    public void LoadStates()
    {
        // _objectStates.Clear();
        
        // // Load all states with the matching prefix
        // foreach (var key in PlayerPrefs.GetAllKeys())
        // {
        //     if (key.StartsWith("ObjectState_"))
        //     {
        //         string objectId = key.Substring("ObjectState_".Length);
        //         _objectStates[objectId] = PlayerPrefs.GetInt(key) == 1;
        //     }
        // }
    }

    #if UNITY_EDITOR
    private void OnDestroy()
    {
        // Clean up static reference if this was the editor-created instance
        if (_instance == this)
        {
            _instance = null;
        }
    }
    #endif
}