using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

/// <summary>
/// Class that defines the enemy boss and their behaviours.
/// </summary>
public class EnemyBoss : MonoBehaviour
{
    public float maxHealth;
    public float maxIdleTime;
    public float runSpeed;
    [SerializeField] private AudioClip slash1;
    [SerializeField] private AudioClip slash2;
    [SerializeField] private AudioClip[] footsteps;
    [SerializeField] private AudioClip death;
    [SerializeField] private AudioClip stomp;
    [SerializeField] private AudioClip charge;
    [SerializeField] private AudioClip unstun;
    [SerializeField] private AudioMixerGroup slash1Mixer;
    [SerializeField] private AudioMixerGroup slash2Mixer;
    [SerializeField] private AudioMixerGroup stepsMixer;
    [SerializeField] private AudioMixerGroup deathMixer;
    [SerializeField] private AudioMixerGroup stompMixer;
    [SerializeField] private AudioMixerGroup chargeMixer;
    [SerializeField] private AudioMixerGroup unstunMixer;

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
    private AudioSource slash1AudioSource;
    private AudioSource slash2AudioSource;
    private AudioSource deathAudioSource;
    private AudioSource stepsAudioSource;
    private AudioSource stompAudioSource;
    private AudioSource chargeAudioSource;
    private AudioSource unstunAudioSource;

    public float Health { get => health; set{ health = Mathf.Max(value, 0f); }}
    
    public int CurrentAttack { get; set; }
    public float CurrentAttackRange { get; set; }
    public float CurrentAttackDamage { get; set; }

    // Stores the boss's moveset
    private Dictionary<int, (string, float, float, float)> attacks = new Dictionary<int, (string, float, float, float)>
    {
        // {index, (name, range, damage, cooldown)}

        {0, ("DoubleAxeSlash", 4f, 8f, 5f)},
        {1, ("BullCharge", 5f, 20f, 10f)},
        {2, ("QuakingStomp", 6f, 10f, 12f)},
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

        slash1AudioSource = gameObject.AddComponent<AudioSource>();
        slash1AudioSource.outputAudioMixerGroup = slash1Mixer;
        slash1AudioSource.spatialBlend = 1.0f;

        slash2AudioSource = gameObject.AddComponent<AudioSource>();
        slash2AudioSource.outputAudioMixerGroup = slash2Mixer;
        slash2AudioSource.spatialBlend = 1.0f;
        
        stepsAudioSource = gameObject.AddComponent<AudioSource>();
        stepsAudioSource.outputAudioMixerGroup = stepsMixer;
        stepsAudioSource.spatialBlend = 1.0f;
        
        deathAudioSource = gameObject.AddComponent<AudioSource>();
        deathAudioSource.outputAudioMixerGroup = deathMixer;
        deathAudioSource.spatialBlend = 1.0f;
        
        stompAudioSource = gameObject.AddComponent<AudioSource>();
        stompAudioSource.outputAudioMixerGroup = stompMixer;
        stompAudioSource.spatialBlend = 1.0f;
        
        chargeAudioSource = gameObject.AddComponent<AudioSource>();
        chargeAudioSource.outputAudioMixerGroup = chargeMixer;
        chargeAudioSource.spatialBlend = 1.0f;
        
        unstunAudioSource = gameObject.AddComponent<AudioSource>();
        unstunAudioSource.outputAudioMixerGroup = unstunMixer;
        unstunAudioSource.spatialBlend = 1.0f;
        

        foreach (KeyValuePair<int, (string, float, float, float)> kv in attacks)
        {
            attackCooldowns.Add(0f);
        }

        // Enables the boss HP bar when the boss is spawned
        UpdateUI();
        uiManager.ToggleBossHPBar();

        SelectAttack();
        StartChasing();
    }

    /// <summary>
    /// Updates the health on the health bar, as well as its color depending on 
    /// whether the boss is stunned or not
    /// </summary>
    private void UpdateUI()
    {
        uiManager.SetBossHealthFill(health / maxHealth);
        uiManager.SetBossHealthColor(state == State.Stunned);
    }

    /// <summary>
    /// Resets the damage hitboxes' hits so the attacks can damage the player 
    /// again (Called in the animator)
    /// </summary>
    public void ResetHits()
    {
        foreach(BossDamageHitbox bh in hitboxes)
        {
            bh.hitPlayer = false;
        }
    }

    /// <summary>
    /// Idle behaviour, mostly to transition between states or when all attacks 
    /// are on cooldown
    /// </summary>
    private void StartIdling()
    {
        state = State.Idle;

        navMeshAgent.isStopped = true;
        animator.SetFloat("Velocity", 0f);
        navMeshAgent.speed = 0f;

        SelectAttack();
    }

