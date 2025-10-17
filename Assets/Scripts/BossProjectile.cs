using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float lifeTime = 6f;      // 자동 파괴 시간
    [SerializeField] int damage = 1;           // 플레이어 피격 시 HP 감소량

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // 1️⃣ 플레이어 총알과 부딪힐 때 서로 제거
        if (other.CompareTag("PlayerBullet"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
            return;
        }

        // 2️⃣ 플레이어 본체와 충돌 (HP 감소)
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<Player>();
            if (player != null)
            {
                player.playerHp -= damage; // HP 감소
                if (player.playerHp <= 0)
                {
                    // TODO: GameOver 처리 (씬 전환, UI 등)
                    Debug.Log("Player Dead!");
                }
            }
            Destroy(gameObject); // 보스탄 제거
            return;
        }
    }
}
