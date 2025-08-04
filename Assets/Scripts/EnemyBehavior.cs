using UnityEngine;
using System.Collections;
using System;

public class EnemyBehavior : MonoBehaviour
{

    public int health;
    public int bulletDamage;
    public float enemySpeed;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
    }
}
