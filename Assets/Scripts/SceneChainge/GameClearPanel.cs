using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameClearPanel : MonoBehaviour
{
    [Header("Buttons")]
    public Button homeButton;

    public static GameClearPanel Instance { get; private set; }

    bool opened = false;
    Coroutine pauseRoutine;

    void Awake()
    {
        Instance = this;

        if (homeButton != null)
        {
            homeButton.onClick.RemoveListener(OnClickHome);
            homeButton.onClick.AddListener(OnClickHome);
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
