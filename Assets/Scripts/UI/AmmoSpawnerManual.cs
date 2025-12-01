using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AmmoSpawnerManual : MonoBehaviour
{
    public GameObject ammoIconPrefab;   // 총알 아이콘 프리팹 연결용
    public RectTransform ammoContainer; // 아이콘들이 쌓일 부모 객체
    public float iconHeight = 32f;      // 아이콘 높이
    public float spacing = 5f;          // 아이콘 간격
    public float iconUpdateTerm = 0.1f; // 아이콘 추가/삭제 간격(초)

    private List<GameObject> spawnedIcons = new List<GameObject>();
    private GameManager gameManager;    // GameManager 참조
    private int lastSkillPoint = -1;    // 이전 skillPoint 값 저장
    private Coroutine updateRoutine;    // 코루틴 중복 방지

    void Start()
    {
        gameManager = GameManager.gameManager; // 싱글톤 인스턴스 사용
        if (gameManager == null)
        {
            Debug.LogError("GameManager 오브젝트를 찾을 수 없습니다!");
            enabled = false;
            return;
        }
        lastSkillPoint = gameManager.skillPoint;
        UpdateAmmoIconsInstant();
    }

    void Update()
    {
        if (gameManager == null) return;
        if (gameManager.skillPoint != lastSkillPoint)
        {
            if (updateRoutine != null)
                StopCoroutine(updateRoutine);
            updateRoutine = StartCoroutine(UpdateAmmoIconsSmooth(gameManager.skillPoint));
            lastSkillPoint = gameManager.skillPoint;
        }
    }

    // 즉시 동기화(초기화용)
    void UpdateAmmoIconsInstant()
    {
        int targetCount = Mathf.Max(0, gameManager.skillPoint);
        while (spawnedIcons.Count > targetCount)
            RemoveAmmoIcon();
        while (spawnedIcons.Count < targetCount)
            AddAmmoIcon();
    }

    // 순차적으로 아이콘을 추가/삭제
    IEnumerator UpdateAmmoIconsSmooth(int targetCount)
    {
        targetCount = Mathf.Max(0, targetCount);
        // 아이콘이 더 많으면 한 개씩 제거
        while (spawnedIcons.Count > targetCount)
        {
            RemoveAmmoIcon();
            yield return new WaitForSeconds(iconUpdateTerm);
        }
        // 아이콘이 더 적으면 한 개씩 추가
        while (spawnedIcons.Count < targetCount)
        {
            AddAmmoIcon();
            yield return new WaitForSeconds(iconUpdateTerm);
        }
        updateRoutine = null;
    }

    void AddAmmoIcon()
    {
        GameObject icon = Instantiate(ammoIconPrefab, ammoContainer);
        spawnedIcons.Add(icon);
        int index = spawnedIcons.Count - 1;
        RectTransform rt = icon.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, index * (iconHeight + spacing));
    }

    void RemoveAmmoIcon()
    {
        if (spawnedIcons.Count > 0)
        {
            GameObject icon = spawnedIcons[spawnedIcons.Count - 1];
            spawnedIcons.RemoveAt(spawnedIcons.Count - 1);
            Destroy(icon);
        }
    }

    public void ClearAllAmmoIcons()
    {
        foreach (var icon in spawnedIcons)
        {
            Destroy(icon);
        }
        spawnedIcons.Clear();
    }
}