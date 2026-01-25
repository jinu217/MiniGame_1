using UnityEngine;
using UnityEngine.UI;

public class StoryPanel : MonoBehaviour
{
    const string KEY_SHOW_STORY = "ShowStoryOnStage1";

    [Header("Pages")]
    public GameObject[] pages;

    [Header("Buttons")]
    public Button rightButton;
    public Button leftButton;
    public Button skipButton;

    int index = 0;

    void Awake()
    {
        if (rightButton != null) rightButton.onClick.AddListener(Next);
        if (leftButton != null)  leftButton.onClick.AddListener(Prev);
        if (skipButton != null)  skipButton.onClick.AddListener(OnClickSkip);
    }

    void OnEnable()
    {
        bool show = PlayerPrefs.GetInt(KEY_SHOW_STORY, 1) == 1;

        if (!show)
        {
            gameObject.SetActive(false); 
            return;
        }

        PlayerPrefs.SetInt(KEY_SHOW_STORY, 0);
        PlayerPrefs.Save();

        Time.timeScale = 0f;

        index = 0;
        Refresh();
    }

    void Update()
    {
        if (gameObject.activeInHierarchy && Time.timeScale != 0f)
            Time.timeScale = 0f;
    }

    void OnDisable()
    {
        Time.timeScale = 1f;
    }

    void Next()
    {
        if (pages == null || pages.Length == 0) return;
        index = Mathf.Min(index + 1, pages.Length - 1);
        Refresh();
    }

    void Prev()
    {
        if (pages == null || pages.Length == 0) return;
        index = Mathf.Max(index - 1, 0);
        Refresh();
    }

    void OnClickSkip()
    {
        index = 0;
        gameObject.SetActive(false);
    }

    void Refresh()
    {
        for (int i = 0; i < pages.Length; i++)
            if (pages[i] != null) pages[i].SetActive(i == index);

        if (leftButton != null)  leftButton.interactable  = index > 0;
        if (rightButton != null) rightButton.interactable = index < pages.Length - 1;
    }
}
