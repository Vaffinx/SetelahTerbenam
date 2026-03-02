using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerMovment : MonoBehaviour
{
    public float speed;
    public float jumpForce;

    private Rigidbody2D rb;
    private bool isGrounded;
    private Animator anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // untk Pasang komponen Rigidbody2D ke pemain
    }

    // Update is called once per frame
    void Update()
    {
        float move = Input.GetAxis("Horizontal"); // Mendapatkan input horizontal (A/D atau panah kiri/kanan)
        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y); // Menggerakkan pemain secara horizontal

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (move > 0)
        {
            sr.flipX = false; // Menghadap ke kanan
        }
        else if (move < 0)
        {
            sr.flipX = true; // Menghadap ke kiri
        }
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); // Melompat 
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    { if (collision.collider.CompareTag("Ground"))

        isGrounded = true; // Pemain menyentuh tanah

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))

            isGrounded = false; // Pemain menyentuh tanah

    }




}