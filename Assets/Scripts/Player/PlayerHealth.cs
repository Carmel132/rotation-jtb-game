using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    float health;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyProjectile"))
        {
            health -= collision.GetComponent<EnemyShootableHitbox>().getDamage();
        }
        else if (collision.CompareTag("EnemyHitbox"))
        {
            health -= collision.GetComponent<EnemyHitbox>().getDamage();
            Debug.Log("hitbox");
        }
    }

    public void Update()
    {
        if (health <= 0)
        {
            Debug.Log("YOU DIE");
        }
    }
}