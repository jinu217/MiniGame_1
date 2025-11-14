using UnityEngine;
using System.Collections;

public class Boss_Stage5 : BossBase
{
    [Header("Common (Grow+Accel)")]
    [SerializeField] float growDuration     = 0.9f;   // 성장/축소에 걸리는 시간
    [SerializeField] float accelPerSecond   = 14f;    // 가속
    [SerializeField] float initialSpeedRate = 0.45f;  // projSpeed * rate
    [SerializeField] float bulletLife       = 6f;     // 탄 생존 시간

    // =======================
    // Phase1: 직선 패턴
    // =======================
    [Header("Phase1 (Straight Pattern)")]
    [SerializeField] int   p1VolleyCount   = 4;     // 4연발
    [SerializeField] float p1InnerShotGap  = 0.08f; // 4발 사이 간격(순차 발사)
    [SerializeField] float p1SegmentGap    = 0.4f;  // 작→큰→작→큰 사이 간격
    [SerializeField] float p1LoopDelay     = 1.0f;  // 한 사이클 끝나고 다시 시작까지 딜레이

    // 작아지는 탄(시작은 큼 → 점점 작아짐)
    [SerializeField] float p1ShrinkStartScale = 1.2f;
    [SerializeField] float p1ShrinkEndScale   = 0.3f;

    // 커지는 탄(시작은 작음 → 점점 커짐)
    [SerializeField] float p1GrowStartScale   = 0.3f;
    [SerializeField] float p1GrowEndScale     = 1.2f;

    // =======================
    // Phase2: 부채꼴 패턴
    // =======================
    [Header("Phase2 (Fan Pattern)")]
    [SerializeField] int   fanCount        = 4;     // 부채꼴 안에서 동시에 발사되는 탄 수
    [SerializeField] float fanTotalAngle   = 30f;   // 부채꼴 총각도
    [SerializeField] int   fanRepeats      = 2;     // “부채꼴 2연발”
    [SerializeField] float fanInnerGap     = 0.12f; // 같은 크기에서 2연발 사이 간격

    [SerializeField] float p2SegmentGap    = 0.4f;  // 작 부채꼴 ↔ 큰 부채꼴 사이 간격
    [SerializeField] float p2LoopDelay     = 1.0f;  // 한 사이클 끝난 후 딜레이

    // 작아지는 부채꼴
    [SerializeField] float p2ShrinkStartScale = 1.2f;
    [SerializeField] float p2ShrinkEndScale   = 0.3f;

    // 커지는 부채꼴
    [SerializeField] float p2GrowStartScale   = 0.3f;
    [SerializeField] float p2GrowEndScale     = 1.2f;

    [Header("Phase2 Transition")]
    [SerializeField] float phase2StartDelay = 2f;  // ✅ 1페 마지막 탄 이후 2페 시작까지 딜레이

    Coroutine phase1Routine;
    Coroutine phase2Routine;
    Coroutine phase2StarterRoutine; // ✅ 2페이즈 시작 지연용

    protected override void FireOnce()
    {
        if (!firePoint || !projectilePrefab) return;

        switch (pattern)
        {
            // 1페이즈: 직선(작/큰 조합) 패턴
            case BossPatternType.Straight:
                // 혹시 2페이즈 관련 코루틴 돌고 있으면 정지
                if (phase2StarterRoutine != null)
                {
                    StopCoroutine(phase2StarterRoutine);
                    phase2StarterRoutine = null;
                }
                if (phase2Routine != null)
                {
                    StopCoroutine(phase2Routine);
                    phase2Routine = null;
                }

                if (phase1Routine == null)
                    phase1Routine = StartCoroutine(Phase1Loop());
                break;

            // 2페이즈: 부채꼴(작/큰) 패턴
            case BossPatternType.Mixed:
                // ✅ 바로 Phase2Loop 시작하지 말고, 2초 대기 후 시작
                if (phase2StarterRoutine == null)
                    phase2StarterRoutine = StartCoroutine(StartPhase2AfterDelay(phase2StartDelay));
                break;

            default:
                base.FireOnce();
                break;
        }
    }

    // ✅ 2페이즈로 전환할 때: 1페이즈 멈추고, delay만큼 기다렸다가 Phase2Loop 시작
    IEnumerator StartPhase2AfterDelay(float delaySeconds)
    {
        // 1페이즈 루프 정지
        if (phase1Routine != null)
        {
            StopCoroutine(phase1Routine);
            phase1Routine = null;
        }

        // 딜레이 (1페 마지막 탄 쏜 뒤 쉬는 구간)
        if (delaySeconds > 0f)
            yield return new WaitForSeconds(delaySeconds);

        // 그 사이에 패턴이 다시 바뀌어버렸다면(예: 죽거나 다른 페이즈) 그냥 종료
        if (pattern != BossPatternType.Mixed)
        {
            phase2StarterRoutine = null;
            yield break;
        }

        // 2페이즈 루프 시작
        if (phase2Routine == null)
            phase2Routine = StartCoroutine(Phase2Loop());

        phase2StarterRoutine = null;
    }

