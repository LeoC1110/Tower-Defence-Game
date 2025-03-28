using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform target;
    [SerializeField] private float waveDuration = 30f; // Duration of each wave (seconds)

    [Header("Enemy Types")]
    [SerializeField] private string[] enemyTags = { "AICharriot", "AICatapult" };

    private float timeSinceLastWave = 0f;
    private int currentWave = 0;

    private void Start()
    {
        if (spawnPoint == null || target == null)
        {
            Debug.LogError("EnemyAI is missing spawnPoint or target!");
            return;
        }

        StartCoroutine(SpawnWaves());
    }

    // Coroutine to manage wave-based spawning
    private IEnumerator SpawnWaves()
    {
        while (GameManager.Instance != null && !GameManager.Instance.IsGameOver)
        {
            timeSinceLastWave += Time.deltaTime;

            if (timeSinceLastWave >= waveDuration)
            {
                currentWave++;
                timeSinceLastWave = 0f;

                switch (currentWave)
                {
                    case 1:
                        yield return StartCoroutine(SpawnWave(2, 0, 10f)); // 2 AICharriots, 1 every 10 seconds
                        break;
                    case 2:
                        yield return StartCoroutine(SpawnWave(3, 0, 8f)); // 3 AICharriots, 1 every 8 seconds
                        break;
                    case 3:
                        yield return StartCoroutine(SpawnWave(2, 1, 8f)); // 2 AICharriots + 1 AICatapult, 1 every 8 seconds
                        break;
                    case 4:
                        yield return StartCoroutine(SpawnWave(3, 1, 6f)); // 3 AICharriots + 1 AICatapult, 1 every 6 seconds
                        break;
                    case 5:
                        yield return StartCoroutine(SpawnWave(4, 2, 5f)); // 4 AICharriots + 2 AICatapults, 1 every 5 seconds
                        break;
                    case 6:
                        yield return StartCoroutine(SpawnWave(5, 2, 4f)); // 5 AICharriots + 2 AICatapults, 1 every 4 seconds
                        break;
                    default:
                        yield return new WaitForSeconds(5f); // Wait 5 seconds after the last wave
                        break;
                }
            }

            yield return null;
        }
    }

    // Coroutine to spawn enemies for a single wave
    private IEnumerator SpawnWave(int charriotCount, int catapultCount, float spawnInterval)
    {
        int totalUnits = charriotCount + catapultCount;
        int spawnedUnits = 0;

        while (spawnedUnits < totalUnits)
        {
            // Spawn AICharriot first, then AICatapult
            string tagToSpawn = spawnedUnits < charriotCount ? enemyTags[0] : enemyTags[1];
            SpawnEnemy(tagToSpawn);
            spawnedUnits++;
            yield return new WaitForSeconds(spawnInterval);
        }

        // Wait 5 seconds after the wave ends
        yield return new WaitForSeconds(5f);
    }

    // Method to spawn a single enemy
    private void SpawnEnemy(string tagToSpawn)
    {
        if (enemyTags.Length == 0) return;

        Vector3 spawnPos = spawnPoint.position;
        spawnPos.y = 1.5f; // Ensure Y position is 1.5

        GameObject enemy = ObjectPoolManager.Instance.SpawnFromPool(tagToSpawn, spawnPos, Quaternion.identity);

        if (enemy == null)
        {
            Debug.LogWarning($"[EnemyAI] Failed to spawn enemy with tag: {tagToSpawn}");
            return;
        }

        enemy.tag = "EnemyUnit";

        // Face the target
        Vector3 direction = (target.position - enemy.transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            enemy.transform.rotation = Quaternion.LookRotation(direction);
        }

        if (enemy.TryGetComponent<BaseUnit>(out var baseUnit))
        {
            baseUnit.SetTarget(target);
        }
        else
        {
            Debug.LogWarning($"[EnemyAI] Spawned enemy {enemy.name} has no BaseUnit component");
        }

        Debug.Log($"[EnemyAI] Spawned {tagToSpawn} at {Time.time}");
    }
}