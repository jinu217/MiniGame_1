using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AmmoSpawnerManual : MonoBehaviour
{
    public Image targetImage;
    public float iconUpdateTerm = 0.2f;

    private GameManager gameManager;
    private int lastSkillPoint = -1;
    private int targetSkillPoint = -1;
    private Coroutine updateRoutine;

    public Sprite[] barSprites;

    void Start()
    {
        gameManager = GameManager.gameManager;
        if (gameManager == null)
        {
            Debug.LogError("GameManager 오브젝트를 찾을 수 없습니다!");
            enabled = false;
            return;
        }
        gameManager.skillPoint = 2; // 시작 시 스킬 포인트를 2로 고정
        lastSkillPoint = gameManager.skillPoint;
        targetSkillPoint = lastSkillPoint;
        SetImageToSprite(lastSkillPoint);
    }

    void Update()
    {
        if (gameManager == null) return;
        if (gameManager.skillPoint != targetSkillPoint)
        {
            targetSkillPoint = gameManager.skillPoint;
            if (updateRoutine == null)
                updateRoutine = StartCoroutine(UpdateAmmoIconsSmooth());
        }
    }

    public void SetImageToSprite(int a)
    {
        if (targetImage == null || barSprites == null || barSprites.Length == 0)
            return;

        int idx = Mathf.Clamp(a, 0, barSprites.Length - 1);
        if (barSprites[idx] != null)
            targetImage.sprite = barSprites[idx];
    }

    IEnumerator UpdateAmmoIconsSmooth()
    {
        while (lastSkillPoint != targetSkillPoint)
        {
            if (lastSkillPoint < targetSkillPoint)
                lastSkillPoint++;
            else if (lastSkillPoint > targetSkillPoint)
                lastSkillPoint--;

            SetImageToSprite(lastSkillPoint);
            yield return new WaitForSeconds(iconUpdateTerm);
        }
        updateRoutine = null;
    }
}