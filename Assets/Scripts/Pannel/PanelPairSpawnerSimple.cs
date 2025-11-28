using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class PanelPairSpawnerSimple : MonoBehaviour
{
    public GameManager gameManager;
    
    int[] plusPointRange = { 1, 2, 3 };
    int[] minusPointRange = { -1, -2 };

    public int plusPoint;
    public int minusPoint;

    [Header("Prefabs")]
    

    public GameObject plus1Panel;
    public GameObject plus2Panel;
    public GameObject plus3Panel;
    public GameObject minus1Panel;
    public GameObject minus2Panel;

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

            GameObject plusPrefab = null;
            GameObject minusPrefab = null;

            plusPoint = plusPointRange[Random.Range(0, plusPointRange.Length)];
            minusPoint = minusPointRange[Random.Range(0, minusPointRange.Length)];

            gameManager.plusPanelPoint = plusPoint;
            gameManager.minusPanelPoint = minusPoint;
            
            switch (plusPoint)
            {
                case 1:
                    plusPrefab = plus1Panel; break;
                case 2:
                    plusPrefab = plus2Panel; break;
                case 3:
                    plusPrefab = plus3Panel; break;
            }

            switch (minusPoint)
            {
                case -1:
                    minusPrefab = minus1Panel; break;

                case -2:
                    minusPrefab = minus2Panel; break;
            }

            

            Instantiate(swap ? plusPrefab : minusPrefab, L, Quaternion.Euler(0, 180, 0));
            Instantiate(swap ? minusPrefab : plusPrefab,  R, Quaternion.Euler(0, 180, 0));


            yield return new WaitForSeconds(interval);
        }
    }
}
