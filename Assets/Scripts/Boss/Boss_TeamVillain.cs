using UnityEngine;
using System.Collections;
 
public class Boss_TeamVillain : BossBase
{
    [Header("Mixed Settings (20s 이후)")]
    [SerializeField] int   fanEvery        = 3;     // 몇 번마다 부채꼴
    [SerializeField] int   fanCount        = 5;     // 부채꼴 탄수
    [SerializeField] float fanTotalAngle   = 15f;   // 부채꼴 각도
    [SerializeField] float straightBurstGap= 0.1f; // 직선 연사 간격(2페이즈 기본)

    int mixedFireCounter = 0;

    protected override void FireOnce()
    {
        if (firePoint == null || projectilePrefab == null) return;

        switch (pattern)
        {
            case BossPatternType.Straight:
                FireStraightPhase();      // 0~20초
                break;

            case BossPatternType.Mixed:
                FireMixedPhase();         // 20초~
                break;

            default:
                base.FireOnce();
                break;
        }
    }

    // 0~20초: 플레이어 방향 줄지어 volley발(간격 0.1초)
    void FireStraightPhase()
    {
        StartCoroutine(FireStraightSeq(volley, 0.15f));
    }

    // 20초 이후: 기본 줄지어 volley발 + 주기적으로 부채꼴 5발
    void FireMixedPhase()
    {
        mixedFireCounter++;

        if (mixedFireCounter % fanEvery == 0)
        {
            FireFanAtPlayer(fanCount, fanTotalAngle);
            return;
        }

        StartCoroutine(FireStraightSeq(volley, straightBurstGap));
    }

    void FireFanAtPlayer(int count, float totalAngle)
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
            SpawnBullet(rot * Vector3.forward);
        }
    }
}
