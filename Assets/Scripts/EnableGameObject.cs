using UnityEngine;

public class EnableGameObject : MonoBehaviour
{
    public void SetObjectEnabled(GameObject key)
    {
        bool isEnabled = GameStateManager.Instance.GetObjectState("ClassroomKey");
        if (isEnabled)
        {
            key.SetActive(true);
            Debug.Log("Obiectul " + key.name + " a fost activat.");
        }
        else
        {
            Debug.LogError("Referința către GameObject este nulă!");
        }
    }
}