using System.Collections;
using Unity.Android.Types;
using UnityEngine;

public class BugObject : MonoBehaviour
{
    public float spawnPosX;
    public float spawnPosY = 0.5f;
    public float spawnPosZ = 25f;

    public float bugSpeed = 10f;
    public int bugDamage = 1;

    public int killCount = 0;


    public GameObject bugObject;
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


    void OnTriggerEnter(Collider other)
    {
        if (isHiding) return;


        if (other.CompareTag("PlayerBullet"))
        {
            Hide(transform.position);
            killCount++;

            Debug.Log(killCount);

            if (player != null)
            {
                player.skillPoint++;
                player.skillPoint = Mathf.Clamp(player.skillPoint, 0, player.maxSkillPoint);
            }

            if (killCount % 5 == 0) // (임시) 킬 카운트에 따른 버그 추가 생성
            {
                Spawn();
            }
        }

        if (other.CompareTag("ArrivePoint") || other.CompareTag("Player"))
        {
            Hide(transform.position);
            player.playerHp -= bugDamage;
            Debug.Log(player.playerHp);
        }
    }


    // 복제 매커니즘
    public void Spawn()
    {
        Debug.Log("생성");
        // isSpawn = true;
        int attemptCount = 0;

        float cloneX;
        var allBug = FindObjectsByType<BugObject>(FindObjectsSortMode.None); //모든 BugObject 찾기
        do
        {
            attemptCount++;
            cloneX = Random.Range(-2f, 2f);
            foreach (var otherBug in allBug)
            {
                if (otherBug == this) continue; // 자기 자신 제외

                float otherX = otherBug.transform.position.x;

                if (Mathf.Abs(cloneX - otherX) < 0.5f) // 0.5f 이내면 겹침
                {
                    break;
                }

            }
        }
        while (attemptCount < 30);

        Vector3 clonePos = new Vector3(cloneX, spawnPosY, spawnPosZ);

        GameObject bug = Instantiate(this.gameObject, clonePos, transform.rotation);

        // 복제된 오브젝트 Rigid, Collider, Renderer 초기화
        Rigidbody cloneRb = bug.GetComponent<Rigidbody>();
        if (cloneRb)
        {
            cloneRb.isKinematic = false;
        }

        Collider cloneCol = bug.GetComponent<Collider>();
        if (cloneCol) cloneCol.enabled = true;

        Renderer[] cloneRends = bug.GetComponentsInChildren<Renderer>(true);
        foreach (var r in cloneRends)
        {
            if (r) r.enabled = true;
        }

        //isSpawn = false;
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
