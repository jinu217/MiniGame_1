using UnityEngine;
using System.Collections;

public class PanelPairSpawnerSimple : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject plus1panel;
    public GameObject plus2panel;
    public GameObject plus3panel;
    public GameObject minus1panel;
    public GameObject minus2panel;

    [Header("Spawn Settings")]
    public float interval = 10f, z = 25f, y = 0f, leftX = -1.5f, rightX = 3f;

    int[] plusPointRange = { 1, 2, 3 };
    int[] minusPointRange = { -1, -2 };

    public int plusPoint;
    public int minusPoint;




    void OnEnable() => StartCoroutine(Loop());

    IEnumerator Loop()
    {
        while (true)
        {
            // 매번 좌/우 랜덤 스왑
            bool swap = Random.value < 0.5f;

            Vector3 L = new Vector3(leftX,  y, z);
            Vector3 R = new Vector3(rightX, y, z);

            GameObject plusPrefab = null;
            GameObject minusPrefab = null;

            plusPoint = plusPointRange[Random.Range(0, plusPointRange.Length)];
            minusPoint = minusPointRange[Random.Range(0, minusPointRange.Length)];

            switch (plusPoint)
            {
                case 1:
                    plusPrefab = plus1panel; break;
                case 2:
                    plusPrefab = plus2panel; break;
                case 3:
                    plusPrefab = plus3panel; break;
            }
            switch (minusPoint)
            {
                case -1:
                    minusPrefab = minus1panel; break;
                case -2:
                    minusPrefab = minus2panel; break;
            }   


            Instantiate(swap ? plusPrefab  : minusPrefab, L, Quaternion.Euler(0, 180, 0));
            Instantiate(swap ? minusPrefab : plusPrefab,  R, Quaternion.Euler(0, 180, 0));

            yield return new WaitForSeconds(interval);
        }
    }
}
