using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour
{
    public AudioClip rollClip;
    public AudioClip jumpClip;
    public AudioClip powerUpClip;

    private AudioSource audioSource;
    private AudioSource rollingAudioSource;

    private void Awake()
    {
        // Initialize the main audio source
        audioSource = GetComponent<AudioSource>();

        rollingAudioSource = gameObject.AddComponent<AudioSource>();
        rollingAudioSource.loop = true; // looping for continuous rolling sound
        rollingAudioSource.playOnAwake = false;
    }

    public void PlayRollingSound(float velocityRatio)
    {
        if (!rollingAudioSource.isPlaying)
        {
            rollingAudioSource.clip = rollClip;
            rollingAudioSource.Play();
        }

        // Adjust pitch and volume based on the ball velocity, only for the rolling sound
        rollingAudioSource.pitch = Mathf.Lerp(0.5f, 2.0f, velocityRatio);
        rollingAudioSource.volume = Mathf.Lerp(0.01f, 0.05f, velocityRatio);
    }

    public void StopRollingSound()
    {
        if (rollingAudioSource.isPlaying)
        {
            rollingAudioSource.Stop();
        }
    }

    public void PlayJumpSound(float volume = 0.1f, float pitch = 1.0f)
    {
        audioSource.PlayOneShot(jumpClip, volume);
    }

    public void PlayPowerUpSound(float volume = 0.2f, float pitch = 1.0f)
    {
        audioSource.PlayOneShot(powerUpClip, volume);
    }
}
