using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CombatManager - Central manager that oversees all combat in the game.
/// Responsibilities: Track combat encounters, manage global settings, coordinate systems.
/// </summary>
public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    public enum CombatState
    {
        NotInCombat,
        InCombat,
        Victory,
        Defeat
    }

    private CombatState currentState = CombatState.NotInCombat;
    private float combatDuration = 0f;
    private int totalEnemiesDefeated = 0;

    private PlayerCombatSystem playerCombat;
    private List<EnemyCombatSystem> activeEnemies = new List<EnemyCombatSystem>();

    [SerializeField] private float damageMultiplier = 1f;
    [SerializeField] private float enemyDamageMultiplier = 1f;
    [SerializeField] private float healthMultiplier = 1f;

    public delegate void CombatStateChangedDelegate(CombatState newState);
    public delegate void EnemyDefeatedDelegate(GameObject enemy);
    public delegate void CombatEndedDelegate(bool playerWon);

    public event CombatStateChangedDelegate OnCombatStateChanged;
    public event EnemyDefeatedDelegate OnEnemyDefeated;
    public event CombatEndedDelegate OnCombatEnded;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        playerCombat = FindObjectOfType<PlayerCombatSystem>();

        EnemyCombatSystem[] enemies = FindObjectsOfType<EnemyCombatSystem>();
        foreach (EnemyCombatSystem enemy in enemies)
        {
            RegisterEnemy(enemy);
        }
    }

    void Update()
    {
        if (currentState == CombatState.InCombat)
        {
            combatDuration += Time.deltaTime;

            if (activeEnemies.Count == 0 && !playerCombat.GetComponent<HealthSystem>().IsDead())
            {
                EndCombat(true);
            }
        }
    }

    /// <summary>Registers an enemy with the combat manager to track them.</summary>
    public void RegisterEnemy(EnemyCombatSystem enemy)
    {
        if (!activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);

            HealthSystem enemyHealth = enemy.GetComponent<HealthSystem>();
            if (enemyHealth != null)
            {
                enemyHealth.OnDeath += () => OnEnemyDeath(enemy);
            }
        }
    }

    /// <summary>Called when an enemy is defeated.</summary>
    private void OnEnemyDeath(EnemyCombatSystem enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            totalEnemiesDefeated++;

            OnEnemyDefeated?.Invoke(enemy.gameObject);
            Debug.Log($"Enemy defeated! Total defeated: {totalEnemiesDefeated}");

            if (activeEnemies.Count == 0 && currentState == CombatState.InCombat)
            {
                EndCombat(true);
            }
        }
    }

    /// <summary>Starts a combat encounter.</summary>
    public void StartCombat()
    {
        if (currentState == CombatState.InCombat)
            return;

        currentState = CombatState.InCombat;
        combatDuration = 0f;
        OnCombatStateChanged?.Invoke(currentState);

        Debug.Log("Combat started! Enemies in encounter: " + activeEnemies.Count);
    }

    /// <summary>Ends the current combat encounter.</summary>
    private void EndCombat(bool playerWon)
    {
        CombatState newState = playerWon ? CombatState.Victory : CombatState.Defeat;
        currentState = newState;

        OnCombatStateChanged?.Invoke(currentState);
        OnCombatEnded?.Invoke(playerWon);

        string resultText = playerWon ? "VICTORY" : "DEFEAT";
        Debug.Log($"Combat ended - {resultText}! Duration: {combatDuration:F1}s");
    }

    public void SetPlayerDamageMultiplier(float multiplier) => damageMultiplier = multiplier;
    public void SetEnemyDamageMultiplier(float multiplier) => enemyDamageMultiplier = multiplier;
    public CombatState GetCombatState() => currentState;
    public float GetCombatDuration() => combatDuration;
    public int GetEnemiesDefeated() => totalEnemiesDefeated;
    public int GetActiveEnemyCount() => activeEnemies.Count;
}
