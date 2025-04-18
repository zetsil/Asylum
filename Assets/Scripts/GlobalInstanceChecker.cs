using UnityEngine;

public class GlobalInstanceChecker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameStateManager manager = GameStateManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
