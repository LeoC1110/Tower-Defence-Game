using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private void StopCurrentBGM()
    {
        // safe call: only call StopBGM when AudioManager exists
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBGM();
        }
    }

    public void LoadEasyScene()
    {
        StopCurrentBGM();
        SceneManager.LoadScene("Easy");
    }

    public void LoadSettingsScene()
    {
        StopCurrentBGM();
        SceneManager.LoadScene("Setting");
    }

    public void LoadMainScreenScene()
    {
        StopCurrentBGM();
        SceneManager.LoadScene("Main Screen");
    }

    public void LoadLoadScene()
    {
        StopCurrentBGM();
        SceneManager.LoadScene("Load");
    }

    public void ExitGame()
    {
        Debug.Log("Exiting the game..."); 
        Application.Quit(); 
    }
}
