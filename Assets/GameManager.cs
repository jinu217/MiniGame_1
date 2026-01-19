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
    public float healKitSpawnCycle = 2f;
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

        ApplyStageMaxHP(scene.buildIndex);

        if (scene.name == "StartScene")
        {
            ResetRunToFull();
            return;
        }

        if (!scene.name.StartsWith("Stage"))
            return;

        isGameOver = false;
        isStageClear = false;
        if (scene.buildIndex == 1 && pendingHpInit == HpInitMode.Keep)
        {
            playerHp = playerMaxHp;
        }
        else
        {
            switch (pendingHpInit)
            {
                case HpInitMode.Full:
                    playerHp = playerMaxHp;
                    break;

                case HpInitMode.Half:
                    playerHp = Mathf.Ceil(playerMaxHp * 0.5f);
                    break;

                case HpInitMode.Keep:
                default:
                    playerHp = Mathf.Clamp(playerHp, 1f, playerMaxHp);
                    break;
            }
        }
        stageStartHp = playerHp;

        pendingHpInit = HpInitMode.Keep;
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
