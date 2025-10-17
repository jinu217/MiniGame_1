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
                StartCoroutine(FireThreeInSequence());
                break;

            case BossPatternType.DiagonalRandom:
                FireDiagonalRandom();
                break;

            default:
                base.FireOnce();
                break;
        }
    }

    IEnumerator FireThreeInSequence()
    {
        Vector3 dir = GetDirToPlayer3D();
        if (dir == Vector3.zero) yield break;

        // 3발 순차 발사
        for (int i = 0; i < 3; i++)
        {
            SpawnBullet(dir);
            yield return new WaitForSeconds(0.3f);
        }
    }

    void FireDiagonalRandom()
    {
        for (int i = 0; i < volley; i++)
        {
            var dir = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-0.3f, 0.3f),   // 🎯 Y축도 랜덤 살짝 포함
                Random.Range(-0.7f, -0.3f)
            ).normalized;

            SpawnBullet(dir);
        }
    }

    // 🔥 X, Y, Z 포함한 3D 정조준
    Vector3 GetDirToPlayer3D()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (!player) return Vector3.zero;

        Vector3 dir = player.transform.position - firePoint.position;
        return dir.normalized;
    }
}
