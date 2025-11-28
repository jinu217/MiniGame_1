using UnityEngine;

public class AutoShooter : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;   // 발사할 총알 프리팹
    public Transform firePoint;       // 기본 총구 위치
    public Transform[] firePoints;    // 스프레드 등 다중 발사 지원
    public float bulletSpeed = 15f;   // 총알 속도
    public float fireRate = 0.5f;    // 발사 간격(초)

    [Header("Sound Settings")]
    public AudioClip PlayerShootSound; // 총소리 클립
    public AudioClip SpreadShootSound; // 스프레드 모드 총소리 클립
    public AudioSource audioSource; // 재생용 AudioSource

    [HideInInspector]
    public bool isSpreadMode = false;

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

    /// </test>
    
    void Fire()
    {
        // firePoints가 비어 있으면 기본 firePoint만 사용
        if (firePoints == null || firePoints.Length == 0)
            firePoints = new Transform[] { firePoint };

        foreach (var point in firePoints)
        {
            if (!point) continue;

            // 각 FirePoint를 기준으로 총알 생성
            GameObject bullet = Instantiate(bulletPrefab, point.position, point.rotation);
            bullet.tag = "PlayerBullet";

            // Rigidbody 보장 + 설정
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb == null) rb = bullet.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            // 각 FirePoint의 방향으로 발사
            rb.linearVelocity = point.forward * bulletSpeed;

            // Collider(Trigger)
            Collider col = bullet.GetComponent<Collider>();
            if (col == null) col = bullet.AddComponent<SphereCollider>();
            col.isTrigger = true;

            //총알 충돌 처리 스크립트 추가
            if (bullet.GetComponent<BulletCollision>() == null)
                bullet.AddComponent<BulletCollision>();

            // 자동 제거
            Destroy(bullet, 5f); 
        }
        //플레이어 총소리
        if (audioSource != null)
        {
            if (isSpreadMode && SpreadShootSound != null)
                audioSource.PlayOneShot(SpreadShootSound, 1f);     //스프레드 모드 소리, 볼륨 값
            else if (PlayerShootSound != null)
                audioSource.PlayOneShot(PlayerShootSound, 1f);     // 기본 소리, 볼륨 값
        }

    }

    public class BulletCollision : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            // "Obstacle" 태그를 가진 오브젝트에 닿으면 총알 삭제
            if (other.CompareTag("Obstacle"))
            {
                Destroy(gameObject);       // 총알 삭제
            }
        }
    }
}