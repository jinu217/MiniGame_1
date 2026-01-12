using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro 네임스페이스 추가

public class BossHPUI : MonoBehaviour
{
    public Image bossHpBarFill;
    public TextMeshProUGUI bossHpText; // 타입 변경

    void Update()
    {
        if (BossManager.Instance == null)
            return;

        if (BossManager.Instance.CurrentBoss == null)
        {
            bossHpBarFill.fillAmount = 0f;
            bossHpText.text = "";
            return;
        }

        float maxHp = BossManager.Instance.config.maxHP;
        float currentHp = BossManager.Instance.CurrentBoss.CurrentHP;

        if (maxHp > 0)
            bossHpBarFill.fillAmount = currentHp / maxHp;

        bossHpText.text = $"{currentHp} / {maxHp}";
    }
}