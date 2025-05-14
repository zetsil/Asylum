using UnityEngine;

public class ClasroomKey : MonoBehaviour
{
    public GameObject refrenceKey ;
    void Start()
    {
        bool isEnabled = GameStateManager.Instance.GetObjectState("ClassroomKey");

        if (isEnabled)
        {
            Debug.Log("yeeeee");
            refrenceKey.SetActive(true);
        }
        else
        {
            Debug.Log("nuuuu");
            refrenceKey.SetActive(false);
        }
    }

    void Update()
    {
        
    }
}