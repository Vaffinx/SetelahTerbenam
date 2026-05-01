using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private PlayerMovment player;
    private Collider2D collider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindAnyObjectByType<PlayerMovment>(); 
        collider = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            player.GameOver("Jebakan!");
            Debug.Log("Player is hit by obstacle!");
        }
    }
}
