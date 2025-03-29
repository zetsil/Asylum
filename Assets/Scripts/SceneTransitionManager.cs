using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;
    
    private DoorData _currentTransition;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void SetTransitionData(DoorData data)
    {
        _currentTransition = data;
    }
    
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }
    
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Your loading screen/show transition
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        PositionPlayer();
    }
    
    private void PositionPlayer()
    {
        if (_currentTransition == null) return;
        
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
        }
    }
}

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