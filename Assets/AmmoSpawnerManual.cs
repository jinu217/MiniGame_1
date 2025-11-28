using UnityEngine;
using System.Collections.Generic;

public class AmmoSpawnerManual : MonoBehaviour
{
    public GameObject ammoIconPrefab;   // 총알 아이콘 프리팹 연결용
    public RectTransform ammoContainer; // 아이콘들이 쌓일 부모 객체
    public float iconHeight = 32f;      // 아이콘 높이
    public float spacing = 5f;          // 아이콘 간격
    public float spawnInterval = 0.2f;  // 생성 간격

    private List<GameObject> spawnedIcons = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnAmmo(5)); // 테스트용 5개 생성
    }

    System.Collections.IEnumerator SpawnAmmo(int ammoCount)
    {
        yield return new WaitForSeconds(2f);

        for (int i = 0; i < ammoCount; i++)
        {
            GameObject icon = Instantiate(ammoIconPrefab, ammoContainer);
            spawnedIcons.Add(icon);

            int index = spawnedIcons.Count - 1;
            RectTransform rt = icon.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0, index * (iconHeight + spacing));

            yield return new WaitForSeconds(spawnInterval);
        }
    }

}