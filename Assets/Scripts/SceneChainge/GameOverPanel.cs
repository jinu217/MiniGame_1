using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    [Header("Buttons")]
    public Button homeButton;
    public Button restartButton;

    bool opened = false;

    void Awake()
    {
        if (homeButton != null)
        {
            homeButton.onClick.RemoveAllListeners();
            homeButton.onClick.AddListener(OnClickHome);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(OnClickRestart);
        }
    }

    void OnDisable()
    {
        opened = false;
        Time.timeScale = 1f;
    }

    public void Open()
    {
        if (opened) return;
        opened = true;

        // ✅ 부모까지 켜서(비활성화 시작해도) 확실히 표시
        Transform t = transform;
        while (t != null)
        {
            if (!t.gameObject.activeSelf)
                t.gameObject.SetActive(true);
            t = t.parent;
        }

        transform.SetAsLastSibling();
        Time.timeScale = 0f;
    }

    void OnClickHome()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
        SceneManager.LoadScene("StartScene");
    }

    void OnClickRestart()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
