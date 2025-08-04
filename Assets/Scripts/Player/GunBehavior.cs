using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    private Camera mainCamera;
    public GameObject bulletPrefab; 
    public Transform firePoint; 
    public float bulletSpeed = 30f;
    public float fireRate = 0.5f; 
    public float spreadAngle = 10f;
    public int bulletCount = 3; 
    private float nextFireTime;
    public GameObject player;
    public GameObject shotgunSprite;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z;
        Vector3 direction = mousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (Mathf.Abs(transform.rotation[3]) >= Mathf.Abs(transform.rotation[2]) && player.transform.localScale.x < 0) //player facing left, gun facing right
        {
            player.transform.localScale = new Vector3(player.transform.localScale.x * -1, player.transform.localScale.y, player.transform.localScale.z); //flip player
            shotgunSprite.transform.localEulerAngles = new Vector3(0,0,0); //edit rotation of gun sprite so looks correct
            //firePoint.transform.localEulerAngles = new Vector3(0, 0, 0); //edit rotation of gun sprite so looks correct
        }
        if (Mathf.Abs(transform.rotation[3]) < Mathf.Abs(transform.rotation[2]) && player.transform.localScale.x > 0) //player facing right, gun facing left
        {
            player.transform.localScale = new Vector3(player.transform.localScale.x * -1, player.transform.localScale.y, player.transform.localScale.z);
            shotgunSprite.transform.localEulerAngles = new Vector3(0, 0, 180);
            //firePoint.transform.localEulerAngles = new Vector3(0, 0, 180);
        }

        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            
            Shoot();
            GameManager.gm.PlayerShootSFX();
            nextFireTime = Time.time + fireRate;
        }

        Debug.DrawLine(shotgunSprite.transform.position, firePoint.transform.position, Color.red);

    }

    void Shoot()
    {
        
        float baseAngle = transform.eulerAngles.z;

        
        float startAngle = -spreadAngle * (bulletCount - 1) / 2;

        for (int i = 0; i < bulletCount; i++)
        {

            float currentAngle = baseAngle + startAngle + (i * spreadAngle);
            Quaternion bulletRotation = Quaternion.Euler(0, 0, currentAngle);


            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, bulletRotation);

            Vector3 bulletVelocity = bulletRotation * Vector2.right * bulletSpeed;
           
            /* Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            Vector2 direction = bulletRotation * Vector2.right; 
            rb.linearVelocity = direction * bulletSpeed; */
        }
    }
}