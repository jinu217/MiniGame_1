using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpreadFireController : MonoBehaviour
{
    [Header("References")]
    public AutoShooter shooter;       // AutoShooter 연결
    public Transform baseFirePoint;   // 기준 FirePoint (정면)
    public Button spreadButton;       // UI 버튼 (없으면 null 가능)
    public Player player;

    [Header("Spread Settings")]
    public float spreadAngle = 45f;   // 좌우 각도(±)
    public float spreadDuration = 5f; // 유지 시간
    public float lateralOffset = 0.15f;// 좌우 포인트 가로 오프셋(겹침 방지)

    Transform leftPoint, rightPoint;
    bool isActive = false;

    void Start()
    {
        if (spreadButton != null)
            spreadButton.onClick.AddListener(Activate);
    }

    void Update()
    {
        // skillPoint가 가득 차면 버튼 자동 활성화
        if (player != null && spreadButton != null)
        {
            if (player.skillPoint >= player.maxSkillPoint && !isActive)
                spreadButton.interactable = true;
            else
                spreadButton.interactable = false;
        }
    }

    public void Activate()
    {
        if (!isActive && player != null && player.skillPoint >= player.maxSkillPoint)
        {
            StartCoroutine(SpreadRoutine());
        }
    }

    IEnumerator SpreadRoutine()
    {
        isActive = true;

        if (player != null)
            player.skillPoint = player.skillPoint - player.maxSkillPoint;

        if (spreadButton != null)
            spreadButton.interactable = false;

        // 좌/우 FirePoint 생성 (부모 동일, 위치/회전은 base 복제)
        leftPoint = Instantiate(baseFirePoint, baseFirePoint.parent);
        rightPoint = Instantiate(baseFirePoint, baseFirePoint.parent);
        leftPoint.name = "FirePoint_Left";
        rightPoint.name = "FirePoint_Right";

        // 각도 회전 적용
        leftPoint.localRotation = baseFirePoint.localRotation * Quaternion.Euler(0, -spreadAngle, 0);
        rightPoint.localRotation = baseFirePoint.localRotation * Quaternion.Euler(0, spreadAngle, 0);

        // 살짝 좌우로 이동시켜 시각적/물리적 겹침 방지
        leftPoint.localPosition += new Vector3(-lateralOffset, 0f, 0f);
        rightPoint.localPosition += new Vector3(lateralOffset, 0f, 0f);

        // AutoShooter에 3개 발사점 등록 (정면 + 좌 + 우)
        shooter.firePoints = new Transform[] { baseFirePoint, leftPoint, rightPoint };


        // 유지 시간
        yield return new WaitForSeconds(spreadDuration);

        // 좌/우 삭제 + 원복
        if (leftPoint) Destroy(leftPoint.gameObject);
        if (rightPoint) Destroy(rightPoint.gameObject);
        shooter.firePoints = new Transform[] { baseFirePoint };

        isActive = false;
    }

}

