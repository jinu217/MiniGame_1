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
    bool transitioning = false;

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
        transitioning = false;

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
        while (true)
        {
            timer += Time.deltaTime;

            while (currentPhaseIndex + 1 < config.phases.Length &&
                   timer >= config.phases[currentPhaseIndex + 1].startAtSeconds)
            {
                ApplyPhase(currentPhaseIndex + 1);
            }

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
        float speed    = overrideSpeed    ? p.projectileSpeed : 0f;
        int volley      = overrideVolley   ? p.volleyCount : 0;

        if (boss != null)
            boss.SetPattern(p.pattern, interval, speed, volley);
    }

    void OnBossDefeated()
    {
        if (transitioning) return;
        transitioning = true;

        Debug.Log("Boss Down!");

        // 현재 씬 이름으로 스테이지 번호 파싱
        string current = SceneManager.GetActiveScene().name;
        int stageNum = ParseStageNumber(current);

        if (stageNum >= 5)
        {
            Debug.Log("-> Load Scene: GameClear");
            SceneManager.LoadScene("GameClear");
            return;
        }

        string nextScene = "Stage" + (stageNum + 1);

        // ✅ 씬 넘어가기 직전에 몇 스테이지로 가는지 로그
        Debug.Log($"-> Load Scene: {nextScene} (Stage {stageNum + 1})");

        SceneManager.LoadScene(nextScene);
    }

    void OnTimeOver()
    {
        if (transitioning) return;
        transitioning = true;

        Debug.Log("Time Over");
    }

    int ParseStageNumber(string sceneName)
    {
        // "Stage3" -> 3
        if (!sceneName.StartsWith("Stage")) return 0;

        string numStr = sceneName.Replace("Stage", "");
        int num;
        if (int.TryParse(numStr, out num)) return num;

        return 0;
    }
}
