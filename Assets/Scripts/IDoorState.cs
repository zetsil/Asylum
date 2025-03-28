using UnityEngine;

public interface IDoorState
{
    void HandleOpen(Door door);
    void HandleUnlock(Door door);
}

// Locked State
public class LockedState : IDoorState
{
    public void HandleOpen(Door door)
    {
        door.HandleLockedState();
    }

    public void HandleUnlock(Door door)
    {
        Debug.Log("Start Unlocking the door.");
        door.SetState(new OpenState());
    }
}

// Open State
public class OpenState : IDoorState
{
    public void HandleOpen(Door door)
    {
        Debug.Log("The door is already open go to next room");
    }

    public void HandleUnlock(Door door)
    {
        Debug.Log("The door is already open and unlocked");
    }

}