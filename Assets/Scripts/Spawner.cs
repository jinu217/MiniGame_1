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

    // colider 범위 연산
    float GetHalfWidthX(GameObject prefab)
    {
        // BoxCollider가 있으면
        BoxCollider box = prefab.GetComponentInChildren<BoxCollider>();
        if (box != null)
        {
            // 로컬 size × 실제 스케일
            float scaleX = box.transform.lossyScale.x;
            return (box.size.x * scaleX) * 0.5f;
        }

        // CapsuleCollider가 있으면
        CapsuleCollider cap = prefab.GetComponentInChildren<CapsuleCollider>();
        if (cap != null)
        {
            float scaleX = cap.transform.lossyScale.x;
            return cap.radius * scaleX;
        }

        // Collider가 없을 경우 fallback
        return 0.25f;
    }

    float safeMinDistX(GameObject a, GameObject b, float margin = 0.05f)
    {
        float halfA = GetHalfWidthX(a);
        float halfB = GetHalfWidthX(b);
        return halfA + halfB + margin;
    }

    public void BugSpawn()
    {
        //isSpawn = true;
        int attemptCount = 0;

        float cloneX;
        var allHK = FindObjectsByType<HealKit>(FindObjectsSortMode.None); //    BugObject ã  

        float minDistX = safeMinDistX(bugObject, healKit); // colider 안전거리

        do
        {
            attemptCount++;
            cloneX = Random.Range(-2f, 2f);

            bool overlap = false;

            foreach (var otherHK in allHK)
            {
                if (otherHK == this) continue; //  ڱ   ڽ      

                float otherX = otherHK.transform.position.x;

                if (Mathf.Abs(cloneX - otherX) < minDistX) // 0.5f  ̳      ħ
                {
                    overlap = true;
                    break;
                }

            }
            if (!overlap) break;
        }
        while (attemptCount < 30);

        Vector3 clonePos = new Vector3(cloneX, spawnPosY, spawnPosZ);
        GameObject bug = Instantiate(bugObject, clonePos, transform.rotation);
        bug.name = bugObject.name;

        //              Ʈ Rigid, Collider, Renderer  ʱ ȭ
        Rigidbody cloneRb = bug.GetComponent<Rigidbody>();
        if (cloneRb)
        {
            cloneRb.isKinematic = false;
        }


        //isSpawn = false;
    }
    public void HealKitSpawn()
    {

        // isSpawn = true;
        int attemptCount = 0;

        float cloneX;
        var allBug = FindObjectsByType<BugObject>(FindObjectsSortMode.None); //    BugObject ã  

        float minDistX = safeMinDistX(bugObject, healKit); // colider 안전거리

        do
        {
            attemptCount++;
            cloneX = Random.Range(-2f, 2f);

            bool overlap = false;

            foreach (var otherBug in allBug)
            {
                if (otherBug == this) continue; //  ڱ   ڽ      

                float otherX = otherBug.transform.position.x;

                if (Mathf.Abs(cloneX - otherX) < minDistX) // 0.5f  ̳      ħ
                {
                    overlap = true;
                    break;
                }

            }
            if (!overlap) break;
        }
        while (attemptCount < 30);

        Vector3 clonePos = new Vector3(cloneX, spawnPosY, spawnPosZ);

        GameObject hk = Instantiate(healKit, clonePos, transform.rotation);
        hk.name = healKit.name;

        //              Ʈ Rigid, Collider, Renderer  ʱ ȭ
        Rigidbody cloneRb = hk.GetComponent<Rigidbody>();
        if (cloneRb)
        {
            cloneRb.isKinematic = false;
        }

    }

    IEnumerator BugSpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(GameManager.gameManager.bugSpawnCycle);
            BugSpawn();
        }
    }

    IEnumerator HealKitSpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(GameManager.gameManager.healKitSpawnCycle);
            HealKitSpawn();

        }
    }

}
