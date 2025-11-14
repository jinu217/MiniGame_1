using UnityEngine;
using System.Collections;

public class Boss_Stage4 : BossBase
{
    [Header("Common (Grow + Accelerate)")]
    [SerializeField] float startScale       = 0.25f;
    [SerializeField] float endScale         = 1.0f;   // Inspector에서 3~10 추천
    [SerializeField] float growDuration     = 0.9f;
    [SerializeField] float accelPerSecond   = 14f;
    [SerializeField] float initialSpeedRate = 0.45f;  // projSpeed * rate

    [Header("Phase1 (Straight 4-burst)")]
    [SerializeField] float p1Gap       = 0.12f; // 4연발 내부 간격
    [SerializeField] int   p1VolleyFix = 4;     // 4로 고정

    [Header("Phase2 (4-burst → Delay1 → Fan x2 → Delay2 반복)")]
    [SerializeField] int   p2VolleyFix        = 4;     // 4발 고정
    [SerializeField] float p2StraightGap      = 0.12f; // 4연발 내부 간격
    [SerializeField] float p2DelayAfterStraight = 0.8f; // ✅ 딜레이1(직선 이후)
    [SerializeField] int   fanCount           = 5;     // 부채꼴 동시 발사 수
    [SerializeField] float fanTotalAngle      = 20f;   // 부채꼴 총 각도
    [SerializeField] int   fanRepeats         = 2;     // 부채꼴 2연발
    [SerializeField] float fanRepeatGap       = 0.25f; // 부채꼴 사이 간격
    [SerializeField] float p2DelayAfterFan    = 1.1f;  // ✅ 딜레이2(부채꼴 이후) — 딜레이1보다 조금 길게

    Coroutine phase2Routine;

    protected override void FireOnce()
    {
        if (!firePoint || !projectilePrefab) return;

        switch (pattern)
        {
            // Phase1: 직선 4연발(모두 성장/가속)
            case BossPatternType.Straight:
                StartCoroutine(FireStraightSeq_Grow(p1VolleyFix, p1Gap));
                break;

            // Phase2: 4연발 → (딜레이1) → 부채꼴 2연발 → (딜레이2) → 반복
            case BossPatternType.Mixed:
                if (phase2Routine == null)
                    phase2Routine = StartCoroutine(Phase2Loop());
                break;

            default:
                base.FireOnce();
                break;
        }
    }

    IEnumerator Phase2Loop()
    {
        // 딜레이2가 딜레이1보다 짧게 세팅돼 있으면 최소한 같게 맞춰서 리듬 유지
        if (p2DelayAfterFan < p2DelayAfterStraight)
            p2DelayAfterFan = p2DelayAfterStraight + 0.1f;

        while (true)
        {
            // ① 직선 4연발
            yield return StartCoroutine(FireStraightSeq_Grow(p2VolleyFix, p2StraightGap));

            // ② 딜레이1
            if (p2DelayAfterStraight > 0f)
                yield return new WaitForSeconds(p2DelayAfterStraight);

            // ③ 부채꼴 2연발
            for (int r = 0; r < fanRepeats; r++)
            {
                FireFanAtPlayer_Grow(fanCount, fanTotalAngle);
                if (r < fanRepeats - 1 && fanRepeatGap > 0f)
                    yield return new WaitForSeconds(fanRepeatGap);
            }

            // ④ 딜레이2 (딜레이1보다 조금 길게)
            if (p2DelayAfterFan > 0f)
                yield return new WaitForSeconds(p2DelayAfterFan);
        }
    }

    IEnumerator FireStraightSeq_Grow(int count, float gap)
    {
        Vector3 dir = GetDirToPlayer3D();
        if (dir == Vector3.zero) yield break;

        for (int i = 0; i < count; i++)
        {
            SpawnBulletGrowAccel(dir);
            if (i < count - 1 && gap > 0f)
                yield return new WaitForSeconds(gap);
        }
    }

    void FireFanAtPlayer_Grow(int count, float totalAngle)
    {
        Vector3 forward = GetDirToPlayer3D();
        if (forward == Vector3.zero) return;

        Quaternion baseRot = Quaternion.LookRotation(forward);
        float step  = count > 1 ? totalAngle / (count - 1) : 0f;
        float start = -totalAngle * 0.5f;

        for (int i = 0; i < count; i++)
        {
            float yaw = start + step * i;
            Quaternion rot = Quaternion.Euler(0f, yaw, 0f) * baseRot;
            SpawnBulletGrowAccel(rot * Vector3.forward);
        }
    }

    void SpawnBulletGrowAccel(Vector3 dir)
    {
        var go = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(dir));

        var rb = go.GetComponent<Rigidbody>() ?? go.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.linearVelocity = dir * (projSpeed * Mathf.Clamp01(initialSpeedRate));

        var grow = go.GetComponent<BulletGrowAndAccelerate>() ?? go.AddComponent<BulletGrowAndAccelerate>();
        grow.Configure(startScale, endScale, growDuration, accelPerSecond, 6f);
        grow.BeginGrow();
    }

    void OnDisable()
    {
        if (phase2Routine != null)
        {
            StopCoroutine(phase2Routine);
            phase2Routine = null;
        }
    }
}
