using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [Header("Fallback (GM 없을 때만 사용)")]
    [SerializeField] int fallbackDamage = 1;

    void OnTriggerEnter(Collider other)
    {
        var boss = other.GetComponentInParent<BossBase>();
        if (boss != null)
        {
            int dmg = fallbackDamage;
            if (GameManager.gameManager != null)
                dmg = GameManager.gameManager.CurrentPlayerDamage;

            boss.TakeDamage(dmg);
            Destroy(gameObject);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("BossBullet"))
        {
            Destroy(other.gameObject);   // 보스탄 제거
            Destroy(gameObject);         // 내 탄도 제거
        }
    }
}
