using UnityEngine;

public class SlidePanel : MonoBehaviour
{
    public float speed = 5f;        // 내려오는 속도
    public float despawnZ = -10f;   // 화면 뒤로 지나가면 삭제

    void Update()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);
        if (transform.position.z < despawnZ) Destroy(gameObject);
    }
}
