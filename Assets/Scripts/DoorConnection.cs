using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "NewDoorConnection", menuName = "Scene Connections/Door Connection")]
public class DoorConnection : ScriptableObject
{
    public string doorID; // this alwys must be unique
    public SceneDoor fromDoor;
    public SceneDoor toDoor;
    public bool locked;
    [SerializeField, HideInInspector] private bool initialLockedValue;

}

[System.Serializable]
public class SceneDoor
{
    public string sceneName;
    public string doorID; // Unique per scene
    public Vector3 spawnOffset = new Vector3(2f, 0, 0); // Default spawn 2 units right of door
}