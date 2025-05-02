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
    private Queue<int> _lieTypeQueue = new Queue<int>();
    private int _lastLieIndex = -1;

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
        InitializeLieTypeQueue();

        // initialyze global Puzzels Infinity starirs is resolved or not 
        _objectStates[stairsPuzzleID] = false;
        _objectStates[stairsStartPuzzelID] = false;
        
        
        LoadStates(); // Load any saved states
    }


    // Initialize or reshuffle the lie type queue
    public void InitializeLieTypeQueue()
    {
        // Create list of all lie indices
        List<int> lieIndices = new List<int>() { 0, 1, 2, 3 };
        
        // Fisher-Yates shuffle
        for (int i = lieIndices.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = lieIndices[i];
            lieIndices[i] = lieIndices[j];
            lieIndices[j] = temp;
        }

        // Create queue
        _lieTypeQueue = new Queue<int>(lieIndices);
        Debug.Log($"Lie queue reshuffled: {string.Join(",", _lieTypeQueue)}");
    }


     // Get next lie type index
    public int GetNextLieIndex()
    {
        // If queue is empty or would repeat, reshuffle
        if (_lieTypeQueue.Count == 0 || 
           (_lieTypeQueue.Peek() == _lastLieIndex && _lieTypeQueue.Count > 1))
        {
            InitializeLieTypeQueue();
        }

        // Special case: if only 1 type remains and it would repeat,
        // dequeue and immediately requeue it
        if (_lieTypeQueue.Peek() == _lastLieIndex)
        {
            int temp = _lieTypeQueue.Dequeue();
            _lieTypeQueue.Enqueue(temp);
        }

        _lastLieIndex = _lieTypeQueue.Dequeue();
        return _lastLieIndex;
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
            _infiniteLoopSolvedCount = 0;
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