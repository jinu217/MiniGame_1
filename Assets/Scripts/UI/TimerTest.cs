using UnityEngine;

public class TimerTest : MonoBehaviour
{
    public TimerUI timerUI; // Inspector���� TimerUI ����

    void Start()
    {
        // ���� �������ڸ��� 30�� Ÿ�̸� ����
        timerUI.StartTimer(75f);
    }
}

