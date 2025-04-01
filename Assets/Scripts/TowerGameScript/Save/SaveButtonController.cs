using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveButtonController : MonoBehaviour
{
    public void SaveCurrentGame()
    {
        SaveData data = new SaveData
        {
            sceneName = SceneManager.GetActiveScene().name,
            gameTime = GameManager.Instance != null ? GameManager.Instance.GameTime : 0f,
            playerGold = EconomyManager.Instance != null ? EconomyManager.Instance.PlayerGold : 0,
            realTimeString = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            id = Guid.NewGuid().ToString()
        };

        SaveSystem.SaveGame(data); 
        Debug.Log("Game saved");
    }
}

