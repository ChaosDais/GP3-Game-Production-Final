using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadWin : MonoBehaviour
{
    
    public string[] requiredTomes = { "Courtyard", "Vault" };

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CheckTomesAndLoadScene();
        }
    }

    private void CheckTomesAndLoadScene()
    {
        if (GameManagers.Instance == null || GameManagers.Instance.playerData == null) return;

        bool hasAllTomes = true;
        foreach (string tomeId in requiredTomes)
        {
            if (!GameManagers.Instance.playerData.tomesCollected.Contains(tomeId))
            {
                hasAllTomes = false;
                break;
            }
        }

        if (hasAllTomes)
        {
            SceneManager.LoadScene("WinScreen");
        }
    }
}
