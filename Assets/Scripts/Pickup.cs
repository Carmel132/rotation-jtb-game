using UnityEngine;

public class Pickup : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (gameObject.tag == "Button")
            {
                GameManager.gm.ButtonSFX();
                Destroy(gameObject);
            }
            else if (gameObject.tag == "Coin")
            {
                GameManager.gm.CoinSFX();
                Destroy(gameObject);
            }
            else if (gameObject.tag == "LizardButton")
            {
                GameManager.gm.LizardSFX();
            }

        }
    }

}
