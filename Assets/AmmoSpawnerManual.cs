using UnityEngine;
using System.Collections.Generic;

public class AmmoSpawnerManual : MonoBehaviour
{
    public GameObject ammoIconPrefab;   // �Ѿ� ������ ������ �����
    public RectTransform ammoContainer; // �����ܵ��� ���� �θ� ��ü
    public float iconHeight = 32f;      // ������ ����
    public float spacing = 5f;          // ������ ����
    public float spawnInterval = 0.2f;  // ���� ����

    private List<GameObject> spawnedIcons = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnAmmo(5)); // �׽�Ʈ�� 5�� ����
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