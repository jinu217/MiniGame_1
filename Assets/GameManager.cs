using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;

    public float playTime = 0f;
    public int playerHp;
    public int playerDefaultHp = 34;

    [Header("Pannel Point")]
    public int plusPanelPoint;
    public int minusPanelPoint;

    [Header("Player Damage")]
    public int playerBaseDamage = 1;    
    public float damageMultiplier = 1f;

    public int stageValue = 1;
    public bool stageClear = false;
    


    public int CurrentPlayerDamage
        => Mathf.Max(1, Mathf.RoundToInt(playerBaseDamage * damageMultiplier));

    public bool isGameOver = false;
    
    void Awake()
    {
        if(gameManager != null && gameManager != this)
        {
            Destroy(gameObject);
            return;
        }
        gameManager = this;


        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (isGameOver == true) return;

        playTime += Time.deltaTime;
        playerBaseDamage = stageValue; // 스테이지 레벨에 따른 플레이어 공격력 조정
        
        

        if (stageClear == true)
        {
            // 스테이지 클리어 시 팝업 창 도시 or 팝업 씬으로 전환
            playerHp = playerDefaultHp + 2; // 스테이지 클리어 시 최대체력으로 회복 및 최대체력 증가   # 34, 36, 38, 40, 42
            stageClear = false;
        }


    }
}
