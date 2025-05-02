using UnityEngine;

public class ClasroomKey : MonoBehaviour
{
    void Start()
    {
        bool isEnabled = GameStateManager.Instance.GetObjectState("ClassroomKey");

        if (isEnabled)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        
    }
}