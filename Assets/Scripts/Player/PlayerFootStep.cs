using UnityEngine;
using System.Collections;

public class PlayerFootStep : MonoBehaviour
{
    private AudioManager audioManager;
    private PlayerMovment playerMovement;

    void Awake()
    {
        audioManager = FindAnyObjectByType<AudioManager>();
        playerMovement = FindAnyObjectByType<PlayerMovment>();
    }

    void Start()
    {
        StartCoroutine(PlayerFootstep());
    }

    IEnumerator PlayerFootstep()
    {
        while (true)
        {
            if (playerMovement.isMoving && playerMovement.isGrounded && !playerMovement.isHiding)
            {
                audioManager.PlaySFX(audioManager.WalkSFX);
                // Wait for the EXACT length of the audio clip so it never overlaps/stacks
                yield return new WaitForSeconds(audioManager.WalkSFX.length);
            }
            else
            {
                yield return null;
            }
        }
    }
}
