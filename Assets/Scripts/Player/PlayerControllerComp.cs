using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

/* ==== HOW PLAYER PHYSICS WORKS ====
 * 1. A velocity is computed under various conditions (like gravity)
 * 2. A set of key points (typically the edges) are raycasted from to where they would be under the computed velocity
 * 3. If none of the points intersect with a foreign object, then the player moves according to the projected velocity
 * 4. Otherwise, the shortest distance to an obstacle is taken an is proportionally applied to all other points, relative to the velocity
 */

internal struct KinematicFrame
{
    public Vector3 position, velocity;
}

internal class Physics
{
    public GameObject player { get; }
    /// <summary>
    /// List of vertices relative to center-of-mass of game object
    /// </summary>
    public List<Vector3> vertices;

    bool verifyObstacleCollision(List<RaycastHit2D> hits)
    {
        if (hits.Count == 0) { return false; }
        if (hits.Count == 1)
        {
            return !GameObject.ReferenceEquals(player, hits[0].transform.gameObject);
        }
        return true;
    }

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

    float computeAxialVelocity(Vector3 axis, Vector3 velocity)
    {
        return Vector3.Dot(axis, velocity);
    }

    void invokePlayerCollisionEvent(Vector3 axis)
    {
        EventManagerProp.onPlayerCollision(axis);
    }

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
            invokePlayerCollisionEvent(Vector3.up * Mathf.Sign(velocity.y));
        }

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

public class PlayerControllerComp : MonoBehaviour
{

    float horizontalInput { set; get; }
    float verticalInput { set; get; }
    bool jumpInput { set; get; }

    [SerializeField]
    bool isGrounded = false;

    Vector3 velocity { set; get; }
    Bounds bounds { set; get; }
    Physics physics { set; get; }


    void updateInputs()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        jumpInput = Input.GetButton("Jump");
    }

    void applyGravity()
    {
        velocity += Vector3.down * PlayerConstants.GRAVITY_FACTOR;
    }

    void applyMovement()
    {
        velocity = new Vector3(horizontalInput * 4, velocity.y, velocity.z);
    }

    void applyJump()
    {
        if (jumpInput && isGrounded)
        {
            Debug.Log("Jumping!");
            velocity += new Vector3(0, PlayerConstants.JUMP_FORCE);
            isGrounded = false;
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
        //Debug.Log(axis);

        if (axis == Vector3.down)
        {
            isGrounded = true;
            Debug.Log("Grounded!");
        }
    }

    void Start()
    {
        bounds = GetComponent<BoxCollider2D>().bounds;
        physics = new(gameObject, generateVerticesFromBoxCollider(bounds.extents));

        EventManagerProp.PlayerCollision += onPlayerCollision;
    }

    void Update()
    {
        updateInputs();
        applyGravity();
        applyMovement();
        applyJump();

        KinematicFrame kf = physics.computeNextStep(velocity);

        transform.position = kf.position;
        velocity = kf.velocity;
        
    }
};
