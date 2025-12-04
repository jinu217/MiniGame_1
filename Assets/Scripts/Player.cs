using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Move Settings")]
    [Tooltip("목표 위치로 수렴하는 시간(값이 클수록 더 느리게/부드럽게)")]
    public float smoothTime = 0.1f;

    [Header("X 이동 한계 (월드 좌표)")]
    public Vector2 xLimits = new Vector2(-5f, 5f);
    float _targetX;
    float _velX;

    public SlidePanel panel;

    // GameOver 및 HP 체크를 위해 GameManager 캐시
    private GameManager gameManager;

    void Start()
    {
        _targetX = transform.position.x;

        // GameManager 캐시
        if (GameManager.gameManager != null)
            gameManager = GameManager.gameManager;
        else
            gameManager = FindFirstObjectByType<GameManager>();
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        //UI 클릭 시 이동 금지 
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

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
             //UI 터치 시 이동 금지 (모바일)
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(t.fingerId))
                return;

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

        // ----- 게임 오버 조건(HP) 체크 -----
        if (gameManager != null && !gameManager.isGameOver && !gameManager.isStageClear)
        {
            // 플레이어 HP가 0 이하이면 GameOver
            if (gameManager.playerHp <= 0f)
            {
                gameManager.playerHp = 0f;   // 음수 방지
                HandleGameOver();
                return;
            }
        }
    }

    // 화면 가로(0 ~ Screen.width)를 xLimits 범위로 선형 매핑
    float ScreenToRange(float screenX)
    {
        float t = Mathf.InverseLerp(0f, Screen.width, screenX); // 0~1
        return Mathf.Lerp(xLimits.x, xLimits.y, t);
    }

    int GetMaxSkillPointForCurrentStage()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        // 1, 2, 3 스테이지: 최대 10
        if (sceneIndex >= 1 && sceneIndex <= 3)
            return 10;

        // 4, 5 스테이지: 최대 12
        if (sceneIndex == 4 || sceneIndex == 5)
            return 12;

        // 그 외 씬은 기본값 10 (필요하면 수정 가능)
        return 10;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlusPanel"))
        {
            panel = other.GetComponent<SlidePanel>();

            GameManager gm = GameManager.gameManager;
            gm.skillPoint += gm.plusPanelPoint;

            // ▼ 스테이지별 최대치 적용
            int maxSkill = GetMaxSkillPointForCurrentStage();
            if (gm.skillPoint > maxSkill)
                gm.skillPoint = maxSkill;

            Debug.Log("스킬 포인트: " + gm.skillPoint);
            Destroy(other.gameObject);
        }

        if (other.CompareTag("MinusPanel"))
        {
            panel = other.GetComponent<SlidePanel>();

            GameManager gm = GameManager.gameManager;
            gm.skillPoint += gm.minusPanelPoint;

            // ▼ 먼저 0 미만 방지
            if (gm.skillPoint < 0)
                gm.skillPoint = 0;

            // ▼ 그 다음 스테이지별 최대치 적용
            int maxSkill = GetMaxSkillPointForCurrentStage();
            if (gm.skillPoint > maxSkill)
                gm.skillPoint = maxSkill;

            Debug.Log("스킬 포인트: " + gm.skillPoint);
            Destroy(other.gameObject);
        }
    }
    // TimerUI에서 타이머 종료 시 호출할 함수
    public void HandleTimerEnd()
    {
        // 이미 클리어나 게임 오버 상태면 무시
        if (gameManager != null && (gameManager.isGameOver || gameManager.isStageClear))
            return;

        // 필요하다면 여기에서 "타이머로 인해 끝났다" 같은 플래그를 따로 둘 수도 있음
        HandleGameOver();
    }

    // 실제 GameOver 상태 세팅 + 씬 전환 담당
    void HandleGameOver()
    {
        // GameManager 다시 한 번 확보 (혹시 null일 때 대비)
        if (gameManager == null)
        {
            if (GameManager.gameManager != null)
                gameManager = GameManager.gameManager;
            else
                gameManager = FindFirstObjectByType<GameManager>();
        }

        if (gameManager != null)
        {
            if (gameManager.isGameOver)
                return; // 중복 GameOver 방지

            gameManager.isGameOver = true;
            gameManager.isStageClear = false;
        }

        // 실제 GameOver 씬 전환
        SceneManager.LoadScene("GameOverScene");
    }

}
