using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;

    public float playTime = 0f;
    public int playerHp;

    [Header("Pannel Point")]
    public int plusPanelPoint;
    public int minusPanelPoint;

    [Header("Player Damage")]
    public int playerBaseDamage = 1;    
    public float damageMultiplier = 1f;

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

        if (stageClear == true)
        {
            // 스테이지 클리어 시 팝업 창 도시 or 팝업 씬으로 전환
            stageClear = false;
        }

    }
}
