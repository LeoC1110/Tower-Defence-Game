using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game State")]
    [SerializeField] public float gameTime = 180f;

    [Header("Base References")]
    [SerializeField] private GameObject playerBase; // Reference to PlayerBase GameObject
    [SerializeField] private GameObject enemyBase;  // Reference to EnemyBase GameObject

    private bool isGameOver = false;
    private BaseUnit[] allUnits; // Cache all units for disabling on game over

    public float GameTime => gameTime;
    public bool IsGameOver => isGameOver;

    // Provide access to base references for BaseUnit and UIManager
    public GameObject PlayerBase => playerBase;
    public GameObject EnemyBase => enemyBase;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // Find base references if not assigned
        if (playerBase == null)
            playerBase = GameObject.FindWithTag("PlayerBase");
        if (enemyBase == null)
            enemyBase = GameObject.FindWithTag("EnemyBase");

        if (playerBase == null || enemyBase == null)
            Debug.LogError("PlayerBase or EnemyBase not found in scene!");

        // Cache all units
        allUnits = FindObjectsByType<BaseUnit>(FindObjectsSortMode.None);

        UIManager.Instance?.UpdateHPUI();
        // Remove test damage since we'll apply damage directly to BaseUnit
    }

    private void Update()
    {
        if (isGameOver) return;

        if (gameTime > 0f)
        {
            gameTime -= Time.deltaTime;
        }
        else
        {
            gameTime = 0f;
            EndGame("Defeat");
            return;
        }

        // Check win/lose conditions based on base health
        BaseUnit playerBaseUnit = playerBase?.GetComponent<BaseUnit>();
        BaseUnit enemyBaseUnit = enemyBase?.GetComponent<BaseUnit>();

        if (playerBaseUnit != null && playerBaseUnit.Health <= 0)
        {
            EndGame("GameOver");
        }
        else if (enemyBaseUnit != null && enemyBaseUnit.Health <= 0)
        {
            EndGame("Victory");
        }
    }

    // Apply damage directly to the base's BaseUnit component
    public void TakeDamage(bool isPlayer, int damage)
    {
        if (isGameOver) return;

        GameObject targetBase = isPlayer ? playerBase : enemyBase;
        if (targetBase == null)
        {
            Debug.LogError($"TakeDamage: {(isPlayer ? "PlayerBase" : "EnemyBase")} is null!");
            return;
        }

        BaseUnit baseUnit = targetBase.GetComponent<BaseUnit>();
        if (baseUnit != null)
        {
            baseUnit.TakeDamage(damage);
            Debug.Log($"Damage applied to {(isPlayer ? "PlayerBase" : "EnemyBase")}: Health = {baseUnit.Health}");

            // if the target base is destroyed, update the reference
            if (baseUnit.Health <= 0)
            {
                if (isPlayer)
                    playerBase = null;
                else
                    enemyBase = null;
            }
        }
        else
        {
            Debug.LogError($"TakeDamage: BaseUnit component not found on {(isPlayer ? "PlayerBase" : "EnemyBase")}!");
        }

        // update the UI
        UIManager.Instance?.UpdateHPUI();
    }

    // End the game and load the specified scene
    private void EndGame(string sceneName)
    {
        if (isGameOver) return;
        isGameOver = true;

        // Stop all audio sources
        AudioSource[] audioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (var source in audioSources)
        {
            source.Stop();
        }

        Debug.Log($"Game Over: {sceneName}");

        // Show game over message
        UIManager.Instance?.ShowGameOverMessage(sceneName);

        // Disable all units to prevent further updates
        foreach (var unit in allUnits)
        {
            if (unit != null)
                unit.gameObject.SetActive(false);
        }

        // Delay scene loading to allow the message to be visible
        StartCoroutine(LoadSceneAfterDelay(sceneName, 3f));
    }

    private IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
