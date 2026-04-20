using UnityEngine;
using System.Collections;

public class PatrollingEnemy : MonoBehaviour
{
    [Header("Walking Points")]
    public GameObject Point1;
    public GameObject Point2;

    [Header("Components")]
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
    private Transform CurrentPoint;
    private PlayerMovment player;
    private float DefaultSpeed;

    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float stunDuration = 3f;
    private bool isStunned = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       rb = GetComponent<Rigidbody2D>();
       anim = GetComponent<Animator>();
       sr = GetComponent<SpriteRenderer>();
       CurrentPoint = Point1.transform;
       anim.SetBool("Walk", true);
       player = FindObjectOfType<PlayerMovment>();
       DefaultSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        //movement
        Vector2 targetPoint = CurrentPoint.position - transform.position;
        if(CurrentPoint == Point2.transform)
        {
            rb.linearVelocity = new Vector2(speed, 0);
            sr.flipX = false;
        }
        else
        {
            rb.linearVelocity = new Vector2(-speed, 0);
            sr.flipX = true;
        }


        //change point
        if(Vector2.Distance(transform.position, CurrentPoint.position) < 1f && CurrentPoint == Point2.transform)
        {
            CurrentPoint = Point1.transform;
        }
        if(Vector2.Distance(transform.position, CurrentPoint.position) < 1f && CurrentPoint == Point1.transform)
        {
            CurrentPoint = Point2.transform;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Player is caught!");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Point1.transform.position, 0.5f);
        Gizmos.DrawWireSphere(Point2.transform.position, 0.5f);
        Gizmos.DrawLine(Point1.transform.position, Point2.transform.position);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Flash"))
        {
            Debug.Log("Enemy is hit!");
            StartCoroutine(StunEnemy());
            isStunned = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.GameOver("Hantu!");
            Debug.Log("Player is caught!");
        }
    }

    private IEnumerator StunEnemy()
    {

        if (isStunned)
        {
            speed = 0f;
            yield return new WaitForSeconds(stunDuration);
            isStunned = false;
            speed = DefaultSpeed;
        }
    }
}
