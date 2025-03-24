using UnityEngine;
using UnityEngine.UI;

public class UnitSpawner : MonoBehaviour
{
    public GameObject chariotPrefab;
    public GameObject rangedChariotPrefab;
    public GameObject moneyGeneratorPrefab;
    public Transform enemyBase;

    public Transform unitSpawnPoint;  // Manually set spawn location for combat units
    public Transform[] moneyGeneratorPoints; // Manually set 4 spawn locations for MoneyGenerators

    public Button buttonChariot;
    public Button buttonRangedChariot;
    public Button buttonMoneyGenerator;

    private int moneyGeneratorCount = 0; // Track how many MoneyGenerators have been placed

    void Start()
    {
        BindButtons();
    }

    void BindButtons()
    {
        if (buttonChariot != null)
            buttonChariot.onClick.AddListener(() => TrySpawnUnit(chariotPrefab));
        if (buttonRangedChariot != null)
            buttonRangedChariot.onClick.AddListener(() => TrySpawnUnit(rangedChariotPrefab));
        if (buttonMoneyGenerator != null)
            buttonMoneyGenerator.onClick.AddListener(() => TrySpawnMoneyGenerator());
    }

    void TrySpawnMoneyGenerator()
    {
        if (moneyGeneratorCount >= moneyGeneratorPoints.Length)
        {
            Debug.Log("Money Generator limit reached!");
            return;
        }

        if (EconomyManager.Instance == null)
        {
            Debug.LogError("EconomyManager instance not found!");
            return;
        }

        if (EconomyManager.Instance.SpendGold(50)) // Assuming Money Generator costs 50 gold
        {
            Instantiate(moneyGeneratorPrefab, moneyGeneratorPoints[moneyGeneratorCount].position, Quaternion.identity);
            moneyGeneratorCount++;

            if (moneyGeneratorCount >= moneyGeneratorPoints.Length)
            {
                buttonMoneyGenerator.interactable = false; // Disable button when max reached
                Debug.Log("Money Generator purchase limit reached. Button disabled.");
            }
        }
        else
        {
            Debug.Log("Not enough gold to buy Money Generator!");
        }
    }

    void TrySpawnUnit(GameObject unitPrefab)
    {
        if (unitPrefab == null)
        {
            Debug.LogError("Unit prefab is null. Check button assignments.");
            return;
        }

        if (unitSpawnPoint == null)
        {
            Debug.LogError("Unit Spawn Point is null. Assign a valid spawn location in the Inspector.");
            return;
        }

        if (enemyBase == null)
        {
            Debug.LogError("Enemy base is null. Assign enemyBase in the Inspector.");
            return;
        }

        SpawnUnit(unitPrefab);
    }

    public void SpawnUnit(GameObject unitPrefab)
    {
        if (EconomyManager.Instance == null)
        {
            Debug.LogError("EconomyManager instance not found! Cannot spawn units.");
            return;
        }

        Unit unitComponent = unitPrefab.GetComponent<Unit>();
        if (unitComponent == null)
        {
            Debug.LogError($"Unit script missing on {unitPrefab.name}. Ensure it has a Unit component.");
            return;
        }

        if (EconomyManager.Instance.SpendGold(unitComponent.cost))
        {
            GameObject unit = Instantiate(unitPrefab, unitSpawnPoint.position, Quaternion.identity);
            unit.transform.localScale = new Vector3(1f, 1f, 1f);
            unit.GetComponent<Unit>().target = enemyBase;
        }
        else
        {
            Debug.Log("Not enough gold to spawn unit!");
        }
    }
}
