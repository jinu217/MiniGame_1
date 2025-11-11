using System.Collections;
using Unity.Android.Types;
using UnityEngine;

public class BugObject : MonoBehaviour
{
    public float bugSpeed = 10f;
    public int bugDamage = 1;
    
    public int killCount = 0;

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
            killCount++;
            Destroy(gameObject);
        }

        if (other.CompareTag("ArrivePoint") || other.CompareTag("Player"))
        {
            GameManager.gameManager.playerHp -= bugDamage;
            Destroy(gameObject);
        }
    }


}
