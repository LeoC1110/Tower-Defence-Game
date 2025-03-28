using UnityEngine;
using System.Collections;

public class BaseUnit : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private UnitType unitType;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private int damage = 10;
    [SerializeField] private int health = 100;

    // === Public Read-Only Properties === //
    public float MoveSpeed => moveSpeed;
    public float AttackRange => attackRange;
    public float AttackCooldown => attackCooldown;
    public int Damage => damage;
    public int Health => health;
    public UnitType Type => unitType;

    // === Private Runtime State === //
    private Transform target;
    private float lastAttackTime;
    private bool IsBase => unitType == UnitType.PlayerBase || unitType == UnitType.EnemyBase;

    // State Machine
    private UnitState currentState;
    private readonly IdleState idleState = new IdleState();
    private readonly MovingToBaseState movingToBaseState = new MovingToBaseState();
    private readonly AttackingState attackingState = new AttackingState();
    public readonly BaseDefendingState baseDefendingState = new BaseDefendingState(); 

    // Prioritization after kill
    private bool prioritizeBaseAfterKill = false;
    private float basePriorityDelay = 0.5f; // Delay before allowing new enemy targeting
    private float lastKillTime;

    private void Start()
    {
        if (IsBase) moveSpeed = 0f;
        AdjustColliderSize();

        // Initialize state machine
        if (IsBase)
        {
            TransitionToState(baseDefendingState); // Base use BaseDefendingState
        }
        else
        {
            SetTargetToEnemyBase();
            TransitionToState(movingToBaseState);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            return;

        currentState?.Update(this);
    }

    private void OnTriggerStay(Collider other)
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            return;

        currentState?.OnTriggerStay(this, other);
    }

    // Transition to a new state
    public void TransitionToState(UnitState newState)
    {
        currentState?.Exit(this);
        currentState = newState;
        currentState.Enter(this);
    }

    // Move toward the current target
    public void MoveToTarget()
    {
        if (target)
        {
            Debug.Log($"{gameObject.name} moving toward {target.name}");
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        }
    }

    // Start attacking a target
    public void StartAttacking(Transform newTarget)
    {
        target = newTarget;
        prioritizeBaseAfterKill = false; // Allow attacking new enemies
        TransitionToState(attackingState);
    }

    // Take damage and destroy unit if health is 0
    public void TakeDamage(int dmg)
    {
        health -= dmg;

        // UI update
        UIManager.Instance?.UpdateHPUI();

        if (health <= 0)
        {
            if (TryGetComponent<PooledObject>(out var pool))
                pool.ReturnToPool();
            else
                Destroy(gameObject);
        }
    }

    public void SetTarget(Transform newTarget) => target = newTarget;

    // Set target to enemy base
    public void SetTargetToEnemyBase()
    {
        GameObject baseObj;
        if (CompareTag("EnemyUnit") || CompareTag("EnemyBase"))
        {
            baseObj = GameManager.Instance?.PlayerBase;
        }
        else if (CompareTag("PlayerUnit") || CompareTag("PlayerBase"))
        {
            baseObj = GameManager.Instance?.EnemyBase;
        }
        else
        {
            Debug.LogError($"{gameObject.name} has an invalid tag: {gameObject.tag}. Cannot determine target base.");
            return;
        }

        if (baseObj && baseObj != gameObject && baseObj.activeInHierarchy)
        {
            SetTarget(baseObj.transform);
            Debug.Log($"{gameObject.name} targeting {baseObj.name}");
        }
        else
        {
            Debug.LogError($"No valid {(CompareTag("EnemyUnit") || CompareTag("EnemyBase") ? "PlayerBase" : "EnemyBase")} found in scene, self-targeting detected, or target inactive! Unit {gameObject.name} has no target.");
            TransitionToState(idleState);
        }
    }

    private void AdjustColliderSize()
    {
        if (TryGetComponent<BoxCollider>(out var box) && box.isTrigger)
        {
            box.size = new Vector3(
                Mathf.Max(box.size.x, 1f),
                Mathf.Max(box.size.y, 1f),
                Mathf.Max(box.size.z, attackRange * 2f)
            );
        }
    }

    public bool IsEnemy(BaseUnit unit)
    {
        return (CompareTag("PlayerUnit") || CompareTag("PlayerBase")) && (unit.CompareTag("EnemyUnit") || unit.CompareTag("EnemyBase")) ||
               (CompareTag("EnemyUnit") || CompareTag("EnemyBase")) && (unit.CompareTag("PlayerUnit") || unit.CompareTag("PlayerBase"));
    }

    // State Machine Accessors
    public Transform Target => target;
    public float LastAttackTime { get => lastAttackTime; set => lastAttackTime = value; }
    public bool PrioritizeBaseAfterKill => prioritizeBaseAfterKill;
    public float BasePriorityDelay => basePriorityDelay;
    public float LastKillTime { get => lastKillTime; set => lastKillTime = value; }

    public void SetPrioritizeBaseAfterKill(bool value) => prioritizeBaseAfterKill = value;
    public void TransitionToMovingToBase() => TransitionToState(movingToBaseState);
}

