using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management operations

public class ButtonManager : MonoBehaviour
{
    // --- Public Methods for Button Interaction ---

    /// <summary>
    /// Loads a specified game scene, prioritizing a SceneTransitionManager if available.
    /// Attach this method to your "Start Game" button's OnClick() event.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load (e.g., "GameScene", "Level1").</param>
    public void StartGame(string sceneName)
    {
        Debug.Log($"Attempting to load scene: {sceneName}"); // Log for debugging
        try
        {
            // Check if a SceneTransitionManager instance exists
            // This assumes SceneTransitionManager is a singleton with a static 'Instance' property
            if (SceneTransitionManager.Instance != null)
            {
                Debug.Log($"Using SceneTransitionManager to load scene: {sceneName}");
                // This line directly uses the SceneTransitionManager, consistent with your Door script.
                SceneTransitionManager.Instance.LoadScene(sceneName);
            }
            else
            {
                Debug.LogWarning("SceneTransitionManager.Instance not found. Falling back to direct SceneManager.LoadScene.");
                // Fallback: Load the scene directly if no transition manager is present
                SceneManager.LoadScene(sceneName);
            }
        }
        catch (System.Exception e)
        {
            // Log any errors that occur during scene loading
            Debug.LogError($"Failed to load scene '{sceneName}': {e.Message}");
            // If the SceneTransitionManager or its LoadScene method caused an error,
            // you might want to consider a more robust fallback or error handling here.
        }
    }

    /// <summary>
    /// Exits the game application.
    /// Attach this method to your "Exit Game" button's OnClick() event.
    /// </summary>
    public void ExitGame()
    {
        Debug.Log("Exiting game..."); // Log for debugging

        // Check if the application is running in the Unity Editor
        #if UNITY_EDITOR
            // If in editor, stop playing the editor
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // If in a built application, quit the application
            Application.Quit();
        #endif
    }
}