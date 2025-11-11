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
    [SerializeField] protected float projSpeed = 15f;
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

    public void TakeDamage(int dmg)
    {
        if (IsDead) return;
        hp -= Mathf.Max(1, dmg);
        if (hp <= 0) Die();
    }

    protected virtual void Die()
    {
        // TODO: 연출/보상
        Destroy(gameObject);
    }

    public virtual void SetPattern(BossPatternType type, float interval, float speed, int volleyCount)
    {
        pattern = type;
        if (interval > 0f) fireInterval = interval;
        if (speed    > 0f) projSpeed    = speed;
        if (volleyCount >= 1) volley    = volleyCount;

        if (fireRoutine == null && gameObject.activeInHierarchy)
            fireRoutine = StartCoroutine(FireLoop());
    }

    IEnumerator FireLoop()
    {
        float t = 0f;
        while (!IsDead)
        {
            t += Time.deltaTime;
            if (t >= fireInterval)
            {
                t = 0f;
                FireOnce();
            }
            yield return null;
        }
    }

    protected virtual void FireOnce()
    {
        if (firePoint == null || projectilePrefab == null) return;

        Vector3 dir = GetDirToPlayer3D();
        if (dir == Vector3.zero) return;

        int count = Mathf.Max(1, volley);
        for (int i = 0; i < count; i++)
            SpawnBullet(dir);
    }

    protected void SpawnBullet(Vector3 dir)
    {
        var go = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(dir));
        var rb = go.GetComponent<Rigidbody>() ?? go.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.linearVelocity = dir * projSpeed;
    }

    protected Vector3 GetDirToPlayer3D()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (!player || !firePoint) return Vector3.zero;
        return (player.transform.position - firePoint.position).normalized;
    }

    protected void FireStraightSimul(int count)
    {
        Vector3 dir = GetDirToPlayer3D();
        if (dir == Vector3.zero) return;

        count = Mathf.Max(1, count);
        for (int i = 0; i < count; i++) SpawnBullet(dir);
    }

    protected IEnumerator FireStraightSeq(int count, float gap)
    {
        Vector3 dir = GetDirToPlayer3D();
        if (dir == Vector3.zero) yield break;

        count = Mathf.Max(1, count);
        for (int i = 0; i < count; i++)
        {
            SpawnBullet(dir);
            if (i < count - 1) yield return new WaitForSeconds(gap);
        }
    }
}
