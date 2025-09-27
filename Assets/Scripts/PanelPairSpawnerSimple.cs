using UnityEngine;
using System.Collections;

public class PanelPairSpawnerSimple : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject plusPrefab;
    public GameObject minusPrefab;

    [Header("Spawn Settings")]
    public float interval = 10f, z = 25f, y = 0f, leftX = -1.5f, rightX = 3f;

    void OnEnable() => StartCoroutine(Loop());

    IEnumerator Loop()
    {
        while (true)
        {
            // 매번 좌/우 랜덤 스왑
            bool swap = Random.value < 0.5f;

            Vector3 L = new Vector3(leftX,  y, z);
            Vector3 R = new Vector3(rightX, y, z);

            Instantiate(swap ? plusPrefab  : minusPrefab, L, Quaternion.identity);
            Instantiate(swap ? minusPrefab : plusPrefab,  R, Quaternion.identity);

            yield return new WaitForSeconds(interval);
        }
    }
}
