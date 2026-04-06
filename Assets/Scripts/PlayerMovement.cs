using System.Runtime.CompilerServices;
using System.Collections;
using UnityEngine;
using TMPro;


public class PlayerMovment : MonoBehaviour
{
    public float speed;
    public float jumpForce;

    private Rigidbody2D rb;
    private Collider2D collider;
    private Animator anim;
    private SpriteRenderer sr;
    private Vector3 worldPos;


   [SerializeField] private LayerMask groundLayer;

    private bool isJumping = false;
    private bool isGrounded = false;
    private bool isHiding = false;
    private bool isHidingSpot = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // untk Pasang komponen Rigidbody2D ke pemain
        anim = GetComponent<Animator>(); // untuk Pasang komponen Animator ke pemain
        collider = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Grounded(); // Panggil fungsi Grounded tiap frame!
        Hiding();
        FlashlightOnOff();
        FlashLightFollowMouse();

        float move = Input.GetAxis("Horizontal"); // Mendapatkan input horizontal (A/D atau panah kiri/kanan)

        if (!isHiding)
        {
            if (move != 0 || transform.parent == null)
            {
                rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y); // Menggerakkan pemain secara horizontal
            }
            else if (move == 0 && transform.parent != null)
            {
                Rigidbody2D platformRb = transform.parent.GetComponent<Rigidbody2D>();
                if (platformRb != null)
                {
                    rb.linearVelocity = new Vector2(platformRb.linearVelocity.x, rb.linearVelocity.y); // Menggerakkan pemain secara horizontal
                }
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Berhenti saat sembunyi
        }

        if (isFlashOn)
        {
            if (worldPos.x > transform.position.x)
            {
                sr.flipX = false; // Menghadap ke kanan
            }
            else if (worldPos.x < transform.position.x)
            {
                sr.flipX = true; // Menghadap ke kiri
            }
        } 
        else
        {
            if(move > 0)
            {
                sr.flipX = false; // Menghadap ke kanan
            }
            else if(move < 0)
            {
                sr.flipX = true; // Menghadap ke kiri
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isJumping = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); // Melompat 
        }

        //animation
        if (move != 0 && isGrounded)
        {
            anim.SetBool("Walk", true);
        }
        else
        {
            anim.SetBool("Walk", false);
        }
        
        // Cek apakah karakter sedang tidak berada di tanah (sedang melompat atau jatuh)
        if (!isGrounded && isJumping)
        {
            anim.SetBool("Jump", true);
        }
        else
        {
            anim.SetBool("Jump", false);
        }
        

    }

    private void Grounded()
    {
        float rayLength = collider.bounds.extents.y + 0.1f;
        
        RaycastHit2D hit = Physics2D.Raycast(collider.bounds.center, Vector2.down, rayLength, groundLayer);
        
        if (hit.collider != null)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        Debug.DrawRay(collider.bounds.center, Vector2.down * rayLength, Color.red);
    }

    private void Hiding()
    {
        if (isHidingSpot && Input.GetKeyDown("e"))
        {
            isHiding = !isHiding;

            if(isHiding)    
            {
                gameObject.layer = 7;
                sr.color = new Color(1f, 1f, 1f, 0.5f);
                Physics2D.IgnoreLayerCollision(7, 8, true);
                sr.sortingOrder = 1;
                anim.SetBool("Walk", false);
                anim.SetBool("Jump", false);
                Debug.Log("Player is hiding!");
            }
            if(!isHiding)
            {
                gameObject.layer = 6;
                sr.color = new Color(1f, 1f, 1f, 1f);
                Physics2D.IgnoreLayerCollision(7, 8, false);
                sr.sortingOrder = 4;    
                anim.SetBool("Walk", true);
                Debug.Log("Player is not hiding!");
            }
        }
    }

    //hiding
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HidingSpot"))
        {
            isHidingSpot = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("HidingSpot"))
        {
            if (!isHiding)
            {
                isHidingSpot = false;
            }
        }   
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Obstacle"))
        {
            Debug.Log("Player is Hit!");
        }
    }



    // Flashlight
    public GameObject flashlight;
    public float FlashDuration;
    public float FlashCooldown;
    public TextMeshProUGUI flashlightTimerText;

    private Camera cam;
    private float flashlightTimer;
    private bool isFlashOn = false;
    private bool isCoolDown = false;

    private void FlashlightOnOff()
    {
        if (isHiding)
        {
            return;
        }
       if (Input.GetKeyDown(KeyCode.F) && !isCoolDown)
       {
            StartCoroutine(FlashlightTimer());
       }
    }

    private void FlashLightFollowMouse()
    {
        Vector3 MousePos = Input.mousePosition;
        MousePos.z = 0;
        worldPos = cam.ScreenToWorldPoint(MousePos);
        Vector2 direction = worldPos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        flashlight.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private IEnumerator FlashlightTimer()
    {
        isFlashOn = true;
        flashlight.SetActive(true);
        Debug.Log("Flashlight is on!");

        yield return new WaitForSeconds(FlashDuration);

        flashlight.SetActive(false);

        isFlashOn = false;
        Debug.Log("Flashlight is off!");
        isCoolDown = true;
        yield return new WaitForSeconds(FlashCooldown);
        isCoolDown = false;

    }
}