using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;


public class SpreadFireController : MonoBehaviour
{
    [Header("References")]
    public AutoShooter shooter;       // AutoShooter 연결
    public Transform baseFirePoint;   // 기준 FirePoint (정면)
    public Button spreadButton;       // UI 버튼 (없으면 null 가능)
    public ButtonImage buttonImage; // 인스펙터에서 할당


    [Header("Spread Settings")]
    public float spreadAngle = 30f;   // 좌우 각도(±)
    public float spreadDuration = 5f; // 유지 시간
    public float lateralOffset = 0.15f;// 좌우 포인트 가로 오프셋(겹침 방지)

    [Header("Damage Buff Settings")]
    public float spreadDamageMultiplier = 2f;   // 스프레드 중에 곱해줄 배율 (2배)
    GameManager gm;
    float originalDamageMultiplier = 1f;

    //스킬 사용 요구 포인트
    [Header("Requirement")]
    public int requiredSkillPoint_base = 1;
    public int requiredSkillPoint = 1;


    Transform leftPoint, rightPoint;

    bool isActive = false;
    public bool canClick = true;

    void Start()
    {
        // 씬 인덱스
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

            //요구스킬값 = 베이스요구값 * 스테이지 인덱스값
        requiredSkillPoint = requiredSkillPoint_base * sceneIndex;

         gm = GameManager.gameManager;

         if (spreadButton != null)
            spreadButton.onClick.AddListener(Activate);

    }
    
    //스킬 버튼 활성화
    void Update()
    {
        if (gm == null) return;

        // 유지시간 동안(isActive == true)에는 무조건 사용 불가
        bool canUseSpread = (gm.skillPoint >= requiredSkillPoint) && !isActive;

        if (spreadButton != null)
            spreadButton.interactable = canUseSpread;

        // 버튼 이미지도 같이 변경
        if (buttonImage != null)
        {
            if (canUseSpread)
                buttonImage.SetImageToSprite1();
            else
                buttonImage.SetImageToSprite2();
        }
    }

    public void Activate()
    {
        if (isActive)
            return;

        if (gm.skillPoint < requiredSkillPoint)
        {
            Debug.Log($"스프레드 모드 사용 불가: 필요 {requiredSkillPoint}, 현재 {gm.skillPoint}");
            return;
        }

        // 스킬 포인트 감소
        gm.skillPoint -= requiredSkillPoint;

        StartCoroutine(SpreadRoutine());
    
    }


    IEnumerator SpreadRoutine()
    {
        isActive = true;

        //AutoShooter에 스프레드 모드 시작 알림
        shooter.isSpreadMode = true;

        // 데미지 2배 적용
        if (gm != null)
        {
            originalDamageMultiplier = gm.damageMultiplier;
            gm.damageMultiplier = originalDamageMultiplier * spreadDamageMultiplier;
        }

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

        //데미지 복구
        if (gm != null)
        {
            gm.damageMultiplier = originalDamageMultiplier;
        }

        //AutoShooter에 스프레드 모드 종료 알림
        shooter.isSpreadMode = false;

        if (gm.skillPoint < requiredSkillPoint)
        {
            buttonImage.SetImageToSprite2();
        }
        else
        {
            buttonImage.SetImageToSprite1();
        }

        isActive = false;
    }

}
