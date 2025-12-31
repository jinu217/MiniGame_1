using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HPBackgroundAlpha : MonoBehaviour
{
    public Image targetImage; // 인스펙터에서 할당할 이미지

    void Awake()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();
        SetAlpha(0f);
    }

    void Start()
    {
        StopAllCoroutines();
        SetAlpha(0f);
        StartCoroutine(SetAlphaAfterDelay());
    }

    IEnumerator SetAlphaAfterDelay()
    {
        BossManager bossManager = null;
        while (bossManager == null)
        {
            bossManager = FindFirstObjectByType<BossManager>();
            if (bossManager == null)
                yield return null;
        }

        float delay = bossManager.bossSpawnDelay;
        if (delay < 0f || float.IsNaN(delay))
            delay = 0f;

        yield return new WaitForSeconds(delay);

        SetAlpha(1f); // delay 후 불투명도 1(완전 불투명)
    }

    void Update()
    {
        
    }

    public void SetAlpha(float alpha)
    {
        if (targetImage == null) return;
        var c = targetImage.color;
        c.a = Mathf.Clamp01(alpha); // 0(완전 투명) ~ 1(완전 불투명)
        targetImage.color = c;
    }
}