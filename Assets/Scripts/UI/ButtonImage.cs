using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ButtonImage : MonoBehaviour
{
    [Header("Spread UI")]
    public Sprite spreadModeImage1; // 버튼 이미지 추가
    public Sprite spreadModeImage2; // 눌린 버튼 이미지

    public Image targetImage; // 실제 이미지를 바꿀 UI Image 컴포넌트

    // 원하는 스프라이트로 이미지 변경
    public void SetImageToSprite1()
    {
        if (targetImage != null && spreadModeImage1 != null)
            targetImage.sprite = spreadModeImage1;
    }

    public void SetImageToSprite2()
    {
        if (targetImage != null && spreadModeImage2 != null)
            targetImage.sprite = spreadModeImage2;
    }

    public void Start()
    {
        // 초기 이미지 설정 (원하는 스프라이트로 설정)
        SetImageToSprite1();
    }
}
