using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingButton : MonoBehaviour
{
    public string settingSceneName = "Setting";

    // Open setting scene additively and pause game
    public void OpenSetting()
    {
        // Prevent duplicate loading
        if (!SceneManager.GetSceneByName(settingSceneName).isLoaded)
        {
            SceneManager.LoadScene(settingSceneName, LoadSceneMode.Additive);
        }

        Time.timeScale = 0f; // Pause game
    }

    // Close setting scene and resume game
    public void CloseSetting()
    {
        if (SceneManager.GetSceneByName(settingSceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(settingSceneName);
        }

        Time.timeScale = 1f; // Resume game
    }
}
