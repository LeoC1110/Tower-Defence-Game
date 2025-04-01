using System.IO;
using UnityEngine;
using System.Collections.Generic;

public static class SaveSystem
{
    private static string SaveDir => Application.persistentDataPath + "/saves/";

    public static void SaveGame(SaveData data)
    {
        if (!Directory.Exists(SaveDir)) Directory.CreateDirectory(SaveDir);
        string path = SaveDir + data.id + ".json";
        File.WriteAllText(path, JsonUtility.ToJson(data, true));
    }

    public static List<SaveData> LoadAllSaves()
    {
        List<SaveData> saves = new List<SaveData>();
        if (!Directory.Exists(SaveDir)) return saves;
        foreach (string file in Directory.GetFiles(SaveDir, "*.json"))
        {
            string json = File.ReadAllText(file);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            saves.Add(data);
        }
        return saves;
    }

    public static void DeleteSave(string id)
    {
        string path = SaveDir + id + ".json";
        if (File.Exists(path)) File.Delete(path);
    }

    public static SaveData LoadById(string id)
    {
        string path = SaveDir + id + ".json";
        if (!File.Exists(path)) return null;
        return JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
    }
}