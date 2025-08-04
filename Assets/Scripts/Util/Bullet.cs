using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 3f;
    public float speed = 20f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += transform.right * speed * Time.deltaTime; // moves bullet
    }


    /* void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    } */

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("hit enemy");
            Destroy(gameObject);
        }
    }
}