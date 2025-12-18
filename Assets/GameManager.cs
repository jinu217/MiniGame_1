using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 싱글톤
    public static GameManager gameManager;
    public static GameManager Instance => gameManager;

    // 각 스테이지에 있는 패널 스포너
    public PanelPairSpawnerSimple panel;

    public float playTime = 0f;

    [Header("Player Info")]
    public float playerMaxHp = 10f;   // ✅ 추가: 최대 HP(인스펙터에서 조절)
    public float playerHp = 10f;      // ✅ 초기값(안전하게 10으로 시작)
    public int MaxskillPoint = 10;
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
    public bool isStageClear = false;

    void Awake()
    {
        if (gameManager != null && gameManager != this)
        {
            Destroy(gameObject);
            return;
        }

        gameManager = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        if (panel == null)
        {
            panel = FindAnyObjectByType<PanelPairSpawnerSimple>();
        }

        // ✅ 첫 시작에서도 HP 보정(혹시 인스펙터가 0이어도 최소 1)
        if (playerMaxHp <= 0f) playerMaxHp = 10f;
        if (playerHp <= 0f) playerHp = playerMaxHp;
    }

    // 새 씬 로드될 때 호출됨
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        panel = FindAnyObjectByType<PanelPairSpawnerSimple>();

        // Stage 씬 진입 시 상태 초기화 + HP 리셋(핵심)
        if (scene.name.StartsWith("Stage"))
        {
            isGameOver = false;
            isStageClear = false;

            if (playerMaxHp <= 0f) playerMaxHp = 10f;
            playerHp = playerMaxHp;   // ✅ 핵심: 스테이지 시작하면 HP 무조건 채우기
        }
    }

    void Update()
    {
        playTime += Time.deltaTime;

        if (panel != null)
        {
            plusPanelPoint = panel.plusPoint;
            minusPanelPoint = panel.minusPoint;
        }

        if (isGameOver) return;
        if (isStageClear) return;
    }

    public void GameOver()
    {
        // ✅ 누가 호출했는지 바로 찍어서 범인 추적
        Debug.LogError("[GameOver 호출됨]\n" + System.Environment.StackTrace);

        // 이미 게임 오버면 중복 호출 방지
        if (isGameOver) return;

        isGameOver = true;
        isStageClear = false;

        SceneManager.LoadScene("GameOverScene");
    }
}
