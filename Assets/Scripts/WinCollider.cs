using UnityEngine;
using UnityEngine.SceneManagement;

public class WinCollider : MonoBehaviour
{
    public BoxCollider boxTriggerCollider;
    public string tomeID; // A unique identifier for the tome, e.g., the scene name or a specific string

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player touched the win trigger and collected a tome!");
            string currentScene = SceneManager.GetActiveScene().name;

            // Save progress if not already saved
            if (!GameManagers.Instance.playerData.levelsCompleted.Contains(currentScene))
            {
                GameManagers.Instance.playerData.levelsCompleted.Add(currentScene);
                GameManagers.Instance.SaveGame();
            }

            // Track collected tome if not already collected
            if (!GameManagers.Instance.playerData.tomesCollected.Contains(tomeID))
            {
                GameManagers.Instance.playerData.tomesCollected.Add(tomeID);
                GameManagers.Instance.SaveGame();
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene("PlayerHUB");
            
        }
    }
}