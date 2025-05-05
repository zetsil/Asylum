using UnityEngine;

public class VisibilityController : MonoBehaviour
{
    public GameObject targetObject;

    // Make the object visible
    public void SetVisible()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No target object assigned in VisibilityController", this);
        }
    }

    // Hide the object
    public void SetHide()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("No target object assigned in VisibilityController", this);
        }
    }
}