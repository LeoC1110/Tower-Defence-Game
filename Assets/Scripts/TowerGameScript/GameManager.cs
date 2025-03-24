using UnityEngine;
using UnityEngine.SceneManagement; // Import Scene Management

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int playerHP = 100;
    public int enemyHP = 100;
    public float gameTime = 180f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        UIManager.Instance.UpdateHPUI(); // Ensure UI is updated on game start
    }

    void Update()
    {
        if (gameTime > 0)
        {
            gameTime -= Time.deltaTime;
        }
        else
        {
            gameTime = 0; // Prevent negative values
            EndGame();
        }

        // Check if Player HP or Enemy HP reaches 0
        if (playerHP <= 0 || enemyHP <= 0)
        {
            EndGame();
        }
    }

    public void TakeDamage(bool isPlayer, int damage)
    {
        if (isPlayer)
        {
            playerHP -= damage;
            if (playerHP < 0) playerHP = 0;
        }
        else
        {
            enemyHP -= damage;
            if (enemyHP < 0) enemyHP = 0;
        }

        Debug.Log($"Player HP: {playerHP}, Enemy HP: {enemyHP}"); // Debugging log

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateHPUI(); // Update the combined HP UI text

        // Check for Game Over condition after taking damage
        if (playerHP <= 0 || enemyHP <= 0)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        Debug.Log("Game Over! Loading Game Over Scene...");
        SceneManager.LoadScene("GameOver"); 
    }
}
