using UnityEngine;

public class EnemyShootableHitbox : MonoBehaviour
{
    [SerializeField]
    float health;
    [SerializeField]
    float damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.CompareTag("EnemyProjectile"))
        //{
        //    Destroy(collision.gameObject);
        //    Destroy(gameObject);
        //}

        if (collision.CompareTag("Bullet"))
        {
            health -= collision.GetComponent<Bullet>().damage;
            if (health <= 0)
            {
                Destroy(gameObject);
            }
        }

        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
    public float getDamage() => damage;
}
