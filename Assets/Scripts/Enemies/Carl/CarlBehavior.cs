using UnityEngine;
using System.Collections;
using System;

public class CarlBehavior : MonoBehaviour
{

    public int health;
    public int bulletDamage;
    public float enemySpeed;

    private SpriteRenderer sprite;
    private Vector3 currentScale;

    private bool touchingPlayer = false;
    private Vector2 playerPosition;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!touchingPlayer)
        {
            Movement(); // move towards player
        }
        else if (touchingPlayer)
        {
            BackOff(); // back off of player
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            Debug.Log("i am hit");
            health = health - bulletDamage;
            Debug.Log(health);
            if (health <= 0) Destroy(gameObject); // dies at 0 hp
        }
        else if (collision.CompareTag("Player"))
        {
            // Debug.Log("i toucha da playa");
            StartCoroutine(BackingOff());
        }
    }

    IEnumerator BackingOff() // just toggles the variable
    {
        touchingPlayer = true;
        yield return new WaitForSecondsRealtime(0.4f);
        touchingPlayer = false;
    }

    private void Movement() // moves towards player
    {
        playerPosition = GameObject.Find("Player").transform.position;

        if (playerPosition.x > transform.position.x) // move right
        {
            currentScale = transform.localScale;
            currentScale.x = Mathf.Abs(transform.localScale.x) * -1;
            transform.localScale = currentScale;

            transform.position += transform.right * enemySpeed * Time.deltaTime;
            // Debug.Log("carl right");
        }
        else if (playerPosition.x < transform.position.x) // move left
        {
            currentScale = transform.localScale;
            currentScale.x = Mathf.Abs(transform.localScale.x);
            transform.localScale = currentScale;

            sprite.flipX = true;
            transform.position += transform.right * enemySpeed * Time.deltaTime * -1;
            // Debug.Log("carl left");
        }

    }
    
    private void BackOff() // back off after touching player
    {
        playerPosition = GameObject.Find("Player").transform.position;

        if (playerPosition.x > transform.position.x) // move right
        {
            currentScale = transform.localScale;
            currentScale.x = Mathf.Abs(transform.localScale.x) * -1;
            transform.localScale = currentScale;

            transform.position += transform.right * enemySpeed * Time.deltaTime * -2f;
            // Debug.Log("carl right");
        }
        else if (playerPosition.x < transform.position.x) // move left
        {
            currentScale = transform.localScale;
            currentScale.x = Mathf.Abs(transform.localScale.x);
            transform.localScale = currentScale;

            sprite.flipX = true;
            transform.position += transform.right * enemySpeed * Time.deltaTime * 2f;
            // Debug.Log("carl left");
        }

    }
}
