using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float bugX;
    public float healKitX;
    public float spawnPosY = 0.5f;
    public float spawnPosZ = 30f;

    public GameObject bugObject;
    public GameObject healKit;

    

    private void Start()
    {
        StartCoroutine(BugSpawnRoutine());
        StartCoroutine(HealKitSpawnRoutine());
    }

    IEnumerator HideForSeconds(GameObject obj, float seconds, string tempTag)
    {
        if (obj == null) yield break;

        Rigidbody rb = obj.GetComponent<Rigidbody>();

        // 원래 태그 저장
        string originalTag = obj.tag;

        // Z축 물리 이동 잠금
        if (rb != null)
            rb.constraints |= RigidbodyConstraints.FreezePositionZ;

        // 렌더러 끄기
        foreach (var r in obj.GetComponentsInChildren<Renderer>())
            r.enabled = false;

        // 태그 변경
        obj.tag = tempTag;

        float elapsed = 0f;
        while (elapsed < seconds)
        {
            if (obj == null) yield break;

            // 숨김 상태 동안 Z 강제 고정
            Vector3 pos = obj.transform.position;
            pos.z = spawnPosZ;
            obj.transform.position = pos;

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (obj == null) yield break;

        // Z축 물리 이동 해제
        if (rb != null)
            rb.constraints &= ~RigidbodyConstraints.FreezePositionZ;

        // 렌더러 복구
        foreach (var r in obj.GetComponentsInChildren<Renderer>())
            r.enabled = true;

        // 태그 복구
        obj.tag = originalTag;
    }

    public GameObject BugSpawn()
    {
        bugX = Random.Range(-2f, 2f);

        Vector3 clonePos = new Vector3(bugX, spawnPosY, spawnPosZ);
        GameObject bug = Instantiate(bugObject, clonePos, transform.rotation);
        bug.name = bugObject.name;

        //              Ʈ Rigid, Collider, Renderer  ʱ ȭ
        Rigidbody cloneRb = bug.GetComponent<Rigidbody>();
        if (cloneRb)
        {
            cloneRb.isKinematic = false;
        }

        return bug;
        //isSpawn = false;
    }
    public GameObject HealKitSpawn()
    {
        healKitX = Random.Range(-2f, 2f);

        while (Mathf.Abs(bugX - healKitX) < 0.5)
        {
            healKitX = Random.Range(-2f, 2f);
        }

        Vector3 clonePos = new Vector3(healKitX, spawnPosY, spawnPosZ);

        GameObject hk = Instantiate(healKit, clonePos, transform.rotation);
        hk.name = healKit.name;

        //              Ʈ Rigid, Collider, Renderer  ʱ ȭ
        Rigidbody cloneRb = hk.GetComponent<Rigidbody>();
        if (cloneRb)
        {
            cloneRb.isKinematic = false;
        }

        return hk;

    }

    IEnumerator BugSpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(GameManager.gameManager.bugSpawnCycle);
            GameObject bug = BugSpawn();
            StartCoroutine(HideForSeconds(bug, 0.5f, "temp"));
        }
    }

    IEnumerator HealKitSpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(GameManager.gameManager.healKitSpawnCycle);
            GameObject hk = HealKitSpawn();
            StartCoroutine(HideForSeconds(hk, 0.5f, "temp"));
        }
    }

}
