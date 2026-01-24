using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    public static GameManager Instance => gameManager;

    public PanelPairSpawnerSimple panel;

    public float playTime = 0f;

    [Header("shuting sound")]
    public AudioClip healSound;
    public AudioClip BugHitSound;
    public AudioClip BossHitSound;
    public AudioClip PlayerTakeDmg;

    [Header("Player Info")]
    public float playerMaxHp = 10f;
    public float playerHp = 60f;

    public float stageStartHp = 10f;

    public int MaxskillPoint = 10;
    public int skillPoint = 1;

    [Header("Player Damage")]
    public int playerBaseDamage = 1;
    public float damageMultiplier = 1f;

    [Header("Bug Info")]
    public float bugSpawnCycle = 1f;
    public float bugHp = 2;
    public int bugDamage = 1;

    [Header("Healkit Info")]
    public float healKitSpawnCycle = 5f;
    public int healValue = 1;

    [Header("Pannel Info")]
    public int plusPanelPoint;
    public int minusPanelPoint;

    public int CurrentPlayerDamage
        => Mathf.Max(1, Mathf.RoundToInt(playerBaseDamage * damageMultiplier));

    public bool isGameOver = false;
    public bool isStageClear = false;

    enum HpInitMode { Keep, Half, Full }
    HpInitMode pendingHpInit = HpInitMode.Keep;

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
        ApplyStageMaxHP(SceneManager.GetActiveScene().buildIndex);
        playerHp = playerMaxHp;
        stageStartHp = playerHp;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        panel = FindAnyObjectByType<PanelPairSpawnerSimple>();
        Time.timeScale = 1f;

        // 1. 먼저 해당 스테이지의 MaxHP를 설정합니다.
        ApplyStageMaxHP(scene.buildIndex);

        if (scene.name == "StartScene")
        {
            ResetRunToFull();
            return;
        }

        // "Stage"로 시작하는 씬일 경우
        if (scene.name.StartsWith("Stage"))
        {
            isGameOver = false;
            isStageClear = false;

            // 2. [수정됨] 이전 스테이지 체력과 상관없이 항상 최대 체력으로 설정
            playerHp = playerMaxHp;

            // 스테이지 시작 시점의 체력 기록 (재시작 시 활용)
            stageStartHp = playerHp;

            skillPoint = 2;

            // 예약 상태 초기화
            pendingHpInit = HpInitMode.Keep;
        }
    }

    private void ApplyStageMaxHP(int index)
    {
        switch (index)
        {
            case 1: playerMaxHp = 60f; break;
            case 2: playerMaxHp = 66f; break;
            case 3: playerMaxHp = 72f; break;
            case 4: playerMaxHp = 78f; break;
            case 5: playerMaxHp = 84f; break;
            // 기본값 설정 (필요 시)
            default: if (index > 0) playerMaxHp = 10f; break;
        }
        Debug.Log($"Stage {index} 로드: MaxHP가 {playerMaxHp}로 설정되었습니다.");
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

    public void PrepareRestartToStageStartHp()
    {
        playerHp = stageStartHp;
        pendingHpInit = HpInitMode.Keep;
    }

    public void PrepareNextStageKeepHp()
    {
        pendingHpInit = HpInitMode.Keep;
    }

    public void PrepareStartFullHp()
    {
        pendingHpInit = HpInitMode.Full;
    }

    public void ResetRunToFull()
    {
        isGameOver = false;
        isStageClear = false;

        playTime = 0f;
        skillPoint = 1;
        damageMultiplier = 1f;

        playerHp = playerMaxHp;
        stageStartHp = playerMaxHp;
        pendingHpInit = HpInitMode.Keep;
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        isStageClear = false;
        UIFlowManager.Instance?.ShowGameOver();
    }
    public void PlaySoundAtPlayer(AudioClip clip, float volume = 1.0f)
    {
        if (clip == null) return;

        // "Player" 태그를 가진 오브젝트를 찾습니다.
        GameObject player = GameObject.FindWithTag("Player");

        Vector3 spawnPos = (player != null) ? player.transform.position : Camera.main.transform.position;

        AudioSource.PlayClipAtPoint(clip, spawnPos, volume);
    
    }
}
