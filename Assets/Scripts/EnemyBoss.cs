using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBoss : MonoBehaviour
{
    public float maxHealth;
    public float maxIdleTime;
    public float runSpeed;

    private enum State { Idle, Chasing, Attacking, Stunned, Dead };
    
    private NavMeshAgent navMeshAgent;
    
    private PlayerHealth playerHealth;
    private UIManager  uiManager;
    private Animator animator;
    private BossDamageHitbox[] hitboxes;
    private State state;
    private float stateTimer;
    private float stunTimer;
    private float health;
    private int prevAttack;

    public float Health { get => health; set{ health = Mathf.Max(value, 0f); }}
    
    public int CurrentAttack { get; set; }
    public float CurrentAttackRange { get; set; }
    public float CurrentAttackDamage { get; set; }
    private Dictionary<int, (string, float, float, float)> attacks = new Dictionary<int, (string, float, float, float)>
    {
        // {index, (name, range, damage, cooldown)}

        {0, ("DoubleAxeSlash", 4f, 20f, 5f)},
        {1, ("BullCharge", 5f, 45f, 10f)},
        {2, ("QuakingStomp", 6f, 15f, 12f)},
    };
    private List<float> attackCooldowns;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        uiManager = GameObject.Find("UI").GetComponent<UIManager>();
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();
        health = maxHealth;
        stunTimer = 0f;
        stateTimer = 0f;
        prevAttack = 1;
        attackCooldowns = new List<float>();
        hitboxes = GetComponentsInChildren<BossDamageHitbox>();

        foreach (KeyValuePair<int, (string, float, float, float)> kv in attacks)
        {
            attackCooldowns.Add(0f);
        }

        UpdateUI();
        uiManager.ToggleBossHPBar();

        SelectAttack();
        StartChasing();
    }

    private void UpdateUI()
    {
        uiManager.SetBossHealthFill(health / maxHealth);
        uiManager.SetBossHealthColor(state == State.Stunned);
    }

    public void ResetHits()
    {
        foreach(BossDamageHitbox bh in hitboxes)
        {
            bh.hitPlayer = false;
        }
    }

    private void StartIdling()
    {
        state = State.Idle;

        navMeshAgent.isStopped = true;
        animator.SetFloat("Velocity", 0f);
        navMeshAgent.speed = 0f;

        SelectAttack();
    }

    private void StartChasing()
    {
        state = State.Chasing;
        
        navMeshAgent.SetDestination(playerHealth.transform.position);

        navMeshAgent.speed = runSpeed;
        navMeshAgent.isStopped = false;

        animator.SetFloat("Velocity", navMeshAgent.speed);
    }

    private void StartAttacking()
    {
        state = State.Attacking;

        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
        animator.SetFloat("Velocity", 0);
    }

    private void Attack()
    {
        animator.SetTrigger(attacks[CurrentAttack].Item1);
        attackCooldowns[CurrentAttack] = attacks[CurrentAttack].Item4;

        stateTimer = Random.Range(0f, 2f);

        SelectAttack();
    }

    private void SelectAttack()
    {
        float lowestCD = 100f;
        List<int> lowestCDsIndex = new List<int>();

        for(int i = 0; i < attackCooldowns.Count; i++)
        {
            if (attackCooldowns[i] == lowestCD && i != prevAttack)
            {
                lowestCDsIndex.Add(i);
            }

            if(attackCooldowns[i] < lowestCD && i != prevAttack)
            {
                lowestCD = attackCooldowns[i];
                lowestCDsIndex = new List<int>() {i};
            }
        }

        CurrentAttack = lowestCDsIndex[Random.Range(0, lowestCDsIndex.Count)];
        CurrentAttackRange = attacks[CurrentAttack].Item2;
        CurrentAttackDamage = attacks[CurrentAttack].Item3;
        prevAttack = CurrentAttack;
    }

    private void StartStunning()
    {
        state = State.Stunned;

        stunTimer = 5f;

        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
        animator.SetTrigger("Stun");
    }

    private void Die()
    {
        state = State.Dead;

        navMeshAgent.isStopped = true;
        animator.SetFloat("Velocity", 0);
        animator.SetTrigger("Die");

        uiManager.ToggleBossHPBar();
    }

    private void Update()
    {
        UpdateUI();

        switch (state)
        {
            case State.Idle:
                UpdateIdle();
                break;

            case State.Chasing:
                UpdateChase();
                break;

            case State.Attacking:
                UpdateAttack();
                break;

            case State.Stunned:
                UpdateStun();
                break;
        }
    }

    private void FixedUpdate()
    {
        stateTimer -= Time.deltaTime;

        for(int i = 0; i < attackCooldowns.Count; i++)
        {
            attackCooldowns[i] = Mathf.Max(attackCooldowns[i] - Time.deltaTime, 0f);
        }
    }

    private void UpdateIdle()
    {
        if (Vector3.Distance(playerHealth.transform.position, transform.position) > CurrentAttackRange)
            StartChasing();

        else if (attackCooldowns[CurrentAttack] <= 0 && Vector3.Distance(playerHealth.transform.position, transform.position) <= CurrentAttackRange)
            StartAttacking();

        else
            StartIdling();
    }

    private void UpdateStun()
    {
        if (stunTimer > 0)
        {
            stunTimer -= Time.deltaTime;
        }

        else
        {
            state = State.Idle;

            navMeshAgent.isStopped = false;
            navMeshAgent.speed = runSpeed;
            animator.SetTrigger("Unstun");
        }

        UpdateUI();
    }

    private void UpdateChase()
    {
        if (Vector3.Distance(playerHealth.transform.position, transform.position) <= CurrentAttackRange)
            StartAttacking();

        else
        {
            navMeshAgent.SetDestination(playerHealth.transform.position);

            if (navMeshAgent.remainingDistance > CurrentAttackRange)
                StartChasing();
        }
    }

    private void UpdateAttack()
    {
        if (stateTimer <= 0f && attackCooldowns[CurrentAttack] <= 0)
            Attack();
        else
            StartIdling();
    }

    public void Damage(float amount)
    {
        if (state != State.Stunned)
            return;

        Health -= amount;

        if (health <= 0)
            Die();

        UpdateUI();
    }
}
