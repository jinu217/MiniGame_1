using UnityEngine;
using System.Collections;

public class BossBase : MonoBehaviour
{
    [Header("Refs")]
    public Transform firePoint;
    public GameObject projectilePrefab;

    [Header("HP")]
    [SerializeField] int hp;
    public bool IsDead => hp <= 0;

    protected BossPatternType pattern;

    [Header("Fire Settings")]
    [SerializeField] protected float fireInterval = 2f;
    [SerializeField] protected float projSpeed = 150f;
    [SerializeField] protected int volley = 1;

    Coroutine fireRoutine;

    public virtual void Init(int maxHP)
    {
        hp = maxHP;
        var rb = GetComponent<Rigidbody>();
        if (!rb)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
        }
    }

    public virtual void SetPattern(BossPatternType type, float interval, float speed, int volleyCount)
    {
        pattern = type;
        if (interval > 0) fireInterval = interval;
        if (speed > 0) projSpeed = speed;
        if (volleyCount > 0) volley = volleyCount;

        if (fireRoutine != null) StopCoroutine(fireRoutine);
        fireRoutine = StartCoroutine(FireLoop());
    }

    IEnumerator FireLoop()
    {
        while (!IsDead)
        {
            FireOnce();  // 🔹 실제 발사 로직
            yield return new WaitForSeconds(fireInterval);
        }

        Debug.Log($"[BossBase] FireLoop running - interval: {fireInterval}, speed: {projSpeed}");

    }

    // 🔸 패턴별 로직은 여기서 오버라이드할 수 있음
    protected virtual void FireOnce()
    {
        switch (pattern)
        {
            case BossPatternType.Straight:
                FireTowardPlayer();
                break;
            case BossPatternType.DiagonalRandom:
                FireDiagonal();
                break;
            case BossPatternType.Circle:
                FireCircle();
                break;
            case BossPatternType.Mixed:
                FireMixed();
                break;
        }
    }

    // 🔸 공통 발사 유틸리티 (자식도 재사용 가능)
    protected void FireTowardPlayer()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (!player) return;
        Vector3 dir = player.transform.position - firePoint.position;
        dir.y = 0;
        dir.Normalize();
        SpawnBullet(dir);
    }

    protected void FireDiagonal()
    {
        Vector3 dir = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, -0.5f)).normalized;
        SpawnBullet(dir);
    }

    protected void FireCircle()
    {
        int count = Mathf.Max(6, volley * 2);
        for (int i = 0; i < count; i++)
        {
            float a = 360f * i / count;
            Vector3 dir = new Vector3(Mathf.Sin(a * Mathf.Deg2Rad), 0f, Mathf.Cos(a * Mathf.Deg2Rad));
            SpawnBullet(dir);
        }
    }

    protected void FireMixed()
    {
        FireTowardPlayer();
        FireDiagonal();
    }

    protected void SpawnBullet(Vector3 dir)
    {
        var go = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        var rb = go.GetComponent<Rigidbody>() ?? go.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.linearVelocity = dir * projSpeed;
        Destroy(go, 10f);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            hp--;
            Destroy(other.gameObject);
            if (IsDead)
            {
                StopAllCoroutines();
                Destroy(gameObject);
            }
        }
    }
}
