using UnityEngine;
using UnityEngine.UI; // UI 조작용
using TMPro; // TextMeshPro 사용 시 필요
using System.Linq; // LINQ 사용

public class HPMeasure : MonoBehaviour
{
    public Image hpFillImage;   // HP바 Fill 이미지
    public TMP_Text hpText;     // HP 수치 표시용 Text (TextMeshPro)
    
    float maxHP;
    float tmpHP;
    bool hpFilledForFirstBoss = false;

    void Start()
    {
        // 기존: 항상 HP를 채움 → 주석 처리
        // GameManager.gameManager.playerHp = maxHP;
        UpdateHPBar();
        tmpHP = GameManager.gameManager.playerHp;
        maxHP = GameManager.gameManager.playerMaxHp;
    }

    void Update()
    {
        if(tmpHP != GameManager.gameManager.playerHp)
        {
            UpdateHPBar();
            tmpHP = GameManager.gameManager.playerHp;
        }
        if(GameManager.gameManager.playerHp > maxHP)
        {
            GameManager.gameManager.playerHp = maxHP;
        }
        // 테스트: 스페이스바 누르면 체력 10 감소
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(10);
        }
    }

    // 체력 감소 처리
    public void TakeDamage(int damage)
    {
        GameManager.gameManager.playerHp -= damage;
        Debug.Log(damage + "만큼 데미지를 입었습니다!");
        if (GameManager.gameManager.playerHp < 0) GameManager.gameManager.playerHp = 0;
        UpdateHPBar();
    }

    // HP바와 숫자 갱신
    void UpdateHPBar()
    {
        float currentHP = GameManager.gameManager.playerHp;
        float maxHP = GameManager.gameManager.playerMaxHp; // 항상 최신값 사용

        if (hpFillImage != null && maxHP > 0)
            hpFillImage.fillAmount = currentHP / maxHP;

        if (hpText != null)
            hpText.text = currentHP + " / " + maxHP;
    }
}