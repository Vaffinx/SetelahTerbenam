using UnityEngine;

public class PlayerFootStep : MonoBehaviour
{
    private AudioManager audioManager;
    private PlayerMovment playerMovement;
    private bool PlayingFootStep = false;
    private float footstepSpeed = 0.5f;

    void Awake()
    {
        audioManager = FindAnyObjectByType<AudioManager>();
        playerMovement = FindAnyObjectByType<PlayerMovment>();
    }

    void Update()
    {
        if (playerMovement.isMoving && playerMovement.isGrounded && !playerMovement.isHiding)
        {
            if (!PlayingFootStep)
            {
                StartFootStep();
            }
        }
        else
        {
            if (PlayingFootStep)
            {
                StopFootStep();
            }
        }
    }

    void StartFootStep()
    {
        PlayingFootStep = true;
        InvokeRepeating(nameof(PlayFootStep), 0f, footstepSpeed);
    }

    void StopFootStep()
    {
        PlayingFootStep = false;
        CancelInvoke(nameof(PlayFootStep));
    }

    void PlayFootStep()
    {
        audioManager.walk();
    }
}
