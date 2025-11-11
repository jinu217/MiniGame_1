using UnityEngine;
using System.Collections;
 
public class Boss_ExamMaster : BossBase
{
    [Header("Phase1 (Basic)")]
    [SerializeField] float p1Gap = 0.12f; // 줄지어 간격

    [Header("Phase2 (Grow + Accelerate)")]
    [SerializeField] float p2Gap              = 0.15f; // 줄지어 간격
    [SerializeField] float p2StartScale       = 0.25f; // 시작 배율
    [SerializeField] float p2EndScale         = 1.0f;  // 최종 배율 (Inspector에서 3~10 권장)
    [SerializeField] float p2GrowDuration     = 0.9f;  // 성장 완료까지 시간
    [SerializeField] float p2AccelPerSecond   = 14f;   // 초당 가속량
    [SerializeField] float p2InitialSpeedRate = 0.45f; // 초기 속도 배율 (projSpeed * 이 값)

    protected override void FireOnce()
    {
        if (!firePoint || !projectilePrefab) return;

        switch (pattern)
        {
            // Phase1: 직선 줄지어 발사 (volley 반영)
            case BossPatternType.Straight:
                StartCoroutine(FireStraightSeq(Mathf.Max(1, volley), p1Gap));
                break;

            // Phase2: 성장 + 가속 탄
            case BossPatternType.Mixed:
                StartCoroutine(FireGrowAccelSeq(Mathf.Max(1, volley), p2Gap));
                break;

            default:
                base.FireOnce();
                break;
        }
    }

    IEnumerator FireGrowAccelSeq(int count, float gap)
    {
        Vector3 dir = GetDirToPlayer3D();
        if (dir == Vector3.zero) yield break;

        for (int i = 0; i < count; i++)
        {
            SpawnBulletGrowAccel(dir, p2StartScale, p2EndScale, p2GrowDuration, p2AccelPerSecond, p2InitialSpeedRate);
            if (i < count - 1) yield return new WaitForSeconds(gap);
        }
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
        rb.linearVelocity = dir * (projSpeed * Mathf.Clamp01(initialSpeedRate)); // 반드시 velocity 사용

        // 3) 성장/가속 컴포넌트
        var grow = go.GetComponent<BulletGrowAndAccelerate>() ?? go.AddComponent<BulletGrowAndAccelerate>();
        grow.Configure(startScale, endScale, growDuration, accelPerSec, 6f);
        grow.BeginGrow(); // 성장 코루틴 시작
    }
}
