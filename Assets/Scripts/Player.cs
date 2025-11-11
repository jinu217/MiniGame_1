using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Move Settings")]
    [Tooltip("목표 위치로 수렴하는 시간(값이 클수록 더 느리게/부드럽게)")]
    public float smoothTime = 0.1f;

    [Header("X 이동 한계 (월드 좌표)")]
    public Vector2 xLimits = new Vector2(-5f, 5f);
    float _targetX;
    float _velX;

    [Header("플레이어 변수")]
    public int skillPoint = 0;

    public SlidePanel panel;

    void Start()
    {
        _targetX = transform.position.x;
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        // 에디터/PC: 마우스 버튼을 누르고 있는 동안 화면 X 절대좌표를 범위로 매핑
        if (Input.GetMouseButton(0))
        {
            _targetX = Mathf.Clamp(ScreenToRange(Input.mousePosition.x), xLimits.x, xLimits.y);
        }
#else
        // 모바일: 손가락이 화면에 닿아 있으면(이동/정지/시작) 절대좌표를 범위로 매핑
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary || t.phase == TouchPhase.Began)
            {
                _targetX = Mathf.Clamp(ScreenToRange(t.position.x), xLimits.x, xLimits.y);
            }
        }
#endif
        // 부드럽게 목표로 수렴
        Vector3 pos = transform.position;
        pos.x = Mathf.SmoothDamp(pos.x, _targetX, ref _velX, smoothTime);
        transform.position = pos;
    }

    // 화면 가로(0 ~ Screen.width)를 xLimits 범위로 선형 매핑
    float ScreenToRange(float screenX)
    {
        float t = Mathf.InverseLerp(0f, Screen.width, screenX); // 0~1
        return Mathf.Lerp(xLimits.x, xLimits.y, t);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlusPanel"))
        {
            panel = other.GetComponent<SlidePanel>();
            skillPoint += panel.panelPoint;
            Debug.Log("스킬 포인트: " + skillPoint);
            Destroy(other.gameObject);
        }
        if (other.CompareTag("MinusPanel"))
        {
            panel = other.GetComponent<SlidePanel>();
            skillPoint += panel.panelPoint;
            if (skillPoint < 0f) skillPoint = 0;
            Debug.Log("스킬 포인트: " + skillPoint);
            Destroy(other.gameObject);
        }
    }


}
