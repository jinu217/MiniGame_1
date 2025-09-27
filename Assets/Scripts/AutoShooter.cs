using UnityEngine;

public class AutoShooter : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;   // �Ѿ� ������
    public Transform firePoint;       // �Ѿ��� ���� ��ġ
    public float bulletSpeed = 15f;   // �Ѿ� �ӵ�
    public float fireRate = 0.25f;    // �߻� ����(��)

    float _nextFireTime;

    void Update()
    {
        // ���� �ð����� �Ѿ� �߻�
        if (Time.time >= _nextFireTime)
        {
            Fire();
            _nextFireTime = Time.time + fireRate;
        }
    }

    void Fire()
    {
        // �Ѿ� ����
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // ������ٵ� �� �ֱ�
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = firePoint.forward * bulletSpeed;
        }

        // �浹 ó�� ��ũ��Ʈ �������� �߰�
        if (bullet.GetComponent<BulletCollision>() == null)
        {
            bullet.AddComponent<BulletCollision>();
        }

        // 5�� �� �ڵ� �ı� (Ȥ�� �浹 ������ ��츦 ���)
        Destroy(bullet, 5f);
    }
}


// === �Ѿ� �浹 ó���� ���� Ŭ���� ===
public class BulletCollision : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Destroy(collision.gameObject); // ��ֹ� �ı�
            Destroy(gameObject);           // �Ѿ� �ı�
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Destroy(other.gameObject);     // ��ֹ� �ı�
            Destroy(gameObject);           // �Ѿ� �ı�
        }
    }
}

