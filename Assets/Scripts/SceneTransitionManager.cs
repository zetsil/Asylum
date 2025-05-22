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
                
                // #if UNITY_EDITOR
                if (_instance == null && Application.isPlaying)
                {
                    Debug.LogWarning("SceneTransitionManager auto-created for editor testing");
                    GameObject go = new GameObject("SceneTransitionManager (Editor-Temporary)");
                    _instance = go.AddComponent<SceneTransitionManager>();
                }
                // #endif
            }
            return _instance;
        }
    }

    private DoorData _currentTransition;
    private bool _isTransitioning = false; // Flag to track transition state

    
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

        if (_isTransitioning)
        {
            Debug.LogWarning("Scene transition already in progress - ignoring request");
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
        _isTransitioning = true; // Set flag at start of transition
        
        try
        {
            // Step 1: Load Transition scene additively
            AsyncOperation transitionLoad = SceneManager.LoadSceneAsync("Transition", LoadSceneMode.Additive);
            yield return new WaitUntil(() => transitionLoad.isDone);

            // Step 2: Get the TransitionController
            GameObject transitionRoot = GameObject.Find("TransitionCanvas");
            TransitionController transition = transitionRoot.GetComponent<TransitionController>();
            Camera transitionCamera = GameObject.Find("TransitionCamera").GetComponent<Camera>();
            transitionCamera.gameObject.SetActive(false);

            // Step 3: Fade In (black screen hides current scene)
            yield return transition.StartCoroutine(transition.FadeIn(1f));
            yield return new WaitForSeconds(1f);

            // Step 4: Unload previous scene (except Transition)
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.name != "Transition") {
                yield return SceneManager.UnloadSceneAsync(activeScene);
            }

            // Step 5: Load the new scene additively
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            asyncLoad.allowSceneActivation = false;
            transitionCamera.gameObject.SetActive(true);
            while (asyncLoad.progress < 0.9f)
            {
                float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
                Debug.Log($"Loading progress: {progress * 100}%");
                yield return null;
            }

            asyncLoad.allowSceneActivation = true;
            yield return new WaitUntil(() => asyncLoad.isDone);
            yield return null;
            transitionCamera.gameObject.SetActive(false);
            PositionPlayer();
            // Step 6: Set the new scene as active
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

            // Step 7: Fade Out (reveal new scene)
            yield return transition.StartCoroutine(transition.FadeOut(1f));

            // Step 8: Unload transition scene
            yield return SceneManager.UnloadSceneAsync("Transition");
        }
        finally
        {
            _isTransitioning = false; // Ensure flag is reset even if something fails
        }
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

    // #if UNITY_EDITOR
    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
    // #endif
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