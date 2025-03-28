using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private Text tooltipUI;
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private UnitType unitType;
    [SerializeField] private UnitFactoryProvider factoryProvider;

    private BaseUnit unitData;

    private void Awake()
    {
        // Validate setup
        if (!tooltipUI || !unitPrefab || !factoryProvider)
        {
            Debug.LogWarning($"{nameof(UIButtonTooltip)} is missing references!");
            enabled = false;
            return;
        }

        if (!unitPrefab.TryGetComponent(out unitData))
        {
            Debug.LogWarning($"Unit prefab '{unitPrefab.name}' is missing BaseUnit!");
            enabled = false;
            return;
        }

        HideTooltip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!enabled || unitData == null) return;

        int cost = factoryProvider.GetCost(unitType);
        tooltipUI.text = FormatStats(unitData, cost);
        tooltipUI.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip();
    }

    private void HideTooltip()
    {
        if (tooltipUI != null)
        {
            tooltipUI.text = string.Empty;
            tooltipUI.gameObject.SetActive(false);
        }
    }

    private string FormatStats(BaseUnit unit, int cost)
    {
        return $"MS: {unit.MoveSpeed}\n" +
               $"AR: {unit.AttackRange}\n" +
               $"AC: {unit.AttackCooldown}\n" +
               $"DMG: {unit.Damage}\n" +
               $"HP: {unit.Health}\n" +
               $"COST: {cost}";
    }
}
