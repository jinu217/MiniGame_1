using System.Collections;
using UnityEngine;

public class BugObject : MonoBehaviour
{
    public float bugSpeed = 10f;

    //public int killCount = 0;
    public float bugHp;
    public bool isArrive;
    public bool isBugLarge;
    public void Arrive() => isArrive = true;

    bool isHiding;
    public bool isDistroy = false;

    public Spawner spawner;

    public float PlayerTakeDmgSoundVolume = 1.7f;

    Rigidbody rd;
    Collider col;
    Renderer[] rends;

    void Awake()
    {
        bugHp = GameManager.gameManager.bugHp;
        rd = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        rends = GetComponentsInChildren<Renderer>(true);
        if (rd != null) rd.isKinematic = false;
        //transform.rotation = Quaternion.Euler(-90f, 180f, 0f);
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

        if (other.CompareTag("ArrivePoint") || other.CompareTag("Player"))
        {
            GameManager.gameManager.PlaySoundAtPlayer(GameManager.gameManager.PlayerTakeDmg, PlayerTakeDmgSoundVolume);

            GameManager.gameManager.playerHp -= GameManager.gameManager.bugDamage;
            Debug.Log("버그가 도착하여 플레이어에게 " + GameManager.gameManager.bugDamage + "만큼 데미지를 입혔습니다!");
            Destroy(gameObject);
        }
    }

    public void BugLarge(bool large)
    {
        isBugLarge = large;

        if (large)
            bugHp = GameManager.gameManager.bugHp * 2f;
        else
            bugHp = GameManager.gameManager.bugHp;
    }

    public void TakeDamage(int dmg)
    {
        bugHp -= dmg;
        //Debug.Log(dmg + "의 데미지!");
        if (bugHp <= 0)
            Destroy(gameObject);
    }
}
