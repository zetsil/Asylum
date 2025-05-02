using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    private static SceneTransitionManager _instance;
    public static SceneTransitionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SceneTransitionManager>();
                
                #if UNITY_EDITOR
                if (_instance == null && Application.isPlaying)
                {
                    Debug.LogWarning("SceneTransitionManager auto-created for editor testing");
                    GameObject go = new GameObject("SceneTransitionManager (Editor-Temporary)");
                    _instance = go.AddComponent<SceneTransitionManager>();
                }
                #endif
            }
            return _instance;
        }
    }

    private DoorData _currentTransition;
    
    private void Awake()
    {
        // Handle duplicate instances
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning($"Duplicate SceneTransitionManager on {gameObject.name} - destroying");
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void SetTransitionData(DoorData data)
    {
        _currentTransition = data;
    }
    
    public void LoadScene(string sceneName)
    {
        if (!Application.isPlaying)
        {
            Debug.LogError("Cannot load scenes in edit mode!");
            return;
        }

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Invalid scene name");
            return;
        }

        StartCoroutine(LoadSceneAsync(sceneName));
    }
    
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Show loading screen/transition here
        Debug.Log($"Loading scene: {sceneName}");
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;
        
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            // Progress from 0-0.9 (1.0 happens after activation)
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            Debug.Log($"Loading progress: {progress * 100}%");
            
            // When loading is almost complete
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            
            yield return null;
        }
        
        PositionPlayer();
    }
    
    private void PositionPlayer()
    {
        if (_currentTransition == null)
        {
            Debug.LogWarning("No transition data available");
            return;
        }
        
        var spawnPoints = FindObjectsOfType<DoorSpawnPoint>();
        DoorSpawnPoint matchingPoint = null;
        
        foreach (var point in spawnPoints)
        {
            if (point.prevSceneName == _currentTransition.fromScene &&
                point.prevDoorID == _currentTransition.fromDoorID)
            {
                matchingPoint = point;
                break;
            }
        }
        
        if (matchingPoint != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = matchingPoint.transform.position + matchingPoint.spawnOffset;
            }
            else
            {
                Debug.LogWarning("Player object not found in scene");
            }
        }
        else
        {
            Debug.LogWarning($"No matching spawn point found for door {_currentTransition.fromDoorID} from {_currentTransition.fromScene}");
        }
    }

    #if UNITY_EDITOR
    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
    #endif
}

[System.Serializable]
public class DoorData
{
    public string fromScene;
    public string fromDoorID;
    public string toScene;
    
    public DoorData(string fromScene, string fromDoorID, string toScene)
    {
        this.fromScene = fromScene;
        this.fromDoorID = fromDoorID;
        this.toScene = toScene;
    }
}