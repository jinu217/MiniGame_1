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
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButton(0))
            _targetX = Mathf.Clamp(ScreenToRange(Input.mousePosition.x), xLimits.x, xLimits.y);
#else
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(t.fingerId))
                return;

            if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary || t.phase == TouchPhase.Began)
                _targetX = Mathf.Clamp(ScreenToRange(t.position.x), xLimits.x, xLimits.y);
        }
#endif

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
}
