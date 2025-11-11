using UnityEngine;
using UnityEngine.UI; // UI 조작용
using TMPro; // TextMeshPro 사용 시 필요

public class HPMeasure : MonoBehaviour
{
    public Image hpFillImage;   // HP바 Fill 이미지
    public TMP_Text hpText;     // HP 수치 표시용 Text (TextMeshPro)

    public float maxHP = 100f;
    private float currentHP;

    void Start()
    {
        currentHP = maxHP; // 시작 시 체력을 가득 채움
        UpdateHPBar();
    }

    void Update()
    {
        // 테스트: 스페이스바 누르면 체력 10 감소
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(10);
        }
    }

    // 체력 감소 처리
    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;
        UpdateHPBar();
    }

    // HP바와 숫자 갱신
    void UpdateHPBar()
    {
        if (hpFillImage != null)
            hpFillImage.fillAmount = currentHP / maxHP;

        if (hpText != null)
            hpText.text = Mathf.RoundToInt(currentHP) + " / " + Mathf.RoundToInt(maxHP);
        // 예: 80 / 100
    }
}