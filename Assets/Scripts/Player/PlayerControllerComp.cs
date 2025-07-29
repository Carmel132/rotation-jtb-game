using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerComp : MonoBehaviour
{

    float horizontalInput { set; get; }
    float verticalInput { set; get; }

    Vector2 velocity { set; get; }
    Bounds bounds { set; get; }
    void updateInputs()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    bool onGround()
    {

        Vector3 bot = transform.position - new Vector3(0, bounds.extents.y, 0);
        Debug.Log(bot);
        Debug.Log(transform.position);
        List<RaycastHit2D> hit = new();
        ContactFilter2D contactFilter = new();
        contactFilter.NoFilter();
        if (Physics2D.Raycast(new Vector2(bot.x, bot.y), Vector2.down, contactFilter, hit, 10) > 0)
        {
            foreach (RaycastHit2D h in hit)
            {
                if (!Object.ReferenceEquals(h.transform.gameObject, gameObject) && h.distance <= 0.01f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    void applyGravity()
    {
        velocity += Vector2.down * PlayerConstants.GRAVITY_FACTOR * Time.deltaTime;
    }

    void applyVelocity()
    {
        transform.position = transform.position + new Vector3(velocity.x, velocity.y, 0) * Time.deltaTime;
    }

    void Start()
    {
        bounds = GetComponent<BoxCollider2D>().bounds;
    }

    void Update()
    {
        updateInputs();

        bool ong = onGround();

        //Debug.Log(ong);
        if (ong)
        {
            velocity = Vector2.zero;
        }
        else
        {
            applyGravity();
        }

        applyVelocity();
    }
};
