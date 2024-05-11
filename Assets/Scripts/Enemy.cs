using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    private enum State { Idle, Patrolling, Chasing, Attacking, Hurting, Dead };
    public PlayerHealth playerHealth;
    public List<Transform> waypoints;

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private State state;
    private int nextWaypoint;
    private float health;
    private float stateTimer;

    public float Health { get => health; set{ health = Mathf.Max(value, 0f); }}

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        health = enemyData.maxHealth;
        nextWaypoint = 0;

        stateTimer = 0;

        StartIdling();
    }

    private void StartIdling()
    {
        state = State.Idle;

        navMeshAgent.isStopped = true;
        animator.SetFloat("Velocity", 0f);
        navMeshAgent.speed = 0f;

        stateTimer = Random.Range(0f, enemyData.maxIdleTime);
    }

    private void StartPatrolling()
    {
        state = State.Patrolling;

        Vector3 destination = waypoints[nextWaypoint].position + new Vector3(Random.Range(-3, 3), 0, Random.Range(-3, 3));

        navMeshAgent.SetDestination(destination);

        navMeshAgent.speed = enemyData.walkSpeed;
        navMeshAgent.isStopped = false;

        animator.SetFloat("Velocity", navMeshAgent.speed);
    }

    private void StartChasing()
    {
        state = State.Chasing;
        
        navMeshAgent.SetDestination(playerHealth.transform.position);

        navMeshAgent.speed = enemyData.runSpeed;
        navMeshAgent.isStopped = false;

        animator.SetFloat("Velocity", navMeshAgent.speed);
    }

    private void StartAttacking()
    {
        state = State.Attacking;

        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
        animator.SetFloat("Velocity", 0);

        Attack();
    }

    private void Attack()
    {
        animator.SetTrigger("Attack");

        stateTimer = enemyData.attackCooldown;
    }

    private void DamagePlayer()
    {
        if (Vector3.Distance(playerHealth.transform.position, transform.position) <= enemyData.attackRange)
            playerHealth.Damage(enemyData.attackDamage);
    }

    private void StartHurting()
    {
        state = State.Hurting;
        stateTimer = enemyData.hitStunCooldown;

        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
        animator.SetFloat("Velocity", 0);

        animator.SetTrigger("Hurt");
    }

    private void Die()
    {
        state = State.Dead;

        navMeshAgent.isStopped = true;
        animator.SetFloat("Velocity", 0);

        animator.SetTrigger("Die");
    }

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

    private bool IsPlayerOnSight()
    {
        float distance = Vector3.Distance(playerHealth.transform.position, transform.position);

        if (distance > enemyData.detectionRange)
            return false;

        if (distance > enemyData.sensingRange && Vector3.Angle(playerHealth.transform.position - transform.position, transform.forward) > enemyData.detectionAngle / 2)
            return false;

        if (Physics.Linecast(transform.position, playerHealth.transform.position, out RaycastHit hitInfo) &&
            hitInfo.collider.transform != playerHealth.transform)
            return false;

        return true;
    }

    private void UpdatePatrol()
    {
        if (IsPlayerOnSight())
            StartChasing();

        else if (navMeshAgent.remainingDistance <= 1.5f)
            StartIdling();
    }

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
            StartIdling();
    }

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

    private void UpdateHurt()
    {
        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0f)
            StartChasing();
    }

    public void Damage(float amount)
    {
        if (state != State.Hurting)
        {
            health = Mathf.Max(health - amount, 0);

            if (health > 0)
                StartHurting();
            else
                Die();
        }
    }
}
