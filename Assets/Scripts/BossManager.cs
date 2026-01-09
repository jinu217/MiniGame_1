using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BossManager : MonoBehaviour
{
    [Header("Config")]
    public BossConfig config;
    public Transform spawnPoint;
    public float timeLimit = 140f;

    [Header("Boss Spawn Delay")]
    public float bossSpawnDelay = 90f;

    [Header("Pattern Overrides")]
    public bool overrideInterval = true;
    public bool overrideSpeed = true;
    public bool overrideVolley = true;

    BossBase boss;
    float timer;
    int currentPhaseIndex = -1;
    Coroutine battleRoutine;
    Coroutine spawnRoutine;

    bool bossSpawned = false;
    bool ended = false;

    public static BossManager Instance { get; private set; }
    public BossBase CurrentBoss => boss;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (config == null || config.bossPrefab == null || config.phases == null || config.phases.Length == 0)
        {
            enabled = false;
            return;
        }

        bossSpawned = false;
        ended = false;

        spawnRoutine = StartCoroutine(SpawnBossAfterDelay());
    }

    IEnumerator SpawnBossAfterDelay()
    {
        if (bossSpawnDelay > 0f)
            yield return new WaitForSeconds(bossSpawnDelay);

        var spawnPos = spawnPoint ? spawnPoint.position : transform.position;
        var go = Instantiate(config.bossPrefab, spawnPos, Quaternion.identity);

        boss = go.GetComponent<BossBase>();
        if (boss == null)
        {
            enabled = false;
            yield break;
        }

        boss.Init(config.maxHP);

        bossSpawned = true;
        timer = 0f;
        currentPhaseIndex = -1;
        ApplyPhase(0);

        battleRoutine = StartCoroutine(BattleLoop());
    }

    IEnumerator BattleLoop()
    {
        while (!ended)
        {
            timer += Time.deltaTime;

            while (currentPhaseIndex + 1 < config.phases.Length &&
                   timer >= config.phases[currentPhaseIndex + 1].startAtSeconds)
            {
                ApplyPhase(currentPhaseIndex + 1);
            }

            if ((bossSpawned && boss == null) || (boss != null && boss.IsDead))
            {
                OnBossDefeated();
                yield break;
            }

            if (timer >= timeLimit)
            {
                OnTimeOver();
                yield break;
            }

            yield return null;
        }
    }

    void ApplyPhase(int index)
    {
        currentPhaseIndex = index;
        var p = config.phases[index];

        if (boss != null)
            boss.SetPattern(
                p.pattern,
                overrideInterval ? p.fireInterval : 0f,
                overrideSpeed ? p.projectileSpeed : 0f,
                overrideVolley ? p.volleyCount : 0
            );
    }

    void OnBossDefeated()
    {
        if (ended) return;
        ended = true;

        Debug.Log("Boss Down!");

        UIFlowManager.Instance?.OnBossDefeated();
    }

    void OnTimeOver()
    {
        if (ended) return;
        ended = true;

        Debug.Log("Time Over");
    }
}
