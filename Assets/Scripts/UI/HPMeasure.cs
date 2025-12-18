using UnityEngine;
using UnityEngine.UI; // UI 조작용
using TMPro; // TextMeshPro 사용 시 필요
using System.Linq; // LINQ 사용

public class HPMeasure : MonoBehaviour
{
    public Image hpFillImage;   // HP바 Fill 이미지
    public TMP_Text hpText;     // HP 수치 표시용 Text (TextMeshPro)
    
    public float maxHP = 100;
    float tmpHP;
    bool hpFilledForFirstBoss = false;

    void Start()
    {
        // 기존: 항상 HP를 채움 → 주석 처리
        // GameManager.gameManager.playerHp = maxHP;
        UpdateHPBar();
        tmpHP = GameManager.gameManager.playerHp;
    }

    void Update()
    {
        // 1. 첫 번째 보스(과제) 등장 시 HP를 가득 채움 (한 번만)
        if (!hpFilledForFirstBoss)
        {
            // BossBase를 상속받는 모든 보스 오브젝트 탐색
            var boss = FindObjectOfType<BossBase>();
            if (boss != null)
            {
                // BossConfig를 BossManager에서 참조
                var bossManager = FindObjectOfType<BossManager>();
                if (bossManager != null && bossManager.config != null && bossManager.config.phases != null)
                {
                    // phases에 PaperShot 패턴이 있으면 과제(1스테이지) 보스
                    bool isFirstBoss = bossManager.config.phases.Any(phase => phase.pattern == BossPatternType.PaperShot);
                    if (isFirstBoss)
                    {
                        GameManager.gameManager.playerHp = maxHP;
                        UpdateHPBar();
                        tmpHP = GameManager.gameManager.playerHp;
                        hpFilledForFirstBoss = true;
                        Debug.Log("첫 번째 보스(과제) 등장! 플레이어 HP를 가득 채움");
                    }
                }
            }
        }

        if(tmpHP != GameManager.gameManager.playerHp)
        {
            UpdateHPBar();
            tmpHP = GameManager.gameManager.playerHp;
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
        if (hpFillImage != null)
            hpFillImage.fillAmount = GameManager.gameManager.playerHp / maxHP;

        if (hpText != null)
            hpText.text = GameManager.gameManager.playerHp + " / " + maxHP;
        // 예: 80 / 100
    }
}