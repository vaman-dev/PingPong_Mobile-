using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BallHitSoundSubscriber : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("Rally Racket Hit Sounds")]
    [SerializeField] private AudioClip[] rallyHitClips;

    [Header("Table Hit Sounds")]
    [SerializeField] private AudioClip[] tableHitClips;

    [Header("Volume")]
    [SerializeField] private float rallyHitVolume = 1f;
    [SerializeField] private float tableHitVolume = 0.75f;

    [Header("Pitch Randomness")]
    [SerializeField] private float minPitch = 0.95f;
    [SerializeField] private float maxPitch = 1.08f;

    [Header("Anti-Spam")]
    [SerializeField] private float racketHitCooldown = 0.05f;
    [SerializeField] private float tableHitCooldown = 0.08f;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

    private float lastRacketHitSoundTime = -999f;
    private float lastTableHitSoundTime = -999f;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
        audioSource.dopplerLevel = 0f;
    }

    private void OnEnable()
    {
        GameEvents.OnRacketHitBall += HandleRacketHitBall;
        GameEvents.OnBallHitTable += HandleBallHitTable;
    }

    private void OnDisable()
    {
        GameEvents.OnRacketHitBall -= HandleRacketHitBall;
        GameEvents.OnBallHitTable -= HandleBallHitTable;
    }

    private void HandleRacketHitBall(RacketHitData hitData)
    {
        if (hitData.HitType != RacketHitType.Rally)
            return;

        if (Time.time - lastRacketHitSoundTime < racketHitCooldown)
            return;

        PlaySound(
            rallyHitClips,
            hitData.ContactPoint,
            rallyHitVolume
        );

        lastRacketHitSoundTime = Time.time;

        DebugLog("[BallHitSoundSubscriber] Rally racket hit sound played.");
    }

    private void HandleBallHitTable(Vector3 contactPoint)
    {
        if (Time.time - lastTableHitSoundTime < tableHitCooldown)
            return;

        PlaySound(
            tableHitClips,
            contactPoint,
            tableHitVolume
        );

        lastTableHitSoundTime = Time.time;

        DebugLog("[BallHitSoundSubscriber] Table hit sound played.");
    }

    private void PlaySound(AudioClip[] clips, Vector3 position, float volume)
    {
        AudioClip clip = GetRandomClip(clips);

        if (clip == null)
        {
            DebugLog("[BallHitSoundSubscriber] Missing audio clip.");
            return;
        }

        audioSource.transform.position = position;
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(clip, volume);
    }

    private AudioClip GetRandomClip(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0)
            return null;

        return clips[Random.Range(0, clips.Length)];
    }

    private void DebugLog(string message)
    {
        if (showDebugLogs)
            Debug.Log(message, this);
    }
}