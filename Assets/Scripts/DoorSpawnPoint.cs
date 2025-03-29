using UnityEngine;

public class DoorSpawnPoint : MonoBehaviour
{
    public string prevSceneName; // Scene this door connects FROM
    public string prevDoorID;    // Must match the connecting door's ID
    public Vector3 spawnOffset;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position + spawnOffset, 0.5f);
        Gizmos.DrawLine(transform.position, transform.position + spawnOffset);
    }
}