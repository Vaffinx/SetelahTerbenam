using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("AudioSource")]
    [SerializeField] AudioSource BGM;
    [SerializeField] AudioSource SFX;

    [Header("AudioClip")]
    public AudioClip bgm;
    public AudioClip jumpSFX;
    public AudioClip WalkSFX;
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

    public void jump()
    {
        SFX.PlayOneShot(jumpSFX);
    }

    public void walk()
    {
        SFX.PlayOneShot(WalkSFX);
    }

    public void die()
    {
        SFX.PlayOneShot(DieSFX);
    }
}
