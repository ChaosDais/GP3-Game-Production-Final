using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameManagers : MonoBehaviour
{
    public static GameManagers Instance;
    public PlayerData playerData;

    private string saveFile;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        saveFile = Application.persistentDataPath + "/save.dat";
        LoadGame();
    }

    public void SaveGame()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(saveFile, FileMode.Create);
        formatter.Serialize(stream, playerData);
        stream.Close();
    }

    public void LoadGame()
    {
        if (File.Exists(saveFile))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(saveFile, FileMode.Open);
            playerData = formatter.Deserialize(stream) as PlayerData;
            stream.Close();
        }
        else
        {
            playerData = new PlayerData();
        }
    }
}
