using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;


//^^^needed

/* ==== HOW PLAYER PHYSICS WORKS ====
 * 1. A velocity is computed under various conditions (like gravity)
 * 2. A set of key points (typically the edges) are raycasted from to where they would be under the computed velocity
 * 3. If none of the points intersect with a foreign object, then the player moves according to the projected velocity
 * 4. Otherwise, the shortest distance to an obstacle is taken an is proportionally applied to all other points, relative to the velocity
 */

/// <summary>
/// Holds velocity and position data
/// </summary>
internal struct KinematicFrame
{
    public Vector3 position, velocity;
}

/// <summary>
/// Class responsible for holding player physics logic
/// </summary>
internal class Physics
{
    public GameObject player { get; }
    /// <summary>
    /// List of vertices relative to center-of-mass of game object
    /// </summary>
    public List<Vector3> vertices;

    /// <summary>
    /// Determines whether a list of raycast hits actually amounted to a hit
    /// </summary>
    /// <param name="hits">List of raycast hits</param>
    /// <returns>Whether a foreign object was hit</returns>
    bool verifyObstacleCollision(List<RaycastHit2D> hits)
    {
        if (hits.Count == 0) { return false; }
        if (hits.Count == 1)
        {
            return !GameObject.ReferenceEquals(player, hits[0].transform.gameObject);
        }
        return true;
    }

    /// <summary>
    /// Computes the distance to the nearest obstacle on a given axis
    /// </summary>
    /// <param name="position">The current center of mass of the player</param>
    /// <param name="axis">The axis to test</param>
    /// <returns>The distance of the nearest object. Returns `Mathf.Infinity` if no objects are found</returns>
    float computeDistanceToNearestObstacleOnAxis(Vector3 position, Vector3 axis)
    {
        float minDistance = Mathf.Infinity;


        foreach (var vertex in vertices)
        {
            Util.DebugUnfilteredRaycast2D(position + vertex, axis, axis.magnitude, out List<RaycastHit2D> hits);

            if (!verifyObstacleCollision(hits)) { continue; }

            minDistance = Mathf.Min(minDistance, hits
                .Where(hit => !GameObject.ReferenceEquals(player, hit.transform.gameObject))
                .Min(a => a.distance));
        }

        return minDistance;
    }

    /// <summary>
    /// Ensures the player does not move inside an obstacle
    /// </summary>
    /// <param name="position">Position of the player</param>
    /// <param name="axis">Axis to test</param>
    /// <param name="axialVelocity">Velocity of player on the given axis</param>
    /// <param name="wasCollision">Out variable returning whether a collision occured</param>
    /// <returns>The true velocity the player encountered on the axis</returns>
    float snapToObstacleOnAxis(Vector3 position, Vector3 axis, float axialVelocity, out bool wasCollision)
    {
        float minDistance = computeDistanceToNearestObstacleOnAxis(position, axis);

        if (minDistance < PlayerConstants.SKIN_WIDTH)
        {
            wasCollision = true;
            return 0;
        }


        if (minDistance <= Mathf.Abs(axialVelocity))
        {
            wasCollision = true;
            return Mathf.Sign(axialVelocity) * (Mathf.Max(minDistance, 0f) - PlayerConstants.SKIN_WIDTH);
        }
        wasCollision = false;
        return axialVelocity;
    }

    /// <summary>
    /// Computes axial velocity
    /// </summary>
    /// <param name="axis">The given axis</param>
    /// <param name="velocity">The absolute velocity in the standard basis</param>
    /// <returns>The axial velocity</returns>
    float computeAxialVelocity(Vector3 axis, Vector3 velocity)
    {
        return Vector3.Dot(axis, velocity);
    }

    /// <summary>
    /// Calls the player collision event
    /// </summary>
    /// <param name="axis">The axis that was intersected</param>
    void invokePlayerCollisionEvent(Vector3 axis)
    {
        EventManagerProp.onPlayerCollision(axis);
    }

    void invokePlayerGroundedEvent(bool grounded)
    {
        EventManagerProp.isPlayerGrounded(grounded);
    }

    /// <summary>
    /// Computes the next KinematicFrame
    /// </summary>
    /// <param name="velocity">The current velocity of the player</param>
    /// <returns>The kinematic frame</returns>
    public KinematicFrame computeNextStep(Vector3 velocity)
    {

        Vector3 frameIndependentVelocity = velocity * Time.deltaTime;

        // Horizontal test
        float xTrueVel = snapToObstacleOnAxis(player.transform.position, Vector3.right * Mathf.Sign(frameIndependentVelocity.x), computeAxialVelocity(Vector3.right, frameIndependentVelocity), out bool wasHorizontalCollision);
        float xNextVel = wasHorizontalCollision ? 0 : velocity.x;

        if (wasHorizontalCollision)
        {
            invokePlayerCollisionEvent(Vector3.right * Mathf.Sign(velocity.x));
        }

        // Vertical test
        float yTrueVel = snapToObstacleOnAxis(player.transform.position + new Vector3(xTrueVel, 0), Vector3.up * Mathf.Sign(frameIndependentVelocity.y), computeAxialVelocity(Vector3.up, frameIndependentVelocity), out bool wasVerticalCollision);
        float yNextVel = wasVerticalCollision ? 0 : velocity.y;

        if (wasVerticalCollision)
        {
            Vector3 collisionAxis = Vector3.up * Mathf.Sign(velocity.y);
            invokePlayerCollisionEvent(collisionAxis);
        }

        Debug.Log($"{wasVerticalCollision}, {velocity.y}");

        invokePlayerGroundedEvent(wasVerticalCollision && velocity.y < 0);

        KinematicFrame ret = new KinematicFrame
        {
            position = player.transform.position + new Vector3(xTrueVel, yTrueVel, 0),
            velocity = new Vector3(xNextVel, yNextVel)
        };

        return ret;
    }

