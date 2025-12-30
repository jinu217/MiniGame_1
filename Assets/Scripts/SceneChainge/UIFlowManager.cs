using UnityEngine;
using UnityEngine.SceneManagement;

public class UIFlowManager : MonoBehaviour
{
    public static UIFlowManager Instance { get; private set; }

    [Header("Panels (자동으로 찾아서 연결됨)")]
    public StageClearPanel stageClearPanel;
    public GameOverPanel gameOverPanel;
    public GameClearPanel gameClearPanel;

    [Header("Stage Settings")]
    public int lastStageNumber = 5;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    void Start()
    {
        RefreshPanelRefs();
        HideAllPanels();
    }

    void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshPanelRefs();
        HideAllPanels();
        Time.timeScale = 1f;
    }

    void RefreshPanelRefs()
    {
        stageClearPanel = FindFirstObjectByType<StageClearPanel>(FindObjectsInactive.Include);
        gameOverPanel   = FindFirstObjectByType<GameOverPanel>(FindObjectsInactive.Include);
        gameClearPanel  = FindFirstObjectByType<GameClearPanel>(FindObjectsInactive.Include);
    }

    void HideAllPanels()
    {
        if (stageClearPanel != null) stageClearPanel.gameObject.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.gameObject.SetActive(false);
        if (gameClearPanel != null) gameClearPanel.gameObject.SetActive(false);
    }

    public void OnBossDefeated()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        int stageNum = ParseStageNumber(sceneName);

        if (stageNum >= lastStageNumber)
        {
            ShowGameClear();
        }
        else
        {
            ShowStageClear();
        }
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.Open();
        else Debug.LogError("[UIFlowManager] GameOverPanel을 찾지 못했습니다.");
    }

    public void ShowStageClear()
    {
        if (stageClearPanel != null) stageClearPanel.Open();
        else Debug.LogError("[UIFlowManager] StageClearPanel을 찾지 못했습니다.");
    }

    public void ShowGameClear()
    {
        if (gameClearPanel != null) gameClearPanel.Open();
        else Debug.LogError("[UIFlowManager] GameClearPanel을 찾지 못했습니다.");
    }

    int ParseStageNumber(string sceneName)
    {
        if (!sceneName.StartsWith("Stage")) return 0;

        int num;
        if (int.TryParse(sceneName.Replace("Stage", ""), out num))
            return num;

        return 0;
    }
}
