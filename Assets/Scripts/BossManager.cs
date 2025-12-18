using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BossManager : MonoBehaviour
{
    [Header("Config")]
    public BossConfig config;
    public Transform spawnPoint;
    public float timeLimit = 35f;

    [Header("Boss Spawn Delay")]
    public float bossSpawnDelay = 10f;

    [Header("Stage Clear UI")]
    public StageClearPanel stageClearPanel;   

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
    bool ended = false; // ✅ 처치/타임오버로 게임 종료 상태

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

        if (battleRoutine != null) StopCoroutine(battleRoutine);
        battleRoutine = StartCoroutine(BattleLoop());
    }

    void OnDisable()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }

        if (battleRoutine != null)
        {
            StopCoroutine(battleRoutine);
            battleRoutine = null;
        }
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

            // 보스가 Destroy되어 null이 되면 처치로 간주
            if (bossSpawned && boss == null)
            {
                OnBossDefeated();
                yield break;
            }

            if (boss != null && boss.IsDead)
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

        float interval = overrideInterval ? p.fireInterval : 0f;
        float speed = overrideSpeed ? p.projectileSpeed : 0f;
        int volley = overrideVolley ? p.volleyCount : 0;

        if (boss != null)
            boss.SetPattern(p.pattern, interval, speed, volley);
    }

    void OnBossDefeated()
    {
        if (ended) return;
        ended = true;

        Debug.Log("Boss Down!");

        // ✅ 씬 이동 X → 패널 띄우기(패널이 Time.timeScale=0 처리)
        if (stageClearPanel != null)
        {
            stageClearPanel.Open();
        }
        else
        {
            Debug.LogError("[BossManager] stageClearPanel이 연결되지 않았습니다. (StageClear 패널을 BossManager에 연결하세요)");
        }
    }

    void OnTimeOver()
    {
        if (ended) return;
        ended = true;

        Debug.Log("Time Over");
        // TODO: 여기서도 게임오버 패널을 띄우고 싶으면 같은 방식으로 처리
        // 예) gameOverPanel.Open();
    }

    // (BossManager에서 더 이상 씬 넘기지 않으므로 ParseStageNumber는 필요 없어짐)
}
