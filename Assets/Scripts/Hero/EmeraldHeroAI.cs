using UnityEngine;
using EmeraldAI;
using UnityEngine.AI;

/// <summary>
/// AI controller using Emerald AI 2024 for hero behavior.
/// Processes player commands and manages pathfinding/combat.
/// </summary>
public class EmeraldHeroAI : MonoBehaviour
{
    public static EmeraldHeroAI Instance { get; private set; }

    [Header("Emerald AI Components")]
    private EmeraldSystem emeraldSystem;
    private NavMeshAgent navAgent;
    private GameObject heroGameObject;
    
    [Header("AI Settings")]
    [SerializeField] private float commandResponseDelay = 0.5f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int maxHealth = 100;
    
    private GameObject currentTarget;
    private bool isExecutingCommand = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetHeroGameObject(GameObject hero)
    {
        heroGameObject = hero;
        
        // Get or add Emerald AI components
        emeraldSystem = hero.GetComponent<EmeraldSystem>();
        if (emeraldSystem == null)
        {
            emeraldSystem = hero.AddComponent<EmeraldSystem>();
            ConfigureEmeraldAI();
        }
        
        navAgent = hero.GetComponent<NavMeshAgent>();
        if (navAgent == null)
        {
            navAgent = hero.AddComponent<NavMeshAgent>();
            ConfigureNavAgent();
        }
        
        Debug.Log("[EmeraldHeroAI] Hero AI configured");
    }

    private void ConfigureEmeraldAI()
    {
        // Configure Emerald AI for friendly hero behavior
        emeraldSystem.BehaviorSettings.CurrentBehavior = CurrentBehavior.Companion;
        emeraldSystem.DetectionSettings.DetectionType = DetectionType.LineOfSight;
        emeraldSystem.StatsSettings.StartingHealth = maxHealth;
        emeraldSystem.StatsSettings.CurrentHealth = maxHealth;
        
        // Set up combat events
        emeraldSystem.CombatEvents.OnTakeDamage.AddListener(OnHeroTakeDamage);
        emeraldSystem.CombatEvents.OnDeath.AddListener(OnHeroDeath);
    }

    private void ConfigureNavAgent()
    {
        navAgent.speed = 3.5f;
        navAgent.angularSpeed = 120f;
        navAgent.stoppingDistance = 1f;
        navAgent.radius = 0.5f;
        navAgent.height = 2f;
    }

    public void MoveTo(Vector3 destination)
    {
        if (navAgent == null || !navAgent.isActiveAndEnabled) return;
        
        // Clear current target when moving
        currentTarget = null;
        if (emeraldSystem != null)
        {
            emeraldSystem.CombatComponent.ClearTarget();
        }
        
        // Set destination
        navAgent.SetDestination(destination);
        isExecutingCommand = true;
        
        Debug.Log($"[EmeraldHeroAI] Moving to {destination}");
    }

    public void AttackTarget(GameObject target)
    {
        if (emeraldSystem == null || target == null) return;
        
        currentTarget = target;
        
        // Set combat target
        emeraldSystem.DetectionComponent.SetDetectedTarget(target.transform);
        emeraldSystem.CombatComponent.SetTarget(target.transform);
        
        // Move into attack range if needed
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance > attackRange)
        {
            Vector3 attackPosition = target.transform.position - (target.transform.position - transform.position).normalized * (attackRange - 0.5f);
            MoveTo(attackPosition);
        }
        
