using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // 레이어로 체크하면 태그 상관없이 정확
        if (other.gameObject.layer == LayerMask.NameToLayer("BossBullet"))
        {
            Destroy(other.gameObject);   // 보스탄 제거
            Destroy(gameObject);         // 내 탄도 제거
        }
    }
}
