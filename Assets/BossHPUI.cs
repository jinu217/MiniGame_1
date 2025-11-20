using UnityEngine;
using UnityEngine.UI;

public class BossHPUI : MonoBehaviour
{
    public Image bossHpBarFill;
    public Text bossHpText;  // UnityEngine.UI.Text

    void Update()
    {
        float maxHp = BossManager.Instance.config.maxHP;
        float currentHp = BossManager.Instance.CurrentBoss.CurrentHP;

        if (maxHp > 0)
            bossHpBarFill.fillAmount = currentHp / maxHp;

        bossHpText.text = currentHp + " / " + maxHp;
    }
}