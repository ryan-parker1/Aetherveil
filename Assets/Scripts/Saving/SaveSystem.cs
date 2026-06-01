using System.IO;
using UnityEngine;

// Static utility — handles reading and writing JSON to disk.
// Save file lives in Application.persistentDataPath/save.json
public static class SaveSystem
{
    private static string SavePath =>
        Path.Combine(Application.persistentDataPath, "save.json");

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(SavePath, json);
        Debug.Log("Game saved to: " + SavePath);
    }

    public static SaveData Load()
    {
        if (!SaveExists())
        {
            Debug.Log("No save file found.");
            return null;
        }

        string json = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        Debug.Log("Game loaded from: " + SavePath);
        return data;
    }

    public static bool SaveExists() => File.Exists(SavePath);

    public static void DeleteSave()
    {
        if (SaveExists())
        {
            File.Delete(SavePath);
            Debug.Log("Save file deleted.");
        }
    }
}
