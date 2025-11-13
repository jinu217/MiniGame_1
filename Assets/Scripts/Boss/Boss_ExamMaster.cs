using UnityEngine;
using System.Collections;
 
public class Boss_ExamMaster : BossBase
{
    [Header("Phase1 (Basic)")]
    [SerializeField] float p1Gap = 0.12f; // 줄지어 간격

    [Header("Phase2 (Grow + Accelerate)")]
    [SerializeField] float p2Gap              = 0.15f;  // 줄지어 간격(4연발 내부)
    [SerializeField] float p2StartScale       = 0.25f;  // 시작 배율
    [SerializeField] float p2EndScale         = 1.0f;   // 최종 배율 (3~10 권장)
    [SerializeField] float p2GrowDuration     = 0.9f;   // 성장 완료까지 시간
    [SerializeField] float p2AccelPerSecond   = 14f;    // 초당 가속량
    [SerializeField] float p2InitialSpeedRate = 0.45f;  // 초기 속도 배율 (projSpeed * 이 값)

    [Header("Phase2 Cadence (빠름/느림 텀)")]
    [SerializeField] int   p2VolleyFixed   = 4;    // 4연발 고정
    [SerializeField] float p2FastInterval  = 0.6f; // 다음 발사까지 빠른 텀
    [SerializeField] float p2SlowInterval  = 1.5f; // 다음 발사까지 느린 텀
    int p2Cycle = 0; // 0,1=빠름 / 2=느림 -> 반복

    protected override void FireOnce()
    {
        if (!firePoint || !projectilePrefab) return;

        switch (pattern)
        {
            // Phase1: 직선 줄지어 발사 (volley 반영)
            case BossPatternType.Straight:
                StartCoroutine(FireStraightSeq(Mathf.Max(1, volley), p1Gap));
                break;

            // Phase2: 4연발 + 성장/가속, 그리고 발사 텀을 "빠름, 빠름, 느림" 순환
            case BossPatternType.Mixed:
                StartCoroutine(FireGrowAccelSeq_Cadence(p2VolleyFixed, p2Gap));
                break;

            default:
                base.FireOnce();
                break;
        }
    }

    IEnumerator FireGrowAccelSeq_Cadence(int count, float gap)
    {
        Vector3 dir = GetDirToPlayer3D();
        if (dir == Vector3.zero) yield break;

        // 4연발(내부 간격 p2Gap), 각 탄은 작→대 성장/가속
        count = Mathf.Max(1, count);
        for (int i = 0; i < count; i++)
        {
            SpawnBulletGrowAccel(dir, p2StartScale, p2EndScale, p2GrowDuration, p2AccelPerSecond, p2InitialSpeedRate);
            if (i < count - 1) yield return new WaitForSeconds(gap);
        }

        // 다음 발사까지의 간격을 즉석에서 조정: 빠름, 빠름, 느림 반복
        if (p2Cycle == 0 || p2Cycle == 1) fireInterval = p2FastInterval;
        else                               fireInterval = p2SlowInterval;
        p2Cycle = (p2Cycle + 1) % 3;
    }

    void SpawnBulletGrowAccel(
        Vector3 dir,
        float startScale,
        float endScale,
        float growDuration,
        float accelPerSec,
        float initialSpeedRate)
    {
        // 1) 탄환 스폰
        var go = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(dir));

        // 2) 물리 세팅
        var rb = go.GetComponent<Rigidbody>() ?? go.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.linearVelocity = dir * (projSpeed * Mathf.Clamp01(initialSpeedRate));

        // 3) 성장/가속 컴포넌트
        var grow = go.GetComponent<BulletGrowAndAccelerate>() ?? go.AddComponent<BulletGrowAndAccelerate>();
        grow.Configure(startScale, endScale, growDuration, accelPerSec, 6f);
        grow.BeginGrow();
    }
}
