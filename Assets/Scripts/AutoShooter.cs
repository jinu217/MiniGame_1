using UnityEngine;

public class AutoShooter : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;   // 발사할 총알 프리팹
    public Transform firePoint;       // 기본 총구 위치
    public Transform[] firePoints;    // 스프레드 등 다중 발사 지원
    public float bulletSpeed = 15f;   // 총알 속도
    public float fireRate = 0.25f;    // 발사 간격(초)

    float _nextFireTime;

    void Update()
    {
        // 자동 발사 (항상 쏘는 구조)
        if (Time.time >= _nextFireTime)
        {
            Fire();
            _nextFireTime = Time.time + fireRate;
        }
    }

    void Fire()
    {
        // firePoints가 비어 있으면 기본 firePoint만 사용
        if (firePoints == null || firePoints.Length == 0)
            firePoints = new Transform[] { firePoint };

        foreach (var point in firePoints)
        {
            if (!point) continue;

            // ✅ 각 FirePoint를 기준으로 총알 생성
            GameObject bullet = Instantiate(bulletPrefab, point.position, point.rotation);
            bullet.tag = "PlayerBullet";

            // Rigidbody 보장 + 설정
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb == null) rb = bullet.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            // ✅ 각 FirePoint의 방향으로 발사
            rb.linearVelocity = point.forward * bulletSpeed;

            // Collider(Trigger)
            Collider col = bullet.GetComponent<Collider>();
            if (col == null) col = bullet.AddComponent<SphereCollider>();
            col.isTrigger = true;

            // 자동 제거
            Destroy(bullet, 5f);
        }
    }
}
