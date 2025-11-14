using UnityEngine;
using UnityEngine.UI;
#if TMP_PRESENT
using TMPro;
#endif

public class TimerUI : MonoBehaviour
{
    [Header("텍스트 UI 연결")]
    public bool useTMP = false;    // TextMeshPro 사용 여부 (true면 TMP, false면 유니티 기본 Text 사용)
    public Text timerTextLegacy;   // 유니티 기본 UI Text
#if TMP_PRESENT
    public TextMeshProUGUI timerTextTMP; // TMP 텍스트 (TMP 패키지 있을 때만 사용)
#endif

    private float timeLeft = 0f;   // 남은 시간
    private bool isRunning = false; // 타이머가 작동 중인지 여부

    // 외부(다른 스크립트)에서 타이머를 시작할 때 호출하는 함수
    public void StartTimer(float seconds)
    {
        timeLeft = Mathf.Max(0, seconds); // 시간이 0보다 작아지지 않게 안전하게 설정
        isRunning = true;                 // 타이머 작동 시작
        UpdateUI();                       // 시작하자마자 UI 갱신
    }

    // 타이머를 중지하고 싶을 때 사용하는 함수 (선택 기능)
    public void StopTimer()
    {
        isRunning = false; // Update()에서 더 이상 감소하지 않게
    }

    void Update()
    {
        if (!isRunning) return; // 타이머가 꺼져 있으면 코드 실행 안 함

        timeLeft -= Time.deltaTime; // 매 프레임마다 흐른 시간만큼 감소

        if (timeLeft <= 0f) // 시간이 끝났으면
        {
            timeLeft = 0f;  // 0보다 떨어지지 않게 고정
            isRunning = false; // 타이머 중지
            OnTimerEnd(); // 타이머 종료 시 실행될 함수 호출
        }

        UpdateUI(); // 매 프레임 UI 업데이트
    }

    // UI Text에 남은 시간을 실제로 화면에 표시하는 부분
    void UpdateUI()
    {
        // 남은 시간을 정수(초) 단위로 표시
        string display = Mathf.CeilToInt(timeLeft).ToString();

#if TMP_PRESENT
        // TMP가 체크되어 있고 텍스트가 할당되어 있으면 TMP 사용
        if (useTMP && timerTextTMP != null) 
            timerTextTMP.text = display;
        else
#endif
        // 아니면 기본 Text 사용
        if (timerTextLegacy != null)
            timerTextLegacy.text = display;
    }

    // 타이머가 0이 되었을 때 자동으로 호출되는 함수
    // 끝났을 때 나오는 효과음 재생 등 추가 기능 구현 가능
    void OnTimerEnd()
    {
        Debug.Log("Timer End");
    }

    // 외부에서 남은 시간을 수동으로 설정해서 바로 보이게 하고 싶을 때 사용
    public void SetTimer(float seconds)
    {
        timeLeft = Mathf.Max(0, seconds);
        isRunning = true;
        UpdateUI();
    }
}
