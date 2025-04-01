using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;

    private bool isPaused = false;

    private void Start()
    {
        if (settingsPanel) settingsPanel.SetActive(false);
    }

    public void TogglePause()
    {
        if (settingsPanel == null) return;

        isPaused = !isPaused;
        settingsPanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void ResumeGame()
    {
        if (settingsPanel == null) return;

        isPaused = false;
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
