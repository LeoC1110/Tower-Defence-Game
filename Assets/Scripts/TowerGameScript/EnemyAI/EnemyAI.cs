using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public static EnemyAI Instance { get; private set; }

    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform target;
    [SerializeField] private float waveDuration = 30f; // Duration of each wave (seconds)

    [Header("Enemy Types")]
    [SerializeField] private string[] enemyTags = { "AICharriot", "AICatapult" };

    private float timeSinceLastWave = 0f;
    private int currentWave = 0;
    private Coroutine waveRoutine;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (spawnPoint == null || target == null)
        {
            Debug.LogError("EnemyAI is missing spawnPoint or target!");
            return;
        }

        waveRoutine = StartCoroutine(SpawnWaves());
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
                        yield return StartCoroutine(SpawnWave(2, 0, 10f));
                        break;
                    case 2:
                        yield return StartCoroutine(SpawnWave(3, 0, 8f));
                        break;
                    case 3:
                        yield return StartCoroutine(SpawnWave(2, 1, 8f));
                        break;
                    case 4:
                        yield return StartCoroutine(SpawnWave(3, 1, 6f));
                        break;
                    case 5:
                        yield return StartCoroutine(SpawnWave(4, 2, 5f));
                        break;
                    case 6:
                        yield return StartCoroutine(SpawnWave(5, 2, 4f));
                        break;
                    default:
                        yield return new WaitForSeconds(5f);
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
            string tagToSpawn = spawnedUnits < charriotCount ? enemyTags[0] : enemyTags[1];
            SpawnEnemy(tagToSpawn);
            spawnedUnits++;
            yield return new WaitForSeconds(spawnInterval);
        }

        yield return new WaitForSeconds(5f);
    }

    // Method to spawn a single enemy
    private void SpawnEnemy(string tagToSpawn)
    {
        if (enemyTags.Length == 0) return;

        Vector3 spawnPos = spawnPoint.position;
        spawnPos.y = 1.5f;

        GameObject enemy = ObjectPoolManager.Instance.SpawnFromPool(tagToSpawn, spawnPos, Quaternion.identity);

        if (enemy == null)
        {
            Debug.LogWarning($"[EnemyAI] Failed to spawn enemy with tag: {tagToSpawn}");
            return;
        }

        enemy.tag = "EnemyUnit";

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

    public void StopSpawning()
    {
        if (waveRoutine != null)
        {
            StopCoroutine(waveRoutine);
            waveRoutine = null;
            Debug.Log("[EnemyAI] Spawning stopped.");
        }
    }

    public void ClearAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("EnemyUnit");
        foreach (var enemy in enemies)
        {
            if (enemy.TryGetComponent<PooledObject>(out var pooled))
                pooled.ReturnToPool();
            else
                Destroy(enemy);
        }
        Debug.Log("[EnemyAI] All enemies cleared.");
    }
}