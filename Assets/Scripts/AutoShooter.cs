using UnityEngine;

public class AutoShooter : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;   // 발사할 총알 프리팹
    public Transform firePoint;       // 총구 위치
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
        // 총알 생성
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.tag = "PlayerBullet";

        // Rigidbody 보장 + 설정
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb == null) rb = bullet.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.linearVelocity = firePoint.forward * bulletSpeed;  

        // 콜라이더(Trigger)
        Collider col = bullet.GetComponent<Collider>();
        if (col == null) col = bullet.AddComponent<SphereCollider>();
        col.isTrigger = true;

        // 자동 제거
        Destroy(bullet, 5f);
    }
}
