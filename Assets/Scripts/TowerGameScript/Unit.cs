using UnityEngine;
using System.Collections;

public enum UnitType { Chariot, RangedChariot, MoneyGenerator }

public class Unit : MonoBehaviour
{
    public UnitType unitType;
    public int health;
    public int damage;
    public float attackSpeed;
    public float attackRange;
    public float movementSpeed;
    public int cost;
    public int bounty;
    public Transform target;
    private Renderer unitRenderer;
    private Color originalColor;

    void Start()
    {
        InitializeUnit();
        unitRenderer = GetComponent<Renderer>();
        if (unitRenderer != null)
        {
            originalColor = unitRenderer.material.color;
        }
        StartCoroutine(MoveToTarget());
    }

    void InitializeUnit()
    {
        switch (unitType)
        {
            case UnitType.Chariot:
                health = PlayerPrefs.GetInt("ChariotHealth", 150);
                damage = PlayerPrefs.GetInt("ChariotDamage", 25);
                attackSpeed = PlayerPrefs.GetFloat("ChariotAttackSpeed", 1.5f);
                attackRange = PlayerPrefs.GetFloat("ChariotAttackRange", 1f);
                movementSpeed = PlayerPrefs.GetFloat("ChariotMovementSpeed", 2f);
                cost = PlayerPrefs.GetInt("ChariotCost", 40);
                bounty = PlayerPrefs.GetInt("ChariotBounty", 25);
                break;
            case UnitType.RangedChariot:
                health = PlayerPrefs.GetInt("RangedChariotHealth", 80);
                damage = PlayerPrefs.GetInt("RangedChariotDamage", 18);
                attackSpeed = PlayerPrefs.GetFloat("RangedChariotAttackSpeed", 1.5f);
                attackRange = PlayerPrefs.GetFloat("RangedChariotAttackRange", 3f);
                movementSpeed = PlayerPrefs.GetFloat("RangedChariotMovementSpeed", 1f);
                cost = PlayerPrefs.GetInt("RangedChariotCost", 45);
                bounty = PlayerPrefs.GetInt("RangedChariotBounty", 30);
                break;
        }
    }

    IEnumerator MoveToTarget()
    {
        while (target != null && health > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, movementSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target.position) < 0.5f)
            {
                GameManager.Instance.TakeDamage(target.CompareTag("PlayerBase"), damage);
                Destroy(gameObject);
                yield break;
            }
            yield return null;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        if (unitRenderer != null)
        {
            StartCoroutine(FlashRed());
        }
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator FlashRed()
    {
        unitRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        unitRenderer.material.color = originalColor;
    }
}