// State Machine States
public abstract class UnitState
{
    public virtual void Enter(BaseUnit unit) { }
    public virtual void Update(BaseUnit unit) { }
    public virtual void OnTriggerStay(BaseUnit unit, Collider other) { }
    public virtual void Exit(BaseUnit unit) { }
}

public class IdleState : UnitState
{
    public override void Enter(BaseUnit unit)
    {
        Debug.Log($"{unit.gameObject.name} entered Idle state");
    }
}

public class MovingToBaseState : UnitState
{
    public override void Enter(BaseUnit unit)
    {
        Debug.Log($"{unit.gameObject.name} entered MovingToBase state");
        if (!unit.Target || !unit.Target.gameObject.activeInHierarchy)
        {
            unit.SetTargetToEnemyBase();
        }
    }

    public override void Update(BaseUnit unit)
    {
        if (!unit.Target || !unit.Target.gameObject.activeInHierarchy)
        {
            unit.SetTargetToEnemyBase();
        }
        unit.MoveToTarget();
    }

    public override void OnTriggerStay(BaseUnit unit, Collider other)
    {
        if (unit.PrioritizeBaseAfterKill && Time.time - unit.LastKillTime < unit.BasePriorityDelay)
        {
            Debug.Log($"{unit.gameObject.name} prioritizing base, ignoring new enemy for {unit.BasePriorityDelay - (Time.time - unit.LastKillTime)} seconds");
            return;
        }

        if (other.TryGetComponent(out BaseUnit enemy) && unit.IsEnemy(enemy))
        {
            Debug.Log($"{unit.gameObject.name} detected enemy {enemy.gameObject.name}, setting as target");
            unit.StartAttacking(enemy.transform);
        }
    }
}

public class AttackingState : UnitState
{
    public override void Enter(BaseUnit unit)
    {
        Debug.Log($"{unit.gameObject.name} entered Attacking state");
        unit.StartCoroutine(AttackRoutine(unit));
    }

    private IEnumerator AttackRoutine(BaseUnit unit)
    {
        Debug.Log($"{unit.gameObject.name} starting attack on {unit.Target.name}");

        // if it's a base, don't check attackRange (since base is stationary)
        bool isBase = unit.Type == UnitType.PlayerBase || unit.Type == UnitType.EnemyBase;

        while (unit.Target && (isBase || Vector3.Distance(unit.transform.position, unit.Target.position) <= unit.AttackRange))
        {
            if (Time.time - unit.LastAttackTime >= unit.AttackCooldown)
            {
                if (unit.Target.TryGetComponent(out BaseUnit enemy) && unit.IsEnemy(enemy) && unit.Target.gameObject.activeInHierarchy)
                {
                    enemy.TakeDamage(unit.Damage);
                    unit.LastAttackTime = Time.time;

                    if (!unit.Target || !unit.Target.gameObject.activeInHierarchy)
                    {
                        unit.LastKillTime = Time.time;
                        unit.SetPrioritizeBaseAfterKill(true);
                        Debug.Log($"{unit.gameObject.name} target destroyed, prioritizing base for {unit.BasePriorityDelay} seconds");
                        break;
                    }
                }
                else
                {
                    unit.LastKillTime = Time.time;
                    unit.SetPrioritizeBaseAfterKill(true);
                    Debug.Log($"{unit.gameObject.name} target destroyed or invalid, prioritizing base for {unit.BasePriorityDelay} seconds");
                    break;
                }
            }
            yield return null;
        }

        Debug.Log($"{unit.gameObject.name} finished attacking, transitioning to " + (isBase ? "BaseDefending" : "MovingToBase"));
        if (isBase)
            unit.TransitionToState(unit.baseDefendingState); 
        else
            unit.TransitionToMovingToBase();
    }
}

public class BaseDefendingState : UnitState
{
    public override void Enter(BaseUnit unit)
    {
        Debug.Log($"{unit.gameObject.name} entered BaseDefending state");
    }

    public override void OnTriggerStay(BaseUnit unit, Collider other)
    {
        Debug.Log($"{unit.gameObject.name} OnTriggerStay called with {other.gameObject.name}");
        if (other.TryGetComponent(out BaseUnit enemy) && unit.IsEnemy(enemy))
        {
            Debug.Log($"{unit.gameObject.name} detected enemy {enemy.gameObject.name}, setting as target");
            unit.StartAttacking(enemy.transform);
        }
        else
        {
            Debug.Log($"{unit.gameObject.name} did not detect enemy in {other.gameObject.name}");
        }
    }
}