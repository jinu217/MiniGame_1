using System.Collections;
using Unity.Android.Types;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float spawnPosX;
    public float spawnPosY = 0.5f;
    public float spawnPosZ = 25f;
    public float bugSpawnCycle = 1f;

    public float healKitSpawnCycle = 2f;

    public GameObject bugObject;
    public GameObject healKit;

    private void Start()
    {
        StartCoroutine(BugSpawnRoutine());
        StartCoroutine(HealKitSpawnRoutine());
    }
    public void BugSpawn()
    {
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

        GameObject bug = Instantiate(bugObject, clonePos, transform.rotation);
        bug.name = bugObject.name;

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
    public void HealKitSpawn()
    {
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

        GameObject HealKit = Instantiate(healKit, clonePos, transform.rotation);
        HealKit.name = healKit.name;

        // 복제된 오브젝트 Rigid, Collider, Renderer 초기화
        Rigidbody cloneRb = healKit.GetComponent<Rigidbody>();
        if (cloneRb)
        {
            cloneRb.isKinematic = false;
        }

        Collider cloneCol = healKit.GetComponent<Collider>();
        if (cloneCol) cloneCol.enabled = true;

        Renderer[] cloneRends = healKit.GetComponentsInChildren<Renderer>(true);
        foreach (var r in cloneRends)
        {
            if (r) r.enabled = true;
        }

        //isSpawn = false;
    }
    IEnumerator BugSpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(bugSpawnCycle);
            BugSpawn();
        }
    }

    IEnumerator HealKitSpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(healKitSpawnCycle);
            HealKitSpawn();
            
        }
    }
}
