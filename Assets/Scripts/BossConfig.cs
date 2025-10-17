using UnityEngine;

public enum BossPatternType {
    Straight,
    DiagonalRandom,
    Circle,
    Mixed,
    PaperShot,       // 과제
    BottleThrow,     // 팀플 빌런
    MidtermBurst,    // 중간고사
    FinalRing,       // 기말고사
    ProfessorPhase   // 교수님
}

[System.Serializable]
public struct BossPhase {
    public string name;              // UI 표시용
    public BossPatternType pattern;  // 패턴 타입
    public float startAtSeconds;     // 이 시간 이후 적용
    public float fireInterval;       // 발사 간격
    public float projectileSpeed;    // 탄 속도
    public int volleyCount;          // 탄 수
}

[CreateAssetMenu(fileName = "BossConfig", menuName = "Game/BossConfig")]
public class BossConfig : ScriptableObject {
    public GameObject bossPrefab;
    public int maxHP = 50;
    public BossPhase[] phases;
}
