using System.Runtime.CompilerServices;
using System.Collections;
using UnityEngine;
using TMPro;


public class PlayerMovment : MonoBehaviour
{
    [Header("Movement Speed")]
    public float speed;
    public float jumpForce;

    [Header("Flashlight Settings")]
    public GameObject flashlight;
    public float FlashDuration;
    public float FlashCooldown;

    [Header("Game Over Settings")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;

    private AudioManager audioManager;
    private Rigidbody2D rb;
    private Collider2D collider;
    private Animator anim;
    private SpriteRenderer sr;
    private Vector3 worldPos;
    private Camera cam;
    private float flashlightTimer;

    [Header("Pause Menu")]
    public GameObject pauseMenu;
    private bool isPaused = false;

    [Header("Flashlight Angle Offset")]
    public float flashlightAngleOffset = -30f;

    [Header("Ground Layer")]
    public LayerMask groundLayer;

    private bool isJumping = false;
    public bool isMoving = false;
    public bool isGrounded = false;
    public bool isHiding = false;
    private bool isHidingSpot = false;
    private bool isFlashOn = false;
    private bool isCoolDown = false;
    private bool isCursorVisible = false;
    private int hidingSpotsInRange = 0; // Menghitung berapa spot sembunyi yang tumpang tindih

    void Awake()
    {
        audioManager = FindAnyObjectByType<AudioManager>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // untk Pasang komponen Rigidbody2D ke pemain
        anim = GetComponent<Animator>(); // untuk Pasang komponen Animator ke pemain
        collider = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        cam = Camera.main;
        Cursor.visible = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        Grounded(); // Panggil fungsi Grounded tiap frame!
        Hiding();
        FlashlightOnOff();
        FlashLightFollowMouse();
        showCursor();
        pause();
        Movement();

    }

    public void Movement()
    {
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
            isMoving = (move != 0);
        }
        else
        {
            isMoving = false;
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
            audioManager.PlaySFX(audioManager.jumpSFX);
        }

        //animation
        if (move != 0 && isGrounded && !isHiding)
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

    private void pause()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            showCursor();
            isPaused = true;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isPaused)
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            showCursor();
            isPaused = false;
        }
    }

    private void showCursor()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote) && !isCursorVisible)
        {
            Cursor.visible = true;
            isCursorVisible = true;
        }
        else if (Input.GetKeyDown(KeyCode.BackQuote) && isCursorVisible)
        {
            Cursor.visible = false;
            isCursorVisible = false;
        }
    }

    public void DisableControl()
    {
        rb.linearVelocity = Vector2.zero;
        anim.SetBool("Walk", false);
        anim.SetBool("Jump", false);
        speed = 0;
        jumpForce = 0;
    }

    public void GameOver(string Teks)
    {
        gameOverPanel.SetActive(true);
        audioManager.PlaySFX(audioManager.DieSFX);
        gameOverText.text = "Karaktermu Terbunuh Karena: " + Teks;

        flashlight.SetActive(false);
        DisableControl();
        Cursor.visible = true;
        isCursorVisible = true;
    }

    public void EnableControl()
    {
        speed = 5;
        jumpForce = 6;
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
                flashlight.SetActive(false);
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
            hidingSpotsInRange++; // Tambah 1 jika masuk area sembunyi
            isHidingSpot = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("HidingSpot"))
        {
            hidingSpotsInRange--; // Kurangi 1 jika keluar
            
            // Kalau sudah tidak ada spot yang tumpang tindih sama sekali
            if (hidingSpotsInRange <= 0)
            {
                hidingSpotsInRange = 0; // Pastikan tidak tembus ke minus
                if (!isHiding)
                {
                    isHidingSpot = false; // Baru matikan isHidingSpot
                }
            }
        }   
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

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
        if (cam == null) cam = Camera.main;

        // Ambil posisi mouse di layar
        Vector3 mouseScreenPos = Input.mousePosition;

        // Cek apakah mouse masih di dalam area kamera (supaya tidak error)
        Rect camRect = cam.pixelRect;
        if (!camRect.Contains(mouseScreenPos))
            return;

        // Ubah posisi mouse dari layar ke dunia game
        // Z harus = jarak kamera ke player (untuk kamera 2D, pakai nilai absolut Z kamera)
        mouseScreenPos.z = Mathf.Abs(cam.transform.position.z);
        worldPos = cam.ScreenToWorldPoint(mouseScreenPos);

        // Pastikan flashlight tetap di posisi player (supaya tidak geser)
        flashlight.transform.position = transform.position;

        // Hitung arah dari player ke mouse
        Vector2 direction = (Vector2)worldPos - (Vector2)transform.position;

        // Hitung sudut + offset (atur offset di Inspector supaya pas)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + flashlightAngleOffset;
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