    public Physics(GameObject player, List<Vector3> vertices)
    {
        this.player = player;
        this.vertices = vertices;
    }
}

/// <summary>
/// The player controller. It is 11:58pm. I cannot type anymore. Good luck solider
/// </summary>
public class PlayerControllerComp : MonoBehaviour
{
    //animation stuff vvv
    public Animator animator;

    
    public RuntimeAnimatorController baseController;
    public RuntimeAnimatorController timController;
    public RuntimeAnimatorController kyleController;

    public Sprite[] shotguns;

    public SpriteRenderer shotgunSprite;

    //animation stuff ^^^

    //for new button vvv
    bool switchInput { set; get; }
    int currentSkin = 0;

    float horizontalInput { set; get; }
    float verticalInput { set; get; }
    bool jumpInput { set; get; }
    bool dashInput { set; get; }

    
    [SerializeField]
    bool isGrounded = false;

    Vector3 velocity { set; get; }
    Bounds bounds { set; get; }
    Physics physics { set; get; }

    //Uh sorry carmel if this messes up ur code ngl - michael, 7/31
    private Vector2 dashDirect;
    private bool isDashing;
    private bool canDash = true;
    private float MaxS = 100;
    private float MinS = 100;

    void updateInputs()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        jumpInput = Input.GetButton("Jump");

        switchInput = Input.GetButtonDown("SwitchSkin");

        dashInput = Input.GetButtonDown("Dash");
    }

    void applyGravity()
    {
        velocity += Vector3.down * PlayerConstants.GRAVITY_FACTOR;
        
    }

    void applyMovement()
    {
        velocity = new Vector3(horizontalInput * 10, velocity.y, velocity.z);
    }

    void applyJump()
    {
        if (jumpInput && isGrounded)
        {
            Debug.Log("Jumping!");
            GameManager.gm.PlayerJumpSFX();
            velocity += new Vector3(0, PlayerConstants.JUMP_FORCE);
            isGrounded = false;
            
        }
    }
    //Michael, 7/31
    private IEnumerator SDash()
    {
        yield return new WaitForSeconds(0.2f);
        isDashing = false;
    }
    void applyDash()
    {
        if (dashInput && canDash)
        {
            Debug.Log("Dashing!");
            GameManager.gm.PlayerDashSFX();
            isDashing = true;
            canDash = false;
            dashDirect = new Vector2(horizontalInput, verticalInput);
            if (dashDirect == Vector2.zero)
            {
                dashDirect = new Vector2(transform.localScale.x, 0);
            }
            dashDirect.Normalize();
            StartCoroutine(SDash());
            return;
        }
        if (isDashing)
        {
            velocity += new Vector3(dashDirect.x * 0.5f, dashDirect.y * 0.24f) * PlayerConstants.DASHING_SPEED;
            return;
        }
        if (isGrounded)
        {
            new WaitForSeconds(1f);
            canDash = true;
        }
    }
    

    void animate()
    {
        //sets variables to trigger animations :thumbs_up:
        animator.SetFloat("Speed", Mathf.Abs(velocity.x));
        animator.SetBool("Grounded", isGrounded);
        animator.SetFloat("HSpeed", velocity.y); //this is horizontal speed, animates jump/fall

    }

    void switchSkin()
    {
        //allows for swapping skins
        if (switchInput)
        {
            currentSkin += 1;
            currentSkin = currentSkin % 3;
        }

        //when pressing button, increases currentSkin and changes animator and shotgun
        //to add more, make a new case, new controller variable, and add shotgun skin to array
        switch(currentSkin)
        {
            case 1:
                animator.runtimeAnimatorController = timController;
                shotgunSprite.sprite = shotguns[1];
                break;
            case 2:
                animator.runtimeAnimatorController = kyleController;
                shotgunSprite.sprite = shotguns[2];
                break;
            default:
                animator.runtimeAnimatorController = baseController;
                shotgunSprite.sprite = shotguns[0];
                // currentSkin = 0;
                break;

        }

    }

    List<Vector3> generateVerticesFromBoxCollider(Vector2 extent)
    {
        List<Vector3> ret = new()
        {
            new Vector3(extent.x, extent.y),
            new Vector3(extent.x, -extent.y),
            new Vector3(-extent.x, -extent.y),
            new Vector3(-extent.x, extent.y)
        };
        return ret;
    }

    void onPlayerCollision(Vector3 axis)
    {
        if (axis == Vector3.down)
        {
            isGrounded = true;

        }
    }

    void isPlayerGrounded(bool grounded)
    {
        isGrounded = grounded;
    }

    void Start()
    {
        bounds = GetComponent<BoxCollider2D>().bounds;
        physics = new(gameObject, generateVerticesFromBoxCollider(bounds.extents));

        //EventManagerProp.PlayerCollision += onPlayerCollision;
        EventManagerProp.PlayerGrounded += isPlayerGrounded;

    }

    void Update()
    {
        updateInputs();
        applyGravity();
        if (!isDashing)
        {
            applyMovement();
        }
        applyJump();
        applyDash();

        KinematicFrame kf = physics.computeNextStep(velocity);

        transform.position = kf.position;
        velocity = kf.velocity;
        animate();
        switchSkin();

    }
};
