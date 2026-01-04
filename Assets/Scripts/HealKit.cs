using System.Collections;
using UnityEngine;
public class HealKit : MonoBehaviour
{
    public float healKitSpeed = 10f;

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
        transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        transform.localScale = new Vector3(1f, 1f, 1f);
    }


    void FixedUpdate()
    {
        if (rd != null && !isHiding)
        {
            rd.linearVelocity = Vector3.back * healKitSpeed; // Z- ���� ����
        }

    }


    void OnTriggerEnter(Collider other)
    {
        if (isHiding) return;

        if (other.CompareTag("ArrivePoint") || other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

}
