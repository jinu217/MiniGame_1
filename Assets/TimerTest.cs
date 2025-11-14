using UnityEngine;

public class TimerTest : MonoBehaviour
{
    public TimerUI timerUI; // Inspector에서 TimerUI 연결

    void Start()
    {
        // 게임 시작하자마자 30초 타이머 시작
        timerUI.StartTimer(30f);
    }
}

