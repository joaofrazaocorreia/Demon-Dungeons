using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Class that defines a basic enemy and their properties.
/// </summary>
public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;

    private enum State { Idle, Patrolling, Chasing, Attacking, Hurting, Dead };
    public PlayerHealth playerHealth;
    public List<Transform> waypoints;

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private EnemyAlertRange alertRange;
    private State state;
    private int nextWaypoint;
    private float health;
    private float stateTimer;
    private bool sawPlayer;

    public float Health { get => health; set{ health = Mathf.Max(value, 0f); }}

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        alertRange = GetComponentInChildren<EnemyAlertRange>();
        health = enemyData.maxHealth;
        nextWaypoint = 0;
        sawPlayer = false;

        stateTimer = 0;
        alertRange.GetComponent<SphereCollider>().radius = enemyData.alertRange;

        StartIdling();
    }

    /// <summary>
    /// Idle behaviour, for when the enemy is at a waypoint not seeing the player.
    /// </summary>
    private void StartIdling()
    {
        state = State.Idle;

        navMeshAgent.isStopped = true;
        animator.SetFloat("Velocity", 0f);
        navMeshAgent.speed = 0f;

        stateTimer = Random.Range(0f, enemyData.maxIdleTime);
    }

    /// <summary>
    /// Patrol behaviour, the enemy moves between waypoints.
    /// </summary>
    private void StartPatrolling()
    {
        state = State.Patrolling;

        Vector3 destination = waypoints[nextWaypoint].position + new Vector3(Random.Range(-3, 3), 0, Random.Range(-3, 3));

        navMeshAgent.SetDestination(destination);

        navMeshAgent.speed = enemyData.walkSpeed;
        navMeshAgent.isStopped = false;

        animator.SetFloat("Velocity", navMeshAgent.speed);
    }

    /// <summary>
    /// Chase behaviour, the enemy chases the player's position.
    /// </summary>
    private void StartChasing()
    {
        state = State.Chasing;
        
        navMeshAgent.SetDestination(playerHealth.transform.position);

        navMeshAgent.speed = enemyData.runSpeed;
        navMeshAgent.isStopped = false;

        animator.SetFloat("Velocity", navMeshAgent.speed);
    }

    /// <summary>
    /// Attack behaviour, the enemy tries to attack and damge the player if its
    /// close enough.
    /// </summary>
    private void StartAttacking()
    {
        state = State.Attacking;

        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
        animator.SetFloat("Velocity", 0);

        Attack();
    }

    /// <summary>
    /// Attacks and sets its cooldown.
    /// </summary>
    private void Attack()
    {
        animator.SetTrigger("Attack");

        stateTimer = enemyData.attackCooldown;
    }

    /// <summary>
    /// Damages the player if hes close enough (called by the animator).
    /// </summary>
    private void DamagePlayer()
    {
        if (Vector3.Distance(playerHealth.transform.position, transform.position) <= enemyData.attackRange)
            playerHealth.Damage(enemyData.attackDamage);
    }

    /// <summary>
    /// Hurt behaviour, when the enemy takes damage.
    /// </summary>
    private void StartHurting()
    {
        state = State.Hurting;
        stateTimer = enemyData.hitStunCooldown;

        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
        animator.SetFloat("Velocity", 0);

        animator.SetTrigger("Hurt");
    }

    /// <summary>
    /// Death behaviour, when the enemy's health reaches 0 and it dies.
    /// </summary>
    private void Die()
    {
        state = State.Dead;

        navMeshAgent.isStopped = true;
        animator.SetFloat("Velocity", 0);

        animator.SetTrigger("Die");
    }

    /// <summary>
    /// Updates the enemy's current state.
    /// </summary>
    private void Update()
    {
        switch (state)
        {
            case State.Idle:
                UpdateIdle();
                break;

            case State.Patrolling:
                UpdatePatrol();
                break;

            case State.Chasing:
                UpdateChase();
                break;

            case State.Attacking:
                UpdateAttack();
                break;

            case State.Hurting:
                UpdateHurt();
                break;
        }
    }

    /// <summary>
    /// Updates the Idle behaviour.
    /// </summary>
    private void UpdateIdle()
    {
        if (IsPlayerOnSight())
            StartChasing();
        else
        {
            stateTimer -= Time.deltaTime;

            if (stateTimer <= 0f && waypoints.Count > 0)
            {
                nextWaypoint = (nextWaypoint + Random.Range(1, waypoints.Count)) % waypoints.Count;
                StartPatrolling();
            }
        }
    }

    /// <summary>
    /// Checks whether the enemy detects the player.
    /// </summary>
    /// <returns></returns>
    private bool IsPlayerOnSight()
    {
        if (sawPlayer)
            return true;


        float distance = Vector3.Distance(playerHealth.transform.position, transform.position);

        if (distance > enemyData.detectionRange)
            return false;

        if (distance > enemyData.sensingRange && Vector3.Angle(playerHealth.transform.position - transform.position, transform.forward) > enemyData.detectionAngle / 2)
            return false;

        if (Physics.Linecast(transform.position, playerHealth.transform.position, out RaycastHit hitInfo) &&
            hitInfo.collider.transform != playerHealth.transform)
            return false;

        BecomeAlerted();
        return true;
    }

    /// <summary>
    /// Updates the patrol behaviour.
    /// </summary>
    private void UpdatePatrol()
    {
        if (IsPlayerOnSight())
            StartChasing();

        else if (navMeshAgent.remainingDistance <= 1.5f)
            StartIdling();
    }

    /// <summary>
    /// Updates the chase behaviour.
    /// </summary>
    private void UpdateChase()
    {
        if (IsPlayerOnSight())
        {
            if (Vector3.Distance(playerHealth.transform.position, transform.position) <= enemyData.attackRange)
                StartAttacking();
            else
                navMeshAgent.SetDestination(playerHealth.transform.position);
        }
        else if (navMeshAgent.remainingDistance <= 1.5f)
            StartChasing();
    }

    /// <summary>
    /// Updates the attack behaviour.
    /// </summary>
    private void UpdateAttack()
    {
        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0f)
        {
            if (IsPlayerOnSight())
            {
                if (Vector3.Distance(playerHealth.transform.position, transform.position) <= enemyData.attackRange)
                    Attack();
                else
                    StartChasing();
            }
            else
                StartIdling();
        }
    }

    /// <summary>
    /// Updates the hurt behaviour.
    /// </summary>
    private void UpdateHurt()
    {
        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0f)
            StartChasing();
    }

    /// <summary>
    /// Deals a given amount of damage to the enemy and kills it if the health
    /// goes below 0.
    /// </summary>
    /// <param name="amount"></param>
    public void Damage(float amount)
    {
        health = Mathf.Max(health - amount, 0);

        if (health > 0)
            StartHurting();
        else
            Die();

    }

    /// <summary>
    /// Alerts itself and all nearby enemies to start chasing and attacking
    /// the player when it sees him.
    /// </summary>
    private void BecomeAlerted()
    {
        sawPlayer = true;

        foreach(Enemy e in alertRange.enemiesInRange)
        {
            if(!e.sawPlayer)
            {
                e.BecomeAlerted();
            }
        }
    }
}
