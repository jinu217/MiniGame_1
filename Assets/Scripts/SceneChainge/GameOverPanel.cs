using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameOverPanel : MonoBehaviour
{
    [Header("Buttons")]
    public Button homeButton;
    public Button restartButton;

    bool opened = false;
    Coroutine pauseRoutine;

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

        if (pauseRoutine != null)
        {
            StopCoroutine(pauseRoutine);
            pauseRoutine = null;
        }
    }

    public void Open()
    {
        if (opened) return;
        opened = true;

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

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

    void OnClickHome()
    {
        Close();
        if (GameManager.Instance != null)
            GameManager.Instance.ResetRunToFull();

        SceneManager.LoadScene("StartScene");
    }

    void OnClickRestart()
    {
        Close();
        if (GameManager.Instance != null)
            GameManager.Instance.PrepareRestartToStageStartHp();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Close()
    {
        Time.timeScale = 1f;

        if (pauseRoutine != null)
        {
            StopCoroutine(pauseRoutine);
            pauseRoutine = null;
        }

        gameObject.SetActive(false);
        opened = false;
    }
}
