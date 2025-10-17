using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float lifeTime = 6f;
    [SerializeField] int damage = 1;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<Player>();
            if (player != null)
            {
                player.playerHp -= damage;
                if (player.playerHp <= 0)
                {
                    Debug.Log("Player Dead!");
                }
            }
            Destroy(gameObject);
            return;
        }
    }
}
