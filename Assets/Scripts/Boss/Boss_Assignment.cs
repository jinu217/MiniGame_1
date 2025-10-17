using UnityEngine;
using System.Collections;

public class Boss_Assignment : BossBase
{
    protected override void FireOnce()
    {
        if (firePoint == null || projectilePrefab == null) return;

        switch (pattern)
        {
            case BossPatternType.PaperShot:
                // 플레이어 방향으로 volley개, 0.2초 간격 줄지어 발사
                StartCoroutine(FireStraightSeq(volley, 0.2f));
                break;

            case BossPatternType.DiagonalRandom:
                FireDiagonalRandom();
                break;

            default:
                // BossBase 기본 동작(동시 volley발)
                base.FireOnce();
                break;
        }
    }

    void FireDiagonalRandom()
    {
        int count = Mathf.Max(1, volley);
        for (int i = 0; i < count; i++)
        {
            var dir = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-0.3f, 0.3f),
                Random.Range(-0.7f, -0.3f)
            ).normalized;

            SpawnBullet(dir);
        }
    }
}
