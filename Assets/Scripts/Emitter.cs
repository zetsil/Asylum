using System.Collections.Generic;
using UnityEngine;

public class Emitter : MonoBehaviour
{
    // List of observers
    private List<IObserver> observers = new List<IObserver>();

    // Add an observer to the list
    public void AddObserver(IObserver observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
            Debug.Log($"{name}: Observer added: {observer}");
        }
    }

    // Remove an observer from the list
    public void RemoveObserver(IObserver observer)
    {
        if (observers.Contains(observer))
        {
            observers.Remove(observer);
            Debug.Log($"{name}: Observer removed: {observer}");
        }
    }

    // Notify all observers
    public void NotifyObservers(string message)
    {
        foreach (var observer in observers)
        {
            observer.HandleEvent(message);
        }
    }

    // Example: Emit an event when a key is pressed
    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         NotifyObservers("Spacebar pressed!");
    //     }
    // }
}