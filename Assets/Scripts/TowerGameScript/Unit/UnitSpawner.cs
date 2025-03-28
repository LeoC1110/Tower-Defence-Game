using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform enemyTarget;
    [SerializeField] private UnitFactoryProvider factoryProvider;
    [SerializeField] private List<Transform> bankSpawnPoints;
    [SerializeField] private Button bankButton;

    private int bankCount = 0;
    private const int BankLimit = 4;

    private void Start()
    {
        if (!spawnPoint || !enemyTarget || !factoryProvider || !bankButton || bankSpawnPoints.Count < BankLimit)
            Debug.LogWarning("UnitSpawner error");
    }

    // Spawn a unit based on the type
    public void SpawnUnit(UnitType type)
    {
        if (!factoryProvider || !enemyTarget) return;

        int cost = factoryProvider.GetCost(type);
        if (EconomyManager.Instance?.SpendGold(cost) != true)
        {
            UIManager.Instance?.ShowMessage($": Not enough gold to buy {type}", 1f);
            return;
        }

        if (type == UnitType.Bank) HandleBankSpawn();
        else SpawnRegularUnit(type);
    }

    // Spawn a regular unit
    private void SpawnRegularUnit(UnitType type)
    {
        Vector3 pos = spawnPoint.position;
        pos.y = 1.5f;

        GameObject unit = ObjectPoolManager.Instance.SpawnFromPool(type.ToString(), pos, Quaternion.identity);
      
        if (unit)
        {
            FaceTarget(unit, enemyTarget.position);
            if (unit.TryGetComponent<BaseUnit>(out var baseUnit))
                baseUnit.SetTarget(enemyTarget);
        }
    }

    private void HandleBankSpawn()
    {
        if (bankCount >= BankLimit || bankCount >= bankSpawnPoints.Count)
        {
            UIManager.Instance?.ShowMessage(": The Bank limit reached!", 1f);
            return;
        }

        Vector3 spawnPos = bankSpawnPoints[bankCount].position;
        GameObject bank = ObjectPoolManager.Instance.SpawnFromPool(UnitType.Bank.ToString(), spawnPos, Quaternion.identity);
        if (bank && bank.TryGetComponent<BaseUnit>(out var baseUnit))
        {
            baseUnit.SetTarget(enemyTarget);
        }

        bankCount++;
        if (bankCount >= BankLimit) bankButton.interactable = false;
    }

    private void FaceTarget(GameObject unit, Vector3 targetPos)
    {
        if (!unit) return;
        Vector3 direction = (targetPos - unit.transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero) unit.transform.rotation = Quaternion.LookRotation(direction);
    }

    public void SpawnChariot() => SpawnUnit(UnitType.Chariot);
    public void SpawnCatapult() => SpawnUnit(UnitType.Catapult);
    public void SpawnBankButton() => SpawnUnit(UnitType.Bank);
}