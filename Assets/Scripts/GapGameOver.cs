using UnityEngine;

public class GapGameOver : MonoBehaviour
{
    private PlayerMovment player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindObjectOfType<PlayerMovment>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.GameOver("Jatuh ke Jurang!");
            Debug.Log("Player is Hit by EndGap!");
        }
    }
}
