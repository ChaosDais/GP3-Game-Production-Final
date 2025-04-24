using UnityEngine;

public class TomeManager : MonoBehaviour
{
    [System.Serializable]
    public class TomeDisplay
    {
        public string tomeId;           // "Courtyard", "Crypt", or "Vault"
        public GameObject hubTome;      // The tome GameObject to show in the hub
    }

    public TomeDisplay[] tomeDisplays;

    private void Start()
    {
        UpdateTomeDisplays();
    }

    private void OnEnable()
    {
        UpdateTomeDisplays();
    }

    private void UpdateTomeDisplays()
    {
        if (GameManagers.Instance == null || GameManagers.Instance.playerData == null) return;

        foreach (TomeDisplay tome in tomeDisplays)
        {
            if (tome.hubTome != null)
            {
                // Show the tome only if it's been collected
                bool isCollected = GameManagers.Instance.playerData.tomesCollected.Contains(tome.tomeId);
                tome.hubTome.SetActive(isCollected);
            }
        }
    }

    // Call this method when a tome is collected
    public void CollectTome(string tomeId)
    {
        if (!GameManagers.Instance.playerData.tomesCollected.Contains(tomeId))
        {
            GameManagers.Instance.playerData.tomesCollected.Add(tomeId);
            GameManagers.Instance.SaveGame();
            UpdateTomeDisplays();
        }
    }
}
