using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageClearPanel : MonoBehaviour
{
    [Header("Buttons")]
    public Button nextStageButton;
    public Button retryButton;
    public Button homeButton;

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

        if (homeButton != null)
        {
            homeButton.onClick.RemoveListener(OnClickHome);
            homeButton.onClick.AddListener(OnClickHome);
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

        nextSceneName = (stageNum >= lastStageNumber)
            ? "GameClear"
            : ("Stage" + (stageNum + 1));

        if (stageNum >= lastStageNumber)
        {
            if (nextStageButton) nextStageButton.gameObject.SetActive(false);
            if (retryButton) retryButton.gameObject.SetActive(false);
            if (homeButton) homeButton.gameObject.SetActive(true);
        }
        else
        {
            if (nextStageButton) nextStageButton.gameObject.SetActive(true);
            if (retryButton) retryButton.gameObject.SetActive(true);
            if (homeButton) homeButton.gameObject.SetActive(false);
        }

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

        CloseForSceneChange();
        SceneManager.LoadScene(nextSceneName);
    }

    void OnClickRetry()
    {
        if (string.IsNullOrEmpty(currentSceneName))
            currentSceneName = SceneManager.GetActiveScene().name;

        CloseForSceneChange();
        SceneManager.LoadScene(currentSceneName);
    }

    void OnClickHome()
    {
        CloseForSceneChange();
        SceneManager.LoadScene("StartScene");
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
