using UnityEngine;
using System.Collections;

public class Boss_Stage4 : BossBase
{
    [Header("Common (Grow+Accel)")]
    [SerializeField] float startScale       = 0.25f; // 3스테이지처럼 작게 시작
    [SerializeField] float endScale         = 1.0f;  // 점점 커짐 (Inspector에서 3~10도 가능)
    [SerializeField] float growDuration     = 0.9f;  // 성장 완료 시간
    [SerializeField] float accelPerSecond   = 14f;   // 진행 방향 가속
    [SerializeField] float initialSpeedRate = 0.45f; // 초기 속도 배율 (projSpeed * 이 값)

    [Header("Phase1 (Straight like Stage3)")]
    [SerializeField] float p1Gap = 0.12f;   // 줄사격 간격(초)

    [Header("Phase2 (Alternate: Straight ↔ Fan)")]
    [SerializeField] float p2StraightGap = 0.12f; // 직선 줄사격 간격
    [SerializeField] int   fanCount      = 5;     // 부채꼴 발수
    [SerializeField] float fanTotalAngle = 15f;   // 부채꼴 총각도

    bool doStraightThisTime = true; // 2페이즈에서 번갈아 토글

    protected override void FireOnce()
    {
        if (!firePoint || !projectilePrefab) return;

        switch (pattern)
        {
            // Phase1: 3스테이지와 동일(직선 줄사격 + 성장/가속)
            case BossPatternType.Straight:
                StartCoroutine(FireStraightSeq_Grow(Mathf.Max(1, volley), p1Gap));
                break;

            // Phase2: 직선(성장) 1번 → 부채꼴(성장) 1번 번갈아
            case BossPatternType.Mixed:
                if (doStraightThisTime)
                    StartCoroutine(FireStraightSeq_Grow(Mathf.Max(1, volley), p2StraightGap));
                else
                    FireFanAtPlayer_Grow(fanCount, fanTotalAngle);

                doStraightThisTime = !doStraightThisTime;
                break;

            default:
                base.FireOnce();
                break;
        }
    }

    // ===== Helpers =====

    IEnumerator FireStraightSeq_Grow(int count, float gap)
    {
        Vector3 dir = GetDirToPlayer3D();
        if (dir == Vector3.zero) yield break;

        for (int i = 0; i < count; i++)
        {
            SpawnBulletGrowAccel(dir);
            if (i < count - 1) yield return new WaitForSeconds(gap);
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
        // 1) 스폰
        var go = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(dir));

        // 2) 물리
        var rb = go.GetComponent<Rigidbody>() ?? go.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.linearVelocity = dir * (projSpeed * Mathf.Clamp01(initialSpeedRate));

        // 3) 성장/가속
        var grow = go.GetComponent<BulletGrowAndAccelerate>() ?? go.AddComponent<BulletGrowAndAccelerate>();
        grow.Configure(startScale, endScale, growDuration, accelPerSecond, 6f);
        grow.BeginGrow(); // ‘작→대’ 성장 시작
    }
}
