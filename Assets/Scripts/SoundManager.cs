using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class SoundManager : MonoBehaviour
{
    /*
    [Header("Sound Clip")]
    public AudioClip healSound;

    public AudioClip BugHitSound;

    public AudioClip BossHitSound;

    public AudioClip PlayerTakeDmg;

    public AudioClip PlayerShootSound;// 총소리 클립

    public AudioClip SpreadShootSound; // 스프레드 모드 총소리 클립

    [Header("Sound Volume")]
    public float BugHitSoundVolume = 1.7f;
    public float healSoundVolume = 1.0f;
    public float BossHitSoundVolume = 1.5f;


    public void SoundAtPlayer(AudioClip clip, float volume = 1.0f)
    {
        if (clip == null) return;

        // "Player" 태그를 가진 오브젝트를 찾습니다.
        GameObject player = GameObject.FindWithTag("Player");

        Vector3 spawnPos = (player != null) ? player.transform.position : Camera.main.transform.position;

        AudioSource.PlayClipAtPoint(clip, spawnPos, volume);

    }
    public AudioSource audioSource; // 재생용 AudioSource

    public void PlayerGunSound(AudioClip clip, float volume = 0.25f)
    {

        {
            audioSource.PlayOneShot(SpreadShootSound, volume);     //스프레드 모드 소리, 볼륨 값
        }
    }
    public void PlayerSpreadSound(AudioClip clip, float volume = 0.25f)
    {

        {
            audioSource.PlayOneShot(SpreadShootSound, volume);     //스프레드 모드 소리, 볼륨 값
        }
    }
    */
    // BGM -------------------------------------------------------------------------------------------------------------------------------------------



    public static SoundManager Instance;

    [Header("BGM Settings")]
    public AudioClip stageBGM;
    public AudioClip stageBossBGM;
    public AudioClip stageClearBGM; // [추가] 클리어 BGM 변수 추가

    [Range(0f, 1f)] public float bgmVolume = 0.25f;



    private AudioSource bgmPlayer;
    private Coroutine bgmFadeRoutine;
    private Coroutine bossCheckRoutine; // 코루틴 제어를 위한 변수 추가

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // BGM 전용 오디오 소스 설정
            bgmPlayer = GetComponent<AudioSource>();
            if (bgmPlayer == null) bgmPlayer = gameObject.AddComponent<AudioSource>();

            bgmPlayer.loop = true;
            bgmPlayer.spatialBlend = 0f; // 2D 설정
            bgmPlayer.volume = bgmVolume;

            PlayBGM(stageBGM);
            StartBossMonitor();
            //StartCoroutine(WatchBossSpawn());
        }
        else
        {
            // 새로운 씬의 곡 정보를 기존 인스턴스에 업데이트
            Instance.stageBGM = this.stageBGM;
            Instance.stageBossBGM = this.stageBossBGM;
            Instance.stageClearBGM = this.stageClearBGM; // [추가] 클리어 BGM도 업데이트

            // 스테이지 배경음악 재생
            Instance.PlayBGM(this.stageBGM);

            // [수정 2] 새로운 스테이지에 왔으므로 보스 감시 로직을 재시작
            Instance.StartBossMonitor();

            Destroy(gameObject);
        }
    }
    // [수정 3] 외부(혹은 내부)에서 호출 가능한 보스 감시 시작 메서드
    public void StartBossMonitor()
    {
        // 이미 돌고 있는 감시 코루틴이 있다면 정지 (중복 실행 방지)
        if (bossCheckRoutine != null)
        {
            StopCoroutine(bossCheckRoutine);
        }

        // 새로운 감시 코루틴 시작
        bossCheckRoutine = StartCoroutine(WatchBossSpawn());
    }

    // 보스 스폰 감시 로직
    IEnumerator WatchBossSpawn()
    {
        // 보스가 나올 때까지 대기
        while (BossManager.Instance == null || !BossManager.Instance.bossSpawned)
        {
            yield return new WaitForSeconds(0.5f);
        }

        // 보스 등장 시 페이드 없이 즉시 재생
        PlayBGM(stageBossBGM);
    }

    // 페이드 없는 즉시 재생 메서드
    public void PlayBGM(AudioClip clip)
    {
        // 클립이 없거나 이미 해당 클립이 재생 중이면 중단
        if (clip == null || (bgmPlayer.clip == clip && bgmPlayer.isPlaying)) return;

        // 즉시 정지 후 새로운 클립으로 교체 재생
        bgmPlayer.Stop();
        bgmPlayer.clip = clip;
        bgmPlayer.volume = bgmVolume; // 설정된 볼륨으로 즉시 고정
        bgmPlayer.Play();
    }
    public void PlayClearBGM()
    {
        if (stageClearBGM == null) return;

        bgmPlayer.Stop(); // 보스 음악 끄기
        bgmPlayer.clip = stageClearBGM;
        bgmPlayer.loop = false; // [중요] 클리어 음악은 반복 재생 끄기
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.Play();
    }

    // 효과음 재생용 메서드 (PlayOneShot 활용)
    public void PlayEffect(AudioClip clip, float volume = 1.0f)
    {
        if (clip != null && bgmPlayer != null)
        {
            bgmPlayer.PlayOneShot(clip, volume);
        }
    }

}

