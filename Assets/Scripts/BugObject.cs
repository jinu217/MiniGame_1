using System.Collections;
using UnityEngine;

public class BugObject : MonoBehaviour
{
    public float spawnPosX;
    public float spawnPosY = 0.5f;
    public float spawnPosZ = 0f;

    public float bugSpeed = 2f;
    public int bugDamage = 2;

    public Player player;
    public AutoShooter autoShooter;

    public bool isArrive;
    public void Arrive() => isArrive = true;

    bool isHiding;

    Rigidbody rd;
    Collider col;
    Renderer[] rends;

    void Awake()
    {
        rd = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        rends = GetComponentsInChildren<Renderer>(true);
        if (rd != null) rd.isKinematic = false;
    }

    void FixedUpdate()
    {
        if (rd != null && !isHiding)
        {
            rd.linearVelocity = Vector3.back * bugSpeed; // Z- 방향 전진
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isHiding) return;

        var hitRoot = collision.collider.transform.root;

        if (hitRoot.CompareTag("PlayerBullet"))
        {
            Hide(transform.position);
            var rb = collision.rigidbody;
            Destroy(rb ? rb.gameObject : collision.collider.gameObject);
            return;
        }

        if (collision.collider.CompareTag("ArrivePoint") || collision.collider.CompareTag("Player"))
        {
            Hide(transform.position);
            player.playerHp -= bugDamage;
            Debug.Log(player.playerHp);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isHiding) return;

        var hitRoot = other.transform.root;

        if (hitRoot.CompareTag("PlayerBullet"))
        {
            Hide(transform.position);
            var rb = other.attachedRigidbody;
            Destroy(rb ? rb.gameObject : other.gameObject);
            return;
        }

        if (other.CompareTag("ArrivePoint") || other.CompareTag("Player"))
        {
            Hide(transform.position);
            player.playerHp -= bugDamage;
            Debug.Log(player.playerHp);
        }
    }

    public void Hide(Vector3 spawnPos)
    {
        if (isHiding) return;
        isHiding = true;
        isArrive = true;

        if (rd != null)
        {
            rd.linearVelocity = Vector3.zero;
            rd.angularVelocity = Vector3.zero;
            rd.isKinematic = true;
        }
        if (col) col.enabled = false;

        if (rends != null)
        {
            foreach (var r in rends) if (r) r.enabled = false;
        }

        StartCoroutine(HideRoutine(spawnPos));
    }

    IEnumerator HideRoutine(Vector3 spawnPos)
    {
        int frameCount = 0;
        while (frameCount < 20)
        {
            spawnPosX = Random.Range(-2f, 2f);
            frameCount++;
            yield return null;
        }

        transform.position = new Vector3(spawnPosX, spawnPosY, spawnPosZ);

        if (rends != null)
        {
            foreach (var r in rends) if (r) r.enabled = true;
        }
        if (col) col.enabled = true;

        if (rd != null) rd.isKinematic = false;

        isHiding = false;
    }
}