        isExecutingCommand = true;
        Debug.Log($"[EmeraldHeroAI] Attacking {target.name}");
    }

    public void ProcessCommand(string command)
    {
        // Convert command to lowercase for easier processing
        string cmd = command.ToLower();
        
        if (cmd.Contains("attack") || cmd.Contains("fight"))
        {
            // Find nearest enemy
            GameObject nearestEnemy = FindNearestEnemy();
            if (nearestEnemy != null)
            {
                AttackTarget(nearestEnemy);
            }
            else
            {
                Debug.Log("[EmeraldHeroAI] No enemies found to attack");
            }
        }
        else if (cmd.Contains("follow"))
        {
            // Follow player
            FollowPlayer();
        }
        else if (cmd.Contains("stay") || cmd.Contains("stop"))
        {
            // Stop current action
            StopCurrentAction();
        }
        else if (cmd.Contains("defend"))
        {
            // Defensive stance
            SetDefensiveMode();
        }
        else if (cmd.Contains("explore"))
        {
            // Explore nearby area
            ExploreArea();
        }
        
        Debug.Log($"[EmeraldHeroAI] Processing command: {command}");
    }

    private GameObject FindNearestEnemy()
    {
        GameObject nearest = null;
        float nearestDistance = float.MaxValue;
        
        // Find all enemies with Emerald AI
        EmeraldSystem[] allAI = FindObjectsOfType<EmeraldSystem>();
        foreach (var ai in allAI)
        {
            if (ai.gameObject != heroGameObject && ai.BehaviorSettings.CurrentBehavior == CurrentBehavior.Aggressive)
            {
                float distance = Vector3.Distance(transform.position, ai.transform.position);
                if (distance < nearestDistance)
                {
                    nearest = ai.gameObject;
                    nearestDistance = distance;
                }
            }
        }
        
        return nearest;
    }

    private void FollowPlayer()
    {
        GameObject player = GameManager.Instance?.GetVRPlayer();
        if (player != null)
        {
            Vector3 followPosition = player.transform.position - player.transform.forward * 2f;
            MoveTo(followPosition);
        }
    }

    private void StopCurrentAction()
    {
        if (navAgent != null)
        {
            navAgent.isStopped = true;
        }
        
        if (emeraldSystem != null)
        {
            emeraldSystem.CombatComponent.ClearTarget();
        }
        
        currentTarget = null;
        isExecutingCommand = false;
    }

    private void SetDefensiveMode()
    {
        if (emeraldSystem != null)
        {
            emeraldSystem.BehaviorSettings.CurrentBehavior = CurrentBehavior.Cautious;
            emeraldSystem.CombatSettings.AttackDistance = attackRange * 0.5f;
        }
    }

    private void ExploreArea()
    {
        // Pick a random point within exploration radius
        Vector3 randomDirection = Random.insideUnitSphere * 10f;
        randomDirection += transform.position;
        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas))
        {
            MoveTo(hit.position);
        }
    }

    public float GetCurrentHealth()
    {
        return emeraldSystem != null ? emeraldSystem.StatsSettings.CurrentHealth : maxHealth;
    }

    public float GetMaxHealth()
    {
        return emeraldSystem != null ? emeraldSystem.StatsSettings.StartingHealth : maxHealth;
    }

    private void OnHeroTakeDamage()
    {
        Debug.Log($"[EmeraldHeroAI] Hero took damage! Health: {GetCurrentHealth()}/{GetMaxHealth()}");
        
        // Alert UI
        if (VRIFUIManager.Instance != null)
        {
            VRIFUIManager.Instance.UpdateHealthDisplay(GetCurrentHealth(), GetMaxHealth());
        }
    }

    private void OnHeroDeath()
    {
        Debug.Log("[EmeraldHeroAI] Hero has died!");
        
        // Notify game manager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SwitchState(GameManager.GameState.Menu);
        }
    }

    private void Update()
    {
        // Check if we've reached our destination
        if (isExecutingCommand && navAgent != null && !navAgent.pathPending)
        {
            if (navAgent.remainingDistance <= navAgent.stoppingDistance)
            {
                isExecutingCommand = false;
                
                // If we have a target, ensure we're attacking
                if (currentTarget != null && emeraldSystem != null)
                {
                    emeraldSystem.CombatComponent.AttackTarget();
                }
            }
        }
    }
} 