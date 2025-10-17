using UnityEngine;
using UnityEngine.UI; // UI ���ۿ�
using TMPro; // TextMeshPro ��� �� �ʿ�

public class HPMeasure : MonoBehaviour
{
    public Image hpFillImage;   // HP�� Fill �̹���
    public TMP_Text hpText;     // HP ��ġ ǥ�ÿ� Text (TextMeshPro)

    public float maxHP = 100f;
    private float currentHP;

    void Start()
    {
        currentHP = maxHP; // ���� �� ü���� ���� ä��
        UpdateHPBar();
    }

    void Update()
    {
        // �׽�Ʈ: �����̽��� ������ ü�� 10 ����
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(10);
        }
    }

    // ü�� ���� ó��
    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;
        UpdateHPBar();
    }

    // HP�ٿ� ���� ����
    void UpdateHPBar()
    {
        if (hpFillImage != null)
            hpFillImage.fillAmount = currentHP / maxHP;

        if (hpText != null)
            hpText.text = Mathf.RoundToInt(currentHP) + " / " + Mathf.RoundToInt(maxHP);
        // ��: 80 / 100
    }
}