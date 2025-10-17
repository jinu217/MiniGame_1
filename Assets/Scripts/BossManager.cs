using UnityEngine;
using System.Collections;

public class BossManager : MonoBehaviour
{
    [Header("Config")]
    public BossConfig config;
    public Transform spawnPoint;
    public float timeLimit = 35f;

    [Header("Pattern Overrides")]
    public bool overrideInterval = true;
    public bool overrideSpeed = true;
    public bool overrideVolley = true;

    BossBase boss;
    float timer;
    int currentPhaseIndex = -1;
    Coroutine battleRoutine;

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

            while (currentPhaseIndex + 1 < config.phases.Length &&
                   timer >= config.phases[currentPhaseIndex + 1].startAtSeconds)
            {
                ApplyPhase(currentPhaseIndex + 1);
            }

            if (boss.IsDead) { Debug.Log("Boss Down!"); yield break; }
            if (timer >= timeLimit) { Debug.Log("Time Over"); yield break; }

            yield return null;
        }
    }

    void ApplyPhase(int index)
    {
        currentPhaseIndex = index;
        var p = config.phases[index];

        float interval = overrideInterval ? p.fireInterval    : 0f;
        float speed    = overrideSpeed    ? p.projectileSpeed : 0f;
        int   volleyCt = overrideVolley   ? p.volleyCount     : 0;

        boss.SetPattern(p.pattern, interval, speed, volleyCt);
    }
}
