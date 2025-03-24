using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Text hpText; 
    public Text goldText;
    public Text timeText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        UpdateHPUI();
        UpdateGoldUI();
    }

    void Update()
    {
        UpdateTimeUI();
    }

    public void UpdateTimeUI()
    {
        if (GameManager.Instance != null)
        {
            int minutes = Mathf.FloorToInt(GameManager.Instance.gameTime / 60);
            int seconds = Mathf.FloorToInt(GameManager.Instance.gameTime % 60);
            timeText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    public void UpdateHPUI()
    {
        if (GameManager.Instance != null)
        {
            Debug.Log($"Updating UI: Player HP: {GameManager.Instance.playerHP}, Enemy HP: {GameManager.Instance.enemyHP}");

            if (hpText != null)
            {
                hpText.text = $"HP: {GameManager.Instance.playerHP} | Enemy HP: {GameManager.Instance.enemyHP}";
            }
            else
            {
                Debug.LogError("hpText is NULL! Assign it in the Inspector.");
            }
        }
    }

    public void UpdateGoldUI()
    {
        if (EconomyManager.Instance != null)
        {
            goldText.text = "GOLD: " + EconomyManager.Instance.playerGold;
        }
    }
}
