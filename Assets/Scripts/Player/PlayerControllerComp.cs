using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

/*
 * === HOW GRAVITY WORKS ===
 * 1. There is a gravity factor in the PlayerConstants class
 * 2. Each frame, the player computes what its velocity will be as if it is in the air
 * 3. Then it raycasts down to what its position will be to determine if the way is free of obstacles
 * 4. If it is free, then it moves accordingly, otherwise it will snap to the nearest obstacle, as if it collided with it
 */

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

    float computeDistanceToNearestObstacleOnAxis(Vector3 axis)
    {
        float minDistance = Mathf.Infinity;


        foreach (var vertex in vertices)
        {
            Util.UnfilteredRaycast2D(player.transform.position + vertex, axis, axis.magnitude, out List<RaycastHit2D> hits);

            if (!verifyObstacleCollision(hits)) { continue; }

            minDistance = Mathf.Min(minDistance, hits
                .Where(hit => !GameObject.ReferenceEquals(player, hit.transform.gameObject))
                .Min(a => a.distance));
        }

        return minDistance;
    }

    float snapToObstacleOnAxis(Vector3 axis, float axialVelocity, out bool wasCollision)
    {
        float minDistance = computeDistanceToNearestObstacleOnAxis(axis);
        if (minDistance <= Mathf.Abs(axialVelocity))
        {
            wasCollision = true;
            return minDistance;
        }
        wasCollision = false;
        return axialVelocity;
    }

    float computeAxialVelocity(Vector3 axis, Vector3 velocity)
    {
        return Vector3.Dot(axis, velocity);
    }

    public KinematicFrame computeNextStep(Vector3 velocity)
    {

        // Horizontal test
        float xTrueVel = snapToObstacleOnAxis(Vector3.right, computeAxialVelocity(Vector3.right, velocity), out bool wasHorizontalCollision);
        float xNextVel = wasHorizontalCollision ? 0 : velocity.x;

        // Vertical test
        float yTrueVel = snapToObstacleOnAxis(Vector3.up, computeAxialVelocity(Vector3.up, velocity), out bool wasVerticalCollision);
        float yNextVel = wasVerticalCollision ? 0 : velocity.y;

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

    Vector3 velocity { set; get; }
    Bounds bounds { set; get; }
    Physics physics { set; get; }



    void updateInputs()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }


    void ApplyGravity()
    {
        velocity += Vector3.down * PlayerConstants.GRAVITY_FACTOR;
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

    void Start()
    {
        bounds = GetComponent<BoxCollider2D>().bounds;
        physics = new(gameObject, generateVerticesFromBoxCollider(bounds.extents));
    }

    void Update()
    {
        updateInputs();
        ApplyGravity();

        KinematicFrame kf = physics.computeNextStep(velocity * Time.deltaTime);

        transform.position = kf.position;
        velocity = kf.velocity;

        
    }
};
