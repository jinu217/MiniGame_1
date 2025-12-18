using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageClearPanel : MonoBehaviour
{
    [Header("Buttons")]
    public Button nextStageButton;
    public Button retryButton;

    [Header("Stage Settings")]
    public int lastStageNumber = 5; // ✅ 마지막 스테이지 번호

    string nextSceneName;
    string currentSceneName;

    void Awake()
    {
        gameObject.SetActive(false);

        if (nextStageButton != null)
            nextStageButton.onClick.AddListener(OnClickNextStage);
        else
            Debug.LogError("[StageClearPanel] nextStageButton 연결 안 됨");

        if (retryButton != null)
            retryButton.onClick.AddListener(OnClickRetry);
        else
            Debug.LogError("[StageClearPanel] retryButton 연결 안 됨");
    }

    void OnDisable()
    {
        // ✅ 혹시 패널이 꺼진 채로 씬이 바뀌어도 timeScale이 0으로 남지 않게
        Time.timeScale = 1f;
    }

    public void Open()
    {
        currentSceneName = SceneManager.GetActiveScene().name;

        int stageNum = ParseStageNumber(currentSceneName);

        // ✅ 마지막 스테이지면 GameClear로
        nextSceneName = (stageNum >= lastStageNumber) ? "GameClear" : ("Stage" + (stageNum + 1));

        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    void OnClickNextStage()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextSceneName);
    }

    void OnClickRetry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(currentSceneName);
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
