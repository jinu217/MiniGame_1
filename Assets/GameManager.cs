using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    public static GameManager Instance => gameManager;

    public PanelPairSpawnerSimple panel;

    public float playTime = 0f;

    [Header("Player Info")]
    public float playerMaxHp = 10f;
    public float playerHp = 10f;
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
            panel = FindAnyObjectByType<PanelPairSpawnerSimple>();

        if (playerMaxHp <= 0f) playerMaxHp = 10f;
        if (playerHp <= 0f) playerHp = playerMaxHp;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        panel = FindAnyObjectByType<PanelPairSpawnerSimple>();

        if (scene.name.StartsWith("Stage"))
        {
            isGameOver = false;
            isStageClear = false;

            if (playerMaxHp <= 0f) playerMaxHp = 10f;
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
        if (isGameOver) return;

        isGameOver = true;
        isStageClear = false;

        UIFlowManager.Instance?.ShowGameOver();
    }
}
