using UnityEngine;

public class AutoShooter : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;   // 총알 프리팹
    public Transform firePoint;       // 총알이 나올 위치
    public float bulletSpeed = 15f;   // 총알 속도
    public float fireRate = 0.25f;    // 발사 간격(초)

    float _nextFireTime;

    void Update()
    {
        // 일정 시간마다 총알 발사
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

        // 리지드바디에 힘 주기
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = firePoint.forward * bulletSpeed;
        }

        // 충돌 처리 스크립트 동적으로 추가
        if (bullet.GetComponent<BulletCollision>() == null)
        {
            bullet.AddComponent<BulletCollision>();
        }

        // 5초 후 자동 파괴 (혹시 충돌 못했을 경우를 대비)
        Destroy(bullet, 5f);
    }
}


// === 총알 충돌 처리용 내부 클래스 ===
public class BulletCollision : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Destroy(collision.gameObject); // 장애물 파괴
            Destroy(gameObject);           // 총알 파괴
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Destroy(other.gameObject);     // 장애물 파괴
            Destroy(gameObject);           // 총알 파괴
        }
    }
}

