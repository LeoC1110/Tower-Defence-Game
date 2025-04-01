using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingSceneController : MonoBehaviour
{
    public void OnBackButtonPressed()
    {
        SceneManager.LoadScene("Main Screen");
    }
}
