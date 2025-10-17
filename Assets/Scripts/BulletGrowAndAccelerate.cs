using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class BulletGrowAndAccelerate : MonoBehaviour
{
    // 설정 값 (Boss에서 Configure로 주입)
    float startScale = 0.25f; // 시작 배율
    float endScale   = 1.0f;  // 최종 배율
    float growDur    = 0.9f;  // 성장 시간
    float accel      = 14f;   // 초당 가속량
    float lifeMax    = 6f;    // 최대 생존 시간

    Rigidbody rb;
    float currentS = 1f;      // 현재 배율 (LateUpdate에서 강제 적용)
    Coroutine growRoutine;

    // Boss에서 호출: 파라미터 세팅
    public void Configure(float startScale, float endScale, float growDuration, float accelPerSecond, float maxLifetime = 6f)
    {
        this.startScale = Mathf.Max(0.01f, startScale);
        this.endScale   = Mathf.Max(this.startScale + 0.001f, endScale); // 최소 증가 보장
        this.growDur    = Mathf.Max(0.01f, growDuration);
        this.accel      = Mathf.Max(0f,    accelPerSecond);
        this.lifeMax    = Mathf.Max(0.1f,  maxLifetime);

        // 시작 배율 즉시 반영
        currentS = this.startScale;
        transform.localScale = Vector3.one * currentS;

        // 수명 타이머 리셋
        CancelInvoke(nameof(DestroySelf));
        if (lifeMax > 0f) Invoke(nameof(DestroySelf), lifeMax);
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Configure 이전에 AddComponent만 된 경우도 대비
        currentS = startScale;
        transform.localScale = Vector3.one * currentS;

        if (lifeMax > 0f) Invoke(nameof(DestroySelf), lifeMax);
    }

    // Boss에서 성장 시작시 호출
    public void BeginGrow()
    {
        if (growRoutine != null) StopCoroutine(growRoutine);
        growRoutine = StartCoroutine(GrowRoutine());
    }

    IEnumerator GrowRoutine()
    {
        float t = 0f;
        while (t < growDur)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / growDur);
            currentS = Mathf.Lerp(startScale, endScale, k); // 0.25 → 5.0
            yield return null;
        }
        currentS = endScale; // 마지막 값 보정
    }

    void Update()
    {
        // 진행 방향으로 가속 (velocity 사용)
        if (rb && accel > 0f)
        {
            Vector3 v = rb.linearVelocity;
            if (v.sqrMagnitude > 1e-6f)
                rb.linearVelocity = v + v.normalized * accel * Time.deltaTime;
        }
    }

    void LateUpdate()
    {
        // ★ 다른 스크립트가 scale을 덮어써도 여기서 최종 배율로 다시 적용
        transform.localScale = Vector3.one * currentS;
    }

    void DestroySelf()
    {
        if (this) Destroy(gameObject);
    }
}
