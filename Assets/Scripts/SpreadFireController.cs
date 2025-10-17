using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpreadFireController : MonoBehaviour
{
    [Header("References")]
    public AutoShooter shooter;       // AutoShooter ����
    public Transform baseFirePoint;   // ���� FirePoint (����)
    public Button spreadButton;       // UI ��ư (������ null ����)
    public Player player;

    [Header("Spread Settings")]
    public float spreadAngle = 45f;   // �¿� ����(��)
    public float spreadDuration = 5f; // ���� �ð�
    public float lateralOffset = 0.15f;// �¿� ����Ʈ ���� ������(��ħ ����)

    Transform leftPoint, rightPoint;
    bool isActive = false;

    void Start()
    {
        if (spreadButton != null)
            spreadButton.onClick.AddListener(Activate);
    }

    void Update()
    {
        // skillPoint�� ���� ���� ��ư �ڵ� Ȱ��ȭ
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

        // ��/�� FirePoint ���� (�θ� ����, ��ġ/ȸ���� base ����)
        leftPoint = Instantiate(baseFirePoint, baseFirePoint.parent);
        rightPoint = Instantiate(baseFirePoint, baseFirePoint.parent);
        leftPoint.name = "FirePoint_Left";
        rightPoint.name = "FirePoint_Right";

        // ���� ȸ�� ����
        leftPoint.localRotation = baseFirePoint.localRotation * Quaternion.Euler(0, -spreadAngle, 0);
        rightPoint.localRotation = baseFirePoint.localRotation * Quaternion.Euler(0, spreadAngle, 0);

        // ��¦ �¿�� �̵����� �ð���/������ ��ħ ����
        leftPoint.localPosition += new Vector3(-lateralOffset, 0f, 0f);
        rightPoint.localPosition += new Vector3(lateralOffset, 0f, 0f);

        // AutoShooter�� 3�� �߻��� ��� (���� + �� + ��)
        shooter.firePoints = new Transform[] { baseFirePoint, leftPoint, rightPoint };


        // ���� �ð�
        yield return new WaitForSeconds(spreadDuration);

        // ��/�� ���� + ����
        if (leftPoint) Destroy(leftPoint.gameObject);
        if (rightPoint) Destroy(rightPoint.gameObject);
        shooter.firePoints = new Transform[] { baseFirePoint };

        isActive = false;
    }

}