    // =======================
    // Phase1: 직선 4연발 패턴 루프
    // =======================
    IEnumerator Phase1Loop()
    {
        while (true)
        {
            // 1) 작아지는 직선 4연발
            yield return StartCoroutine(
                FireStraightBurstGrow(p1VolleyCount, p1InnerShotGap, p1ShrinkStartScale, p1ShrinkEndScale)
            );
            yield return new WaitForSeconds(p1SegmentGap);

            // 2) 커지는 직선 4연발
            yield return StartCoroutine(
                FireStraightBurstGrow(p1VolleyCount, p1InnerShotGap, p1GrowStartScale, p1GrowEndScale)
            );
            yield return new WaitForSeconds(p1SegmentGap);

            // 3) 작아지는 직선 4연발
            yield return StartCoroutine(
                FireStraightBurstGrow(p1VolleyCount, p1InnerShotGap, p1ShrinkStartScale, p1ShrinkEndScale)
            );
            yield return new WaitForSeconds(p1SegmentGap);

            // 4) 커지는 직선 4연발
            yield return StartCoroutine(
                FireStraightBurstGrow(p1VolleyCount, p1InnerShotGap, p1GrowStartScale, p1GrowEndScale)
            );

            // 한 사이클 끝 → 1초 쉬고 다시 위로
            yield return new WaitForSeconds(p1LoopDelay);
        }
    }

    // 직선 방향으로 순차 4연발(혹은 N연발) + Grow/축소
    IEnumerator FireStraightBurstGrow(int count, float innerGap, float startScale, float endScale)
    {
        Vector3 dir = GetDirToPlayer3D();
        if (dir == Vector3.zero) yield break;

        count = Mathf.Max(1, count);
        for (int i = 0; i < count; i++)
        {
            SpawnBulletGrowAccel(dir, startScale, endScale);
            if (i < count - 1 && innerGap > 0f)
                yield return new WaitForSeconds(innerGap);
        }
    }

    // =======================
    // Phase2: 부채꼴 패턴 루프
    // =======================
    IEnumerator Phase2Loop()
    {
        while (true)
        {
            // 1) 작아지는 부채꼴 2연발
            yield return StartCoroutine(
                FireFanBurstGrow(fanRepeats, fanInnerGap, p2ShrinkStartScale, p2ShrinkEndScale)
            );
            yield return new WaitForSeconds(p2SegmentGap);

            // 2) 커지는 부채꼴 2연발
            yield return StartCoroutine(
                FireFanBurstGrow(fanRepeats, fanInnerGap, p2GrowStartScale, p2GrowEndScale)
            );

            // 한 사이클 끝 → 1초 쉬고 반복
            yield return new WaitForSeconds(p2LoopDelay);
        }
    }

    // 부채꼴 1회 발사(동시 fanCount발 × repeats회)
    IEnumerator FireFanBurstGrow(int repeats, float innerGap, float startScale, float endScale)
    {
        repeats = Mathf.Max(1, repeats);

        for (int r = 0; r < repeats; r++)
        {
            FireFanOnceGrow(fanCount, fanTotalAngle, startScale, endScale);

            if (r < repeats - 1 && innerGap > 0f)
                yield return new WaitForSeconds(innerGap);
        }
    }

    void FireFanOnceGrow(int count, float totalAngle, float startScale, float endScale)
    {
        Vector3 forward = GetDirToPlayer3D();
        if (forward == Vector3.zero) return;

        count = Mathf.Max(1, count);

        Quaternion baseRot = Quaternion.LookRotation(forward);
        float step  = count > 1 ? totalAngle / (count - 1) : 0f;
        float start = -totalAngle * 0.5f;

        for (int i = 0; i < count; i++)
        {
            float yaw = start + step * i;
            Quaternion rot = Quaternion.Euler(0f, yaw, 0f) * baseRot;
            Vector3 dir = rot * Vector3.forward;
            SpawnBulletGrowAccel(dir, startScale, endScale);
        }
    }

    // 공통: Grow/축소 + 가속 붙은 탄 생성
    void SpawnBulletGrowAccel(Vector3 dir, float startScale, float endScale)
    {
        var go = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(dir));

        var rb = go.GetComponent<Rigidbody>() ?? go.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.linearVelocity = dir * (projSpeed * Mathf.Clamp01(initialSpeedRate));

        var grow = go.GetComponent<BulletGrowAndAccelerate>() ?? go.AddComponent<BulletGrowAndAccelerate>();
        grow.Configure(startScale, endScale, growDuration, accelPerSecond, bulletLife);
        grow.BeginGrow();
    }

    void OnDisable()
    {
        if (phase1Routine != null)
        {
            StopCoroutine(phase1Routine);
            phase1Routine = null;
        }
        if (phase2Routine != null)
        {
            StopCoroutine(phase2Routine);
            phase2Routine = null;
        }
        if (phase2StarterRoutine != null)
        {
            StopCoroutine(phase2StarterRoutine);
            phase2StarterRoutine = null;
        }
    }
}
