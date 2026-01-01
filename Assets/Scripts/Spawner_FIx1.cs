using System.Collections;
using System.Collections.Generic;
using Unity.Android.Types;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float spawnPosX;
    public float spawnPosY = 0.5f;
    public float spawnPosZ = 50f;

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

        // 원래 태그 저장
        string originalTag = obj.tag;

        // 렌더러만 끄기 (물리는 유지)
        foreach (var r in obj.GetComponentsInChildren<Renderer>())
            r.enabled = false;

        // 태그 변경
        obj.tag = tempTag;

        yield return new WaitForSeconds(seconds);

        if (obj == null) yield break;

        // 복구
        foreach (var r in obj.GetComponentsInChildren<Renderer>())
            r.enabled = true;

        obj.tag = originalTag;
    }

    public GameObject BugSpawn()
    {

        float cloneX = Random.Range(-2f, 2f);

        Vector3 clonePos = new Vector3(cloneX, spawnPosY, spawnPosZ);
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
        float cloneX = Random.Range(-2f, 2f); ;
        var allBug = FindObjectsByType<BugObject>(FindObjectsSortMode.None); //    BugObject ã  

        Vector3 clonePos = new Vector3(cloneX, spawnPosY, spawnPosZ);

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
