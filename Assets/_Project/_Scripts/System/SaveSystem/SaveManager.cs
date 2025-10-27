using UnityEngine;
using System.IO;
using System.Text;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private string saveFilePath;
    private const string EncryptionKey = "your-secret-key";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            saveFilePath = Path.Combine(Application.persistentDataPath, "savedata.json");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private string EncryptDecrypt(string data)
    {
        StringBuilder result = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
        {
            result.Append((char)(data[i] ^ EncryptionKey[i % EncryptionKey.Length]));
        }
        return result.ToString();
    }

    public void SaveGame(SaveData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);

            if (!Application.isEditor)
            {
                json = EncryptDecrypt(json);
            }

            File.WriteAllText(saveFilePath, json);
            Debug.Log("Game data saved to: " + saveFilePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save game data: " + e.Message);
        }
    }

    public SaveData LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            try
            {
                string json = File.ReadAllText(saveFilePath);

                if (!Application.isEditor)
                {
                    json = EncryptDecrypt(json);
                }
                
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                Debug.Log("Game data loaded from: " + saveFilePath);
                return data;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to load save data due to an exception: " + e.Message);
                Debug.LogWarning("Creating new save data as a fallback.");
                return new SaveData();
            }
        }
        else
        {
            Debug.LogWarning("Save file not found. Creating new save data.");
            return new SaveData();
        }
    }

    public void DeleteSaveData()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Save data deleted.");
        }
    }
}