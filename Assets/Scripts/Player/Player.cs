using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Move Settings")]
    public float smoothTime = 0.1f;

    [Header("X 이동 한계 (월드 좌표)")]
    public Vector2 xLimits = new Vector2(-5f, 5f);

    float _targetX;
    float _velX;

    public SlidePanel panel;

    private GameManager gameManager;

    bool _canCheckHp = false; 

    void Start()
    {
        _targetX = transform.position.x;

        gameManager = (GameManager.gameManager != null)
            ? GameManager.gameManager
            : FindFirstObjectByType<GameManager>();

        StartCoroutine(EnableHpCheckNextFrame());
    }

    System.Collections.IEnumerator EnableHpCheckNextFrame()
    {
        yield return null; // 다음 프레임
        _canCheckHp = true;
    }

    void Update()
    {// 1. UI를 클릭/터치 중이라면 이동 로직 전체를 무시
        if (IsPointerOverUI()) return;

        // 2. 입력 처리 (PC + 모바일 통합)
        bool isInput = false;
        Vector2 inputPos = Vector2.zero;

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButton(0))
        {
            isInput = true;
            inputPos = Input.mousePosition;
        }
#else
    if (Input.touchCount > 0)
    {
        Touch t = Input.GetTouch(0);
        isInput = (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary || t.phase == TouchPhase.Began);
        inputPos = t.position;
    }
#endif

        if (isInput)
        {
            _targetX = Mathf.Clamp(ScreenToRange(inputPos.x), xLimits.x, xLimits.y);
        }

        Vector3 pos = transform.position;
        pos.x = Mathf.SmoothDamp(pos.x, _targetX, ref _velX, smoothTime);
        transform.position = pos;

        if (!_canCheckHp) return;

        var sceneName = SceneManager.GetActiveScene().name;
        if (!sceneName.StartsWith("Stage")) return;

        if (gameManager != null && !gameManager.isGameOver && !gameManager.isStageClear)
        {
            if (gameManager.playerHp <= 0f)
            {
                gameManager.playerHp = 0f;
                gameManager.GameOver();
            }
        }
    }

    float ScreenToRange(float screenX)
    {
        float t = Mathf.InverseLerp(0f, Screen.width, screenX);
        return Mathf.Lerp(xLimits.x, xLimits.y, t);
    }

    int GetMaxSkillPointForCurrentStage()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex >= 1 && sceneIndex <= 3) return 10;
        if (sceneIndex == 4 || sceneIndex == 5) return 12;
        return 10;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlusPanel"))
        {
            panel = other.GetComponent<SlidePanel>();

            GameManager gm = GameManager.gameManager;
            gm.skillPoint += gm.plusPanelPoint;

            int maxSkill = GetMaxSkillPointForCurrentStage();
            if (gm.skillPoint > maxSkill) gm.skillPoint = maxSkill;

            Debug.Log("스킬 포인트: " + gm.skillPoint);
            Destroy(other.gameObject);
        }

        if (other.CompareTag("MinusPanel"))
        {
            panel = other.GetComponent<SlidePanel>();

            GameManager gm = GameManager.gameManager;
            gm.skillPoint += gm.minusPanelPoint;

            if (gm.skillPoint < 0) gm.skillPoint = 0;

            int maxSkill = GetMaxSkillPointForCurrentStage();
            if (gm.skillPoint > maxSkill) gm.skillPoint = maxSkill;

            Debug.Log("스킬 포인트: " + gm.skillPoint);
            Destroy(other.gameObject);
        }
    }

    public void HandleTimerEnd()
    {
        if (gameManager != null && (gameManager.isGameOver || gameManager.isStageClear))
            return;

        if (gameManager != null)
            gameManager.GameOver();
    }

    // UI 터치 여부를 확인하는 통합 함수
    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        // PC용 검사
        if (EventSystem.current.IsPointerOverGameObject()) return true;

        // 모바일용 검사
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                return true;
        }
        return false;
    }
}
