using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    public float speed;
    public float moveDistance;
    private Vector2 startPos;

    private bool isPlayerOnPlatform = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        startPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PlatformHandler();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnPlatform = true;
            Debug.Log("Player is on the platform");
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnPlatform = false;
            Debug.Log("Player is not on the platform");
            collision.transform.SetParent(null);
        }
    }

    private void PlatformHandler()
    {
        if(isPlayerOnPlatform)
        {
            float distance = Mathf.Abs(transform.position.x - startPos.x);
            if(distance < moveDistance)
            {
                rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
}
