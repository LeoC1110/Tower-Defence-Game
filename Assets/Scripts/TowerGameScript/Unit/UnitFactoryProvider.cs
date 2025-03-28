using UnityEngine;
using System.Collections.Generic;

public class UnitFactoryProvider : MonoBehaviour
{
    [System.Serializable]
    public class UnitFactory
    {
        public UnitType unitType;
        public GameObject prefab;
        public int defaultCost = 50;
        public string costKey;
        [SerializeField] private float spawnHeight = 1.5f;

        public GameObject Create(Vector3 position, Transform target)
        {
            if (!prefab) return null;
            Vector3 spawnPos = position;
            spawnPos.y = spawnHeight;
            GameObject unit = Object.Instantiate(prefab, spawnPos, Quaternion.identity);

            if (target && unitType != UnitType.PlayerBase && unitType != UnitType.EnemyBase)
            {
                Vector3 direction = (target.position - position).normalized;
                direction.y = 0;
                if (direction != Vector3.zero) unit.transform.rotation = Quaternion.LookRotation(direction);
                if (unit.TryGetComponent<BaseUnit>(out var baseUnit)) baseUnit.SetTarget(target);
            }
            return unit;
        }

        public int GetCost() => PlayerPrefs.GetInt(costKey, defaultCost);
    }

    [SerializeField] private List<UnitFactory> factories;
    private Dictionary<UnitType, UnitFactory> factoryMap;

    private void Awake()
    {
        factoryMap = new Dictionary<UnitType, UnitFactory>();
        if (factories == null || factories.Count == 0) return;

        foreach (var factory in factories)
        {
            if (factory != null && !factoryMap.ContainsKey(factory.unitType))
                factoryMap[factory.unitType] = factory;
        }
    }

    public GameObject CreateUnit(UnitType type, Vector3 position, Transform target) =>
        factoryMap.TryGetValue(type, out var factory) ? factory.Create(position, target) : null;

    public int GetCost(UnitType type) =>
        factoryMap.TryGetValue(type, out var factory) ? factory.GetCost() : 0;
}