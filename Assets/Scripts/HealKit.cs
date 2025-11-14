using System.Collections;
using Unity.Android.Types;
using UnityEngine;
public class HealKit : MonoBehaviour
{
    public float healKitSpeed = 10f;
    public int healValue = 1;


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
            rd.linearVelocity = Vector3.back * healKitSpeed; // Z- 방향 전진
        }

    }


    void OnTriggerEnter(Collider other)
    {
        if (isHiding) return;


        if (other.CompareTag("PlayerBullet"))
        {
            Destroy(gameObject);
            GameManager.gameManager.playerHp += healValue;
        }

        if (other.CompareTag("ArrivePoint") || other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

}
