using UnityEngine;

public class PlatformMove : MonoBehaviour
{
    public GameObject Point1;
    public GameObject Point2;
    private Rigidbody2D rb;
    private Transform CurrentPoint;
    public float speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        CurrentPoint = Point1.transform;
    }

    // Update is called once per frame
    void Update()
    {
       if (CurrentPoint == Point2.transform)
       {
           rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
       }
       else
       {
           rb.linearVelocity = new Vector2(-speed, rb.linearVelocity.y);
       }

       if(Vector2.Distance(transform.position, CurrentPoint.position) < 2f && CurrentPoint == Point2.transform)
       {
           CurrentPoint = Point1.transform;
       }
       else if(Vector2.Distance(transform.position, CurrentPoint.position) < 2f && CurrentPoint == Point1.transform)
       {
           CurrentPoint = Point2.transform;
       }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Player is on the platform");
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Player is not on the platform");
            collision.transform.SetParent(null);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Point1.transform.position, 0.5f);
        Gizmos.DrawWireSphere(Point2.transform.position, 0.5f);
        Gizmos.DrawLine(Point1.transform.position, Point2.transform.position);
    }
}
