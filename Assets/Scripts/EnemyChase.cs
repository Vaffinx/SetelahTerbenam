using System.Collections;
using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public GameObject Player;
    private float speed = 5f;
    private bool isStunned = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isStunned) return;

    }

    public void StunEnemy(float duration)
    {
        if (!isStunned)
        {
            StartCoroutine(StunRoutine(duration));
        }
    }

    private IEnumerator StunRoutine(float duration)
    {
        isStunned = true;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;
        
        yield return new WaitForSeconds(duration);
        isStunned = false;    }
}
