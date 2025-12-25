using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class HPBackgroundAlpha : MonoBehaviour
{
    Image image;
    bool isAlphaZero = false; // 중복 호출 방지용

    void Awake()
    {
        image = GetComponent<Image>();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        ApplyAlphaLogic();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyAlphaLogic();
    }

    void ApplyAlphaLogic()
    {
        StopAllCoroutines();
        SetAlpha(0f);
        isAlphaZero = false;
        StartCoroutine(SetAlphaAfterDelay());
    }

    IEnumerator SetAlphaAfterDelay()
    {
        BossManager bossManager = null;
        while (bossManager == null)
        {
            bossManager = FindObjectOfType<BossManager>();
            if (bossManager == null)
                yield return null;
        }

        float delay = bossManager.bossSpawnDelay;
        if (delay < 0f || float.IsNaN(delay))
            delay = 0f;

        yield return new WaitForSeconds(delay);

        SetAlpha(1f);
        isAlphaZero = false;
    }

    void Update()
    {
        // BossManager의 ended가 true면 불투명도를 0으로
        var bossManager = FindObjectOfType<BossManager>();
        if (bossManager != null && bossManager.GetType().GetField("ended", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) != null)
        {
            bool ended = (bool)bossManager.GetType().GetField("ended", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(bossManager);
            if (ended && !isAlphaZero)
            {
                SetAlpha(0f);
                isAlphaZero = true;
            }
        }
    }

    public void SetAlpha(float alpha)
    {
        if (image == null) return;
        var c = image.color;
        c.a = Mathf.Clamp01(alpha);
        image.color = c;
    }
}