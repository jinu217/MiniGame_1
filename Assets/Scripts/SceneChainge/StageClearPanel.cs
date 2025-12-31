using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageClearPanel : MonoBehaviour
{
    [Header("Buttons")]
    public Button nextStageButton;
    public Button retryButton;

    [Header("Stage Settings")]
    public int lastStageNumber = 5;

    public static StageClearPanel Instance { get; private set; }

    string nextSceneName;
    string currentSceneName;

    void Awake()
    {
        Instance = this;

        if (nextStageButton != null)
        {
            nextStageButton.onClick.RemoveListener(OnClickNextStage);
            nextStageButton.onClick.AddListener(OnClickNextStage);
        }

        if (retryButton != null)
        {
            retryButton.onClick.RemoveListener(OnClickRetry);
            retryButton.onClick.AddListener(OnClickRetry);
        }
    }

    void OnDisable()
    {
        Time.timeScale = 1f;
    }

    public void Open()
    {
        Transform t = transform;
        while (t != null)
        {
            if (!t.gameObject.activeSelf)
                t.gameObject.SetActive(true);
            t = t.parent;
        }

        currentSceneName = SceneManager.GetActiveScene().name;
        int stageNum = ParseStageNumber(currentSceneName);

        if (stageNum >= lastStageNumber)
        {
            if (GameClearPanel.Instance != null)
                GameClearPanel.Instance.Open();
            return;
        }

        nextSceneName = "Stage" + (stageNum + 1);

        if (nextStageButton) nextStageButton.gameObject.SetActive(true);
        if (retryButton) retryButton.gameObject.SetActive(true);

        transform.SetAsLastSibling();
        Time.timeScale = 0f;
    }

    void OnClickNextStage()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Open();
            if (string.IsNullOrEmpty(nextSceneName)) return;
        }

        if (GameManager.Instance != null)
            GameManager.Instance.PrepareNextStageKeepHp();

        CloseForSceneChange();
        SceneManager.LoadScene(nextSceneName);
    }

    void OnClickRetry()
    {
        if (string.IsNullOrEmpty(currentSceneName))
            currentSceneName = SceneManager.GetActiveScene().name;

        if (GameManager.Instance != null)
            GameManager.Instance.PrepareRestartToStageStartHp();

        CloseForSceneChange();
        SceneManager.LoadScene(currentSceneName);
    }

    void CloseForSceneChange()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }

    int ParseStageNumber(string sceneName)
    {
        if (!sceneName.StartsWith("Stage"))
            return 1;

        int num;
        if (int.TryParse(sceneName.Replace("Stage", ""), out num))
            return num;

        return 1;
    }
}
