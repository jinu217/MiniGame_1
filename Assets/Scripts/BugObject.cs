using System.Collections;
using UnityEngine;

public class FallingObject : MonoBehaviour
{
    public float spawnPosX;
    public float spawnPosY = 0.5f;
    public float spawnPosZ = 0f;

    public float bugSpeed = 2f;
    public int bugDamage = 2;

    public Player player;


    public bool isArrive;
    public void Arrive() 
    {
        isArrive = true; 
    }

    bool isHiding;

    Rigidbody rd;
    Collider col;
    Renderer[] rends;  // 오브젝트 보이게, 숨기게

    void Awake()
    {
        rd = GetComponent<Rigidbody>();    
        col = GetComponent<Collider>();
        rends = GetComponentsInChildren<Renderer>(true);
        if (rd != null)
        {
            rd.isKinematic = false; // 물리 영향 비활성화
        }
    }

    void FixedUpdate() 
    {
        if (rd != null && !isHiding)
        {
            rd.linearVelocity = Vector3.forward * bugSpeed;  
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isHiding)
        {
            return;
        }
        

        if (collision.gameObject.CompareTag("ArrivePoint")) // 선에 닿았을 때
        {
            Hide(transform.position);
            player.playerHp = player.playerHp - bugDamage;
            Debug.Log(player.playerHp);
        }
    }

    public void Hide(Vector3 spawnPos) // 오브젝트 숨기기
    {
        if (isHiding) return;
        isHiding = true;
        isArrive = true;

   
        if (rd != null)
        {
            rd.isKinematic = true;
            rd.linearVelocity = Vector3.zero;
            rd.angularVelocity = Vector3.zero;
        }
        if (col)
        {
            col.enabled = false;
        }


        if (rends != null)
        {
            foreach (var r in rends)
            {
                if (r) r.enabled = false;
            }
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
        
        // 리스폰: X만 랜덤, Y,Z 고정
        transform.position = new Vector3(spawnPosX, spawnPosY, spawnPosZ);

        // 다시 보이기
        if (rends != null)
        {
            foreach (var r in rends)
            {
                if (r) r.enabled = true;
            }
                
        }
            
        if (col)
        {
            col.enabled = true;
        }

        if (rd != null)
        {
            rd.isKinematic = false;
        }
            

        isHiding = false;

    }
}
