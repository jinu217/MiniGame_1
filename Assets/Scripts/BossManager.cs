using UnityEngine;
using System.Collections;

public class BossManager : MonoBehaviour
{
    [Header("Config")]
    public BossConfig config;
    public Transform spawnPoint;
    [Tooltip("보스전 제한시간(초)")]
    public float timeLimit = 35f;

    [Header("Pattern Overrides")]
    [Tooltip("켜면 페이즈의 fireInterval로 덮어씀, 끄면 인스펙터 값 유지")]
    public bool overrideInterval = true;
    [Tooltip("켜면 페이즈의 projectileSpeed로 덮어씀, 끄면 인스펙터 값 유지")]
    public bool overrideSpeed = true;
    [Tooltip("켜면 페이즈의 volleyCount로 덮어씀, 끄면 인스펙터 값 유지")]
    public bool overrideVolley = true;

    BossBase boss;
    float timer;
    int currentPhaseIndex = -1;
    Coroutine battleRoutine;

    // ✅ UI나 다른 시스템에서 접근하기 위한 싱글톤 & 현재 보스
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
            Debug.LogError("[BossManager] Config/Prefab/Phases 누락!");
            enabled = false;
            return;
        }

        var spawnPos = spawnPoint ? spawnPoint.position : transform.position;
        var go = Instantiate(config.bossPrefab, spawnPos, Quaternion.identity);

        boss = go.GetComponent<BossBase>();
        if (boss == null)
        {
            Debug.LogError("[BossManager] BossBase 컴포넌트가 프리팹에 없음!");
            enabled = false;
            return;
        }

        boss.Init(config.maxHP);

        timer = 0f;
        ApplyPhase(0);
        battleRoutine = StartCoroutine(BattleLoop());
    }

    void OnDisable()
    {
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

            // 다음 페이즈로 여러 단계 건너뛸 수 있으니 while로 처리
            while (currentPhaseIndex + 1 < config.phases.Length &&
                   timer >= config.phases[currentPhaseIndex + 1].startAtSeconds)
            {
                ApplyPhase(currentPhaseIndex + 1);
            }

            if (boss.IsDead) { OnBossDefeated(); yield break; }
            if (timer >= timeLimit) { OnTimeOver(); yield break; }

            yield return null;
        }
    }

    void ApplyPhase(int index)
    {
        currentPhaseIndex = index;
        var p = config.phases[index];

        // 토글에 따라 덮어쓸 값/유지할 값 분기
        float interval = overrideInterval ? p.fireInterval    : 0f; // 0 → 인스펙터 값 유지
        float speed    = overrideSpeed    ? p.projectileSpeed : 0f;
        int   volley   = overrideVolley   ? p.volleyCount     : 0;

        boss.SetPattern(p.pattern, interval, speed, volley);

        // TODO: UI 연출 (페이즈 전환 알림)
        // PhaseBanner.Show(p.displayName, 1.0f);
    }

    void OnBossDefeated()
    {
        Debug.Log("Boss Down!");
        // TODO: 보상/다음 스테이지
    }

    void OnTimeOver()
    {
        Debug.Log("Time Over");
        // TODO: 실패 처리
    }
}
