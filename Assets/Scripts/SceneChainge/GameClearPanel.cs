using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameClearPanel : MonoBehaviour
{
    [Header("Buttons")]
    public Button homeButton;

    public static GameClearPanel Instance { get; private set; }

    bool opened = false;

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
        Time.timeScale = 1f;
        opened = false;
    }

    public void Open()
    {
        if (opened) return;
        opened = true;

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

        if (GameManager.Instance != null)
            GameManager.Instance.ResetRunToFull();

        SceneManager.LoadScene("StartScene");
    }
}
