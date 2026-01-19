using Unity.VisualScripting;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [Header("Fallback (GM 없을 때만 사용)")]
    [SerializeField] int fallbackDamage = 1;

    public BugObject bugObject;
    public float BugHitSoundVolume = 1.7f;
    public float healSoundVolume = 1.0f;
    public float BossHitSoundVolume = 1.5f;



    void OnTriggerEnter(Collider other)
    {
        var boss = other.GetComponentInParent<BossBase>();
        if (boss != null)
        {
            GameManager.gameManager.PlaySoundAtPlayer(GameManager.gameManager.BossHitSound, BossHitSoundVolume);


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

        if (other.CompareTag("HealKit")) // 힐킷 상호작용
        {
            GameManager.gameManager.PlaySoundAtPlayer(GameManager.gameManager.healSound, healSoundVolume);

            Destroy(other.gameObject);
            Destroy(gameObject);
            GameManager.gameManager.playerHp += GameManager.gameManager.healValue;
        }

        if (other.CompareTag("Bug")) // 버그 상호작용
        {
            // 버그 죽일때 소리
            GameManager.gameManager.PlaySoundAtPlayer(GameManager.gameManager.BugHitSound, BugHitSoundVolume);

            var bug = other.GetComponent<BugObject>();
            if (bug != null)
            {
                int dmg = fallbackDamage;
                if (GameManager.gameManager != null)
                    dmg = GameManager.gameManager.CurrentPlayerDamage;
                bug.TakeDamage(dmg);
            }
            Destroy(gameObject);
        }
    }
}
