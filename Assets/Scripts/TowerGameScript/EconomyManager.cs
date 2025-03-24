using System.Collections;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance;
    public int playerGold = 120; // Current gold amount
    public int baseGoldPerSecond = 2; // Gold earned per second
    public int moneyGeneratorGoldPerCycle = 3; // Additional gold per MoneyGenerator every cycle
    public float goldGenerationInterval = 1f; // Interval for base gold generation
    public float moneyGeneratorInterval = 2f; // Interval for MoneyGenerator bonus gold

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        StartCoroutine(BaseGoldGeneration());
        StartCoroutine(MoneyGeneratorGoldGeneration());
    }

    // Generates base gold every second
    IEnumerator BaseGoldGeneration()
    {
        while (GameManager.Instance.gameTime > 0)
        {
            yield return new WaitForSeconds(goldGenerationInterval);
            playerGold += baseGoldPerSecond;
            UIManager.Instance.UpdateGoldUI();
        }
    }

    // Generates additional gold based on the number of MoneyGenerators every 2 seconds
    IEnumerator MoneyGeneratorGoldGeneration()
    {
        while (GameManager.Instance.gameTime > 0)
        {
            yield return new WaitForSeconds(moneyGeneratorInterval);
            int generatorCount = GameObject.FindGameObjectsWithTag("MoneyGenerator").Length;
            playerGold += generatorCount * moneyGeneratorGoldPerCycle;
            UIManager.Instance.UpdateGoldUI();
        }
    }

    // Deducts gold when spending; returns true if successful, false if insufficient funds
    public bool SpendGold(int amount)
    {
        if (playerGold >= amount)
        {
            playerGold -= amount;
            UIManager.Instance.UpdateGoldUI();
            return true;
        }
        return false;
    }
}
