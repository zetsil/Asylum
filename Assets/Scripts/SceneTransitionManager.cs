using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    private static SceneTransitionManager _instance;
    private bool _isStrangeDoor = false;
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
    public bool isTransitioning = false; // Flag to track transition state

    
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

        if (isTransitioning)
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
    
    public void LoadStrangeDoorScene(string sceneName)
    {
        if (!Application.isPlaying || isTransitioning)
        {
            Debug.LogWarning("Transition already in progress or not in play mode.");
            return;
        }

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Invalid scene name for Strange Door.");
            return;
        }

        // Setează flag-ul pentru a indica tranziția specială
        _isStrangeDoor = true;
        
        // Rulează corutina standard de încărcare
        StartCoroutine(LoadSceneAsync(sceneName));
    }
    
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        isTransitioning = true;
        bool wasStrangeDoor = _isStrangeDoor;
        
        try
        {
            // Step 1 & 2: Load Transition scene and get components (OK)
            AsyncOperation transitionLoad = SceneManager.LoadSceneAsync("Transition", LoadSceneMode.Additive);
            yield return new WaitUntil(() => transitionLoad.isDone);

            GameObject transitionRoot = GameObject.Find("TransitionCanvas");
            TransitionController transition = transitionRoot.GetComponent<TransitionController>();
            Camera transitionCamera = GameObject.Find("TransitionCamera").GetComponent<Camera>();
            
            
            // NOU: Step A: Pregatirea: Activeaza Camera de Tranzitie
            // Camera veche (din scena descarcata) devine inactiva automat.
            transitionCamera.gameObject.SetActive(true);
            
            // Step 3: Fade In (ecranul devine negru afisat de Camera NOUA)
            // Daca scena de tranzitie incepe cu imaginea transparenta, acest pas este esential.
            yield return transition.StartCoroutine(transition.FadeIn(1f));
            yield return new WaitForSeconds(0.5f); // Scurteaza putin pauza


            // NOU: LOGICA FLASH-ULUI (Se întâmplă pe ecranul Negru, ÎNAINTE de a descărca vechea scenă)
            // Nu mai avem camera veche care sa incurce. Camera de Tranziție afișează negrul.
            if (wasStrangeDoor)
            {
                // FlashWhite transforma ecranul Alb, apoi il lasa Negru.
                yield return transition.StartCoroutine(transition.FlashWhite(0.15f));
            }


            // Step 4: Unload previous scene (ACUM e sigur, Camera de Tranzitie e activa)
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.name != "Transition")
            {
                // Camera veche este descarcata, dar Camera de Tranzitie preia controlul.
                yield return SceneManager.UnloadSceneAsync(activeScene); 
            }

            // Step 5: Load the new scene additively (RĂMÂNE LA FEL)
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            asyncLoad.allowSceneActivation = false;
            
            // NU MAI E NEVOIE de transitionCamera.gameObject.SetActive(true) AICI
            // deoarece a fost activata la Step A.
            
            while (asyncLoad.progress < 0.9f)
            {
                float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
                Debug.Log($"Loading progress: {progress * 100}%");
                yield return null;
            }

            asyncLoad.allowSceneActivation = true;
            yield return new WaitUntil(() => asyncLoad.isDone);
            yield return null;
            
            // NOU: Dezactivarea camerei de tranzitie are loc DUPA ce scena e activa.
            transitionCamera.gameObject.SetActive(false); 
            
            PositionPlayer();
            
            // Step 6 & 7: Set active scene and Fade Out (OK)
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            yield return transition.StartCoroutine(transition.FadeOut(1f));

            // Step 8: Unload transition scene (OK)
            yield return SceneManager.UnloadSceneAsync("Transition");
        }
        finally
        {
            isTransitioning = false;
            _isStrangeDoor = false;
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