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
            string currentScene = SceneManager.GetActiveScene().name;

            // Save progress if not already saved
            if (!GameManager.Instance.playerData.levelsCompleted.Contains(currentScene))
            {
                GameManager.Instance.playerData.levelsCompleted.Add(currentScene);
                GameManager.Instance.SaveGame();
            }

            // Track collected tome if not already collected
            if (!GameManager.Instance.playerData.tomesCollected.Contains(tomeID))
            {
                GameManager.Instance.playerData.tomesCollected.Add(tomeID);
                GameManager.Instance.SaveGame();
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene("PlayerHUB");
            Debug.Log("Player touched the win trigger and collected a tome!");
        }
    }
}