    /// <summary>
    /// Chasing behaviour, the boss moves towards the player if they're out of 
    /// range for the attacks to hit.
    /// </summary>
    private void StartChasing()
    {
        state = State.Chasing;
        
        navMeshAgent.SetDestination(playerHealth.transform.position);

        navMeshAgent.speed = runSpeed;
        navMeshAgent.isStopped = false;

        animator.SetFloat("Velocity", navMeshAgent.speed);
    }

    /// <summary>
    /// Attack behaviour, stops and uses a randomly selected attack if the 
    /// player is close enough.
    /// </summary>
    private void StartAttacking()
    {
        state = State.Attacking;

        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
        animator.SetFloat("Velocity", 0);
    }

    /// <summary>
    /// Uses the currently selected attack and triggers its cooldown.
    /// </summary>
    private void Attack()
    {
        animator.SetTrigger(attacks[CurrentAttack].Item1);
        attackCooldowns[CurrentAttack] = attacks[CurrentAttack].Item4;

        stateTimer = Random.Range(0f, 2f);

        SelectAttack();
    }

    /// <summary>
    /// Selects a random attack to use (different than the previous one used).
    /// </summary>
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

    /// <summary>
    /// Stunned behaviour, called by the animator after using the Bull Charge
    /// attack and lasting until the timer is up.
    /// </summary>
    private void StartStunning()
    {
        state = State.Stunned;

        stunTimer = 5f;

        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
        animator.SetTrigger("Stun");
    }

    /// <summary>
    /// Death behaviour, kills the entity when its hp falls to 0.
    /// </summary>
    private void Die()
    {
        state = State.Dead;

        navMeshAgent.isStopped = true;
        animator.SetFloat("Velocity", 0);
        animator.SetTrigger("Die");

        uiManager.ToggleBossHPBar();
    }

    /// <summary>
    /// Updates the UI and the current state of the Boss.
    /// </summary>
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

    /// <summary>
    /// Lowers the cooldowns and timers in real time.
    /// </summary>
    private void FixedUpdate()
    {
        stateTimer -= Time.deltaTime;

        for(int i = 0; i < attackCooldowns.Count; i++)
        {
            attackCooldowns[i] = Mathf.Max(attackCooldowns[i] - Time.deltaTime, 0f);
        }
    }

    /// <summary>
    /// Updates if currently in the Idle behaviour.
    /// </summary>
    private void UpdateIdle()
    {
        if (Vector3.Distance(playerHealth.transform.position, transform.position) > CurrentAttackRange)
            StartChasing();

        else if (attackCooldowns[CurrentAttack] <= 0 && Vector3.Distance(playerHealth.transform.position, transform.position) <= CurrentAttackRange)
            StartAttacking();

        else
            StartIdling();
    }

    /// <summary>
    /// Updates if currently in the stun behaviour.
    /// </summary>
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

    /// <summary>
    /// Updates if currently in the chase behaviour.
    /// </summary>
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

    /// <summary>
    /// Updates if currently in the attack behaviour.
    /// </summary>
    private void UpdateAttack()
    {
        if (stateTimer <= 0f && attackCooldowns[CurrentAttack] <= 0)
            Attack();
        else
            StartIdling();
    }

    /// <summary>
    /// Receives given damage only if stunned, and dies if health goes below zero.
    /// </summary>
    /// <param name="amount">The amount of damage received.</param>
    public void Damage(float amount)
    {
        if (state != State.Stunned)
            return;

        Health -= amount;

        if (health <= 0)
            Die();

        UpdateUI();
    }

    public void EndGameTrigger()
    {
        SaveFileHandler saveFileHandler = FindObjectOfType<SaveFileHandler>();
        saveFileHandler.TriggerWinScreen = true;
        Cursor.lockState = CursorLockMode.None;
        uiManager.SaveAndQuit();
    }

    public void PlaySlash1()
    {
        slash1AudioSource.pitch = Random.Range(0.9f, 1.1f);
        slash1AudioSource.PlayOneShot(slash1);
    }

    public void PlaySlash2()
    {
        slash2AudioSource.pitch = Random.Range(0.9f, 1.1f);
        slash2AudioSource.PlayOneShot(slash2);
    }

    public void PlayDeath()
    {
        deathAudioSource.PlayOneShot(death);
    }

    public void PlayStep()
    {
        stepsAudioSource.pitch = Random.Range(0.85f, 1.15f);
        stepsAudioSource.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)]);
    }

    public void PlayStomp()
    {
        stompAudioSource.pitch = Random.Range(0.9f, 1.1f);
        stompAudioSource.PlayOneShot(stomp);
    }

    public void PlayCharge()
    {
        chargeAudioSource.pitch = Random.Range(0.9f, 1.1f);
        chargeAudioSource.PlayOneShot(charge);
    }

    public void PlayUnstun()
    {
        unstunAudioSource.pitch = Random.Range(0.9f, 1.1f);
        unstunAudioSource.PlayOneShot(unstun);
    }
}
