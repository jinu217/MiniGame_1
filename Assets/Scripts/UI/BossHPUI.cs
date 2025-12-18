using UnityEngine;
using UnityEngine.UI;

public class BossHPUI : MonoBehaviour
{
    public Image bossHpBarFill;
    public Text bossHpText;

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
