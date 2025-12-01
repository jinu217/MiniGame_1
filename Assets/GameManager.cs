using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    public PanelPairSpawnerSimple panel;
    
    public float playTime = 0f;

    [Header("Player Info")]
    public float playerHp;
    public int skillPoint = 1;

    [Header("Player Damage")]
    public int playerBaseDamage = 1;
    public float damageMultiplier = 1f;

    [Header("Bug Info")]
    public float bugSpawnCycle = 1f;
    public int bugDamage = 1;

    [Header("Healkit Info")]
    public float healKitSpawnCycle = 2f;
    public int healValue = 1;

    [Header("Pannel Info")]
    public int plusPanelPoint;
    public int minusPanelPoint;

    public int CurrentPlayerDamage
        => Mathf.Max(1, Mathf.RoundToInt(playerBaseDamage * damageMultiplier));

    public bool isGameOver = false;
    public bool isStageClear = false; // 씬전환에서 가져야가 할 정보

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
        playTime += Time.deltaTime;
        plusPanelPoint = panel.plusPoint;
        minusPanelPoint = panel.minusPoint;
        if (isGameOver == true) return; // 플레이어 체력이 0이 되면 true

        if (isStageClear == true) return; // 보스의 체력이 0이 되면 true

        
    }
}
