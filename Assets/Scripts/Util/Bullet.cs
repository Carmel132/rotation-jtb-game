using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 3f; 

    void Start()
    {
       
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Enemy"))
        {
            
            
            Destroy(gameObject);
        }
        
        else if (!other.CompareTag("Player")) 
        {
            Destroy(gameObject);
        }
    }
}