using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private Text hpText;
    [SerializeField] private Text goldText;
    [SerializeField] private Text timeText;
    [SerializeField] private Text messageText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Validate UI elements
        ValidateUIElements();

        // Subscribe to scene loaded event to re-initialize UI
        SceneManager.sceneLoaded += OnSceneLoaded;

        InitializeUI();
    }

    private void OnDestroy()
    {
        // Unsubscribe from scene loaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void ValidateUIElements()
    {
        if (!hpText) Debug.LogError("UIManager: hpText is not assigned in the Inspector!");
        if (!goldText) Debug.LogError("UIManager: goldText is not assigned in the Inspector!");
        if (!timeText) Debug.LogError("UIManager: timeText is not assigned in the Inspector!");
        if (!messageText) Debug.LogError("UIManager: messageText is not assigned in the Inspector!");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-initialize UI when a new scene loads
        InitializeUI();
    }

    private void InitializeUI()
    {
        if (messageText) messageText.gameObject.SetActive(false);
        UpdateHPUI();
        UpdateGoldUI();
        UpdateTimeUI();
        ShowMessage("Let's FIGHT!", 3f);
    }

    private void Update()
    {
        // Stop updating time if the game is over
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            return;

        UpdateTimeUI();
    }

    private void UpdateTimeUI()
    {
        if (!timeText || GameManager.Instance == null) return;
        float time = GameManager.Instance.GameTime;
        timeText.text = $"Time: {Mathf.FloorToInt(time / 60f):00}:{Mathf.FloorToInt(time % 60f):00}";
    }

    public void UpdateHPUI()
    {
        if (!hpText || GameManager.Instance == null) return;

        // Get health from BaseUnit components
        int playerHealth = 0;
        int enemyHealth = 0;

        if (GameManager.Instance.PlayerBase != null)
        {
            BaseUnit playerBaseUnit = GameManager.Instance.PlayerBase.GetComponent<BaseUnit>();
            if (playerBaseUnit != null)
                playerHealth = playerBaseUnit.Health;
        }

        if (GameManager.Instance.EnemyBase != null)
        {
            BaseUnit enemyBaseUnit = GameManager.Instance.EnemyBase.GetComponent<BaseUnit>();
            if (enemyBaseUnit != null)
                enemyHealth = enemyBaseUnit.Health;
        }

        hpText.text = $"Player HP: {playerHealth} | Enemy HP: {enemyHealth}";
    }

    public void UpdateGoldUI()
    {
        if (!goldText || EconomyManager.Instance == null) return;
        goldText.text = $"Gold: {EconomyManager.Instance.PlayerGold}";
    }

    public void ShowMessage(string msg, float duration = 2f)
    {
        if (!messageText) return;
        StopAllCoroutines();
        StartCoroutine(ShowMessageCoroutine(msg, duration));
    }

    private IEnumerator ShowMessageCoroutine(string msg, float duration)
    {
        messageText.text = msg;
        messageText.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        messageText.text = string.Empty;
        messageText.gameObject.SetActive(false);
    }

    // Called by GameManager when the game ends
    public void ShowGameOverMessage(string result)
    {
        string message = result switch
        {
            "Victory" => "Victory!",
            "GameOver" => "Game Over!",
            "Defeat" => "Defeat!",
            _ => "Game Ended!"
        };
        ShowMessage(message, 3f);
    }
}