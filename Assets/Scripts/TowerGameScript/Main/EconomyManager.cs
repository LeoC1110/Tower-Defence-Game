using UnityEngine;
using System.Collections;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance;
    public int PlayerGold => playerGold;

    [Header("Base Income")]
    public int baseGoldPerSecond = 2; // The base amount of gold the player earns per second
    public float incomeInterval = 1f; // Interval for base gold generation

    [Header("Bank Income")]
    public int goldPerBank = 3;       // Additional gold earned per Bank unit
    public float bankGoldInterval = 1f; // Interval for checking Bank-based income

    [Header("Player Gold")]
    public int playerGold;            // Current gold of the player

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(GenerateBaseIncome());
        StartCoroutine(GenerateBankIncome());
    }

    private IEnumerator GenerateBaseIncome()
    {
        while (GameManager.Instance != null && GameManager.Instance.gameTime > 0)
        {
            yield return new WaitForSeconds(incomeInterval);
            AddGold(baseGoldPerSecond);
        }
    }

    private IEnumerator GenerateBankIncome()
    {
        while (GameManager.Instance != null && GameManager.Instance.gameTime > 0)
        {
            yield return new WaitForSeconds(bankGoldInterval);

            int bankCount = GameObject.FindGameObjectsWithTag("Bank").Length;
            if (bankCount > 0)
            {
                int income = bankCount * goldPerBank;
                AddGold(income);
                Debug.Log($"Bank Income: +{income} from {bankCount} banks.");
            }
        }
    }

    public void AddGold(int amount)
    {
        playerGold += amount;
        UIManager.Instance?.UpdateGoldUI();
    }

    public bool SpendGold(int amount)
    {
        if (playerGold >= amount)
        {
            playerGold -= amount;
            UIManager.Instance?.UpdateGoldUI();
            return true;
        }
        return false;
    }
}

