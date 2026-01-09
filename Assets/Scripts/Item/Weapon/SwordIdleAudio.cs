using UnityEngine;

public class SwordIdleAudio : MonoBehaviour
{
    private AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        // 팩트체크: 오디오 소스 없으면 에러 뱉고 꺼짐. 방어 코드.
        if (_audioSource == null)
        {
            Debug.LogError($"[SwordIdleAudio] {gameObject.name}에 AudioSource 없다. 넣어라.");
            enabled = false;
            return;
        }
    }

    void Start()
    {
        PlayRandomPhase();
    }

    private void PlayRandomPhase()
    {
        if (_audioSource.clip == null) return;

        // 핵심: 0초부터 시작하는 게 아니라, 전체 길이 중 랜덤한 위치에서 시작
        _audioSource.time = Random.Range(0f, _audioSource.clip.length);
        _audioSource.Play();
    }
}