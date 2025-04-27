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
    public string stairsPuzzleID = "StairsPuzzle";
    public string stairsStartPuzzelID = "stairStart"; 
    private int _infiniteLoopSolvedCount = 0;

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

        // initialyze global Puzzels Infinity starirs is resolved or not 
        _objectStates[stairsPuzzleID] = false;
        _objectStates[stairsStartPuzzelID] = false;
        
        
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


    public int GetCurrentLoopCount()
    {
        return _infiniteLoopSolvedCount;
    }

    // Add these new methods for infinite loop tracking
    public void IncrementInfiniteLoopSolved()
    {
        if (_infiniteLoopSolvedCount < 3)
        {
            _infiniteLoopSolvedCount++;
            // PlayerPrefs.SetInt("InfiniteLoopSolvedCount", _infiniteLoopSolvedCount);
            // PlayerPrefs.Save();
            // _infiniteLoopSolvedCount++;
            Debug.Log($"Infinite loop solved count: {_infiniteLoopSolvedCount}/3");
        }
    }


    public void DecrementInfiniteLoopSolved()
    {
        if (_infiniteLoopSolvedCount > 0)
        {
            _infiniteLoopSolvedCount--;
            Debug.Log($"Infinite loop solved count: {_infiniteLoopSolvedCount}/3");
            
            // Optional: Uncomment to save progress
            // PlayerPrefs.SetInt("InfiniteLoopSolvedCount", _infiniteLoopSolvedCount);
            // PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("Loop count already at minimum (0)");
        }
    }

    public bool GetObjectState(string objectId)
    {
        // Null/empty check
        if (string.IsNullOrEmpty(objectId))
        {
            Debug.LogError("Tried to check state with null/empty ID!");
            return false;
        }

        // Try to get existing state
        if (_objectStates.TryGetValue(objectId, out bool currentState))
        {
            return currentState;
        }

        // Debug different behavior in editor vs. build
        #if UNITY_EDITOR
        Debug.LogWarning($"Object state not initialized for {objectId}. "
                    + "Did you forget to call GetOrRegisterObjectState() first?");
        #endif

        return false;
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