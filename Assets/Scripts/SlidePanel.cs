using UnityEngine;

public class SlidePanel : MonoBehaviour
{

    public float speed = 5f;        // 내려오는 속도
    public float despawnZ = -10f;   // 화면 뒤로 지나가면 삭제

    public int panelPoint;

    private void Awake()
    {
        if (gameObject.CompareTag("PlusPanel"))
        {
            panelPoint = Random.Range(1, 5);

        }
        if (gameObject.CompareTag("MinusPanel"))
        {
            panelPoint = Random.Range(-5, -1);
        }
    }

    void Update()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);
        if (transform.position.z < despawnZ) Destroy(gameObject);


    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            Destroy(gameObject); // 패널 제거
    }

}
