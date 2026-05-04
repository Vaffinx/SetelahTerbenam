using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("AudioSource")]
    [SerializeField] AudioSource BGM;
    [SerializeField] AudioSource SFX;
    [SerializeField] AudioSource RandomPitchSFX;

    [Header("AudioClip")]
    public AudioClip bgm;
    public AudioClip jumpSFX;
    public AudioClip[] WalkSFX;
    public AudioClip DieSFX;

    void Start()    
    {
        BGM.clip = bgm;
        BGM.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFX.PlayOneShot(clip);
    }

    public void StopSFX(AudioClip clip)
    {
        SFX.Stop();
    }

    public void jump()
    {
        SFX.PlayOneShot(jumpSFX);
    }

    public void walk()
    {
        AudioClip clip = WalkSFX[Random.Range(0, WalkSFX.Length)];
        RandomPitchSFX.pitch = Random.Range(1f, 1.1f);
        RandomPitchSFX.PlayOneShot(clip);
        Debug.Log("pitch: " + RandomPitchSFX.pitch);
    }

    public void die()
    {
        SFX.PlayOneShot(DieSFX);
    }
}
