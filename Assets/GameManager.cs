using Unity.VisualScripting;
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
        // 씬이 바뀔 때마다 panel 다시 찾기
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        // 처음 씬에서도 한 번 찾아주기
        if (panel == null)
        {
            panel = FindAnyObjectByType<PanelPairSpawnerSimple>();
        }
    }

    // 새 씬 로드될 때 호출됨
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        panel = FindAnyObjectByType<PanelPairSpawnerSimple>();
    }

    void Update()
    {
        playTime += Time.deltaTime;

        // panel이 없으면(해당 씬에 안 두었거나 아직 못 찾았으면) 그냥 건너뛰기
        if (panel != null)
        {
            plusPanelPoint = panel.plusPoint;
            minusPanelPoint = panel.minusPoint;
        }

        if (isGameOver) return;
        if (isStageClear) return;
    }
}
