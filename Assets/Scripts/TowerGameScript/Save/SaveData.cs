using System;

[System.Serializable]
public class SaveData
{
    public string sceneName;
    public float gameTime;
    public int playerGold;
    public string realTimeString; // human-readable
    public string id; // unique identifier for save file   
}

