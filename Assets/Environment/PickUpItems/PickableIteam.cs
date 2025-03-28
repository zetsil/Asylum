using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Item", menuName = "Inventory/Pickable Item", order = 1)]
public class PickableItem : ScriptableObject
{
    [Header("Basic Information")]
    [SerializeField] private string _id = System.Guid.NewGuid().ToString();
    [SerializeField] private string _name = "New Item";
    [TextArea(3, 5)]
    [SerializeField] private string _description = "Item description";
    [SerializeField] private Sprite _icon;
    [SerializeField] private bool _isPicked;
    
    #if UNITY_EDITOR
    private void OnValidate()
    {
        // Ensure ID is always unique and not empty
        if (string.IsNullOrEmpty(_id))
        {
            _id = System.Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        // Ensure name is not empty
        if (string.IsNullOrEmpty(_name))
        {
            _name = "New Item";
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
    #endif

    /// <summary>
    /// Marks the item as picked or unpicked
    /// </summary>
    public void SetPickedState(bool picked)
    {
        if (_isPicked == picked) return;
        
        _isPicked = picked;
        
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }


    /// <summary>
    /// Resets the item to unpicked state
    /// </summary>
    public void ResetPickState()
    {
        SetPickedState(false);
    }

    public string getID(){
        return _id;
    }

    /// <summ
    
}