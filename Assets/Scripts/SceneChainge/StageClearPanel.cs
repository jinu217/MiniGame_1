using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

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

    Coroutine pauseRoutine;

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
        if (pauseRoutine != null)
        {
            StopCoroutine(pauseRoutine);
            pauseRoutine = null;
        }
    }

    public void Open()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        currentSceneName = SceneManager.GetActiveScene().name;
        int stageNum = ParseStageNumber(currentSceneName);

        if (stageNum >= lastStageNumber)
        {
            if (GameClearPanel.Instance != null)
                GameClearPanel.Instance.Open();
            else
                Debug.LogWarning("[StageClearPanel] GameClearPanel.Instance is null");
            return;
        }

        nextSceneName = "Stage" + (stageNum + 1);

        if (nextStageButton) nextStageButton.gameObject.SetActive(true);
        if (retryButton) retryButton.gameObject.SetActive(true);

        transform.SetAsLastSibling();

        if (pauseRoutine != null) StopCoroutine(pauseRoutine);
        pauseRoutine = StartCoroutine(PauseNextFrame());
    }

    IEnumerator PauseNextFrame()
    {
        yield return null;
        Time.timeScale = 0f;
        pauseRoutine = null;
    }

    void OnClickNextStage()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            currentSceneName = SceneManager.GetActiveScene().name;
            int stageNum = ParseStageNumber(currentSceneName);
            nextSceneName = "Stage" + (stageNum + 1);
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

        if (pauseRoutine != null)
        {
            StopCoroutine(pauseRoutine);
            pauseRoutine = null;
        }

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
