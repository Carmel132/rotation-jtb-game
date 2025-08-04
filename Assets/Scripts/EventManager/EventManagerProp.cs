using UnityEngine;
using System;

/*
 * === HOW TO ADD EVENTS ===
 * 1. Add a `public static event Action {event name};` field to the `EventManagerProp` class
 * 2. Add an invocation function `public static void on{event name}(params...)`
 * 3. The invocation function MUST call `{event name}?.Invoke()` otherwise it wont actually call the delegates
 * 5. ????
 * 4. Profit
 * 67. When adding an event make sure to meticulously document it
 */

/*
 * === HOW TO SUBSCRIBE TO AN EVENT ===
 * Say you have a function `on{event name}(params...)` that needs to be called on every {event name}
 * Write `EventManagerComp.on{event name} += on{event name};`
 * The function will be called automatically whenever the event is invoked
 */

/*
 * === HOW TO BROADCAST EVENTS ===
 * The event manager (should be) statically referenced every time
 * That means the actual "object" in memory for the event manager will only ever exist once
 * This makes it incredible helpful to reference the manager from any context
 * 
 * To invoke an event, call `EventManagerProp.on{event name}(params...);`
 * This function will be accessible from any context
 */

/// <summary>
/// Event manager property. Should realistically be applied to only one object per scene
/// </summary>
public class EventManagerProp : MonoBehaviour
{
    /// <summary>
    /// Player collision event. Called every frame the player is colliding with something (this means the floor)
    /// `axis` corresponds to the Vector3 directions (i.e. {1, 0, 0} == Vector3.right) and the direction
    /// corresponds to the side of the player that is currently colliding
    /// The X axis and Y axis will only ever be called once per frame each
    /// </summary>
    public static event Action<Vector3> PlayerCollision;
    public static void onPlayerCollision(Vector3 axis)
    {
        PlayerCollision?.Invoke(axis);
    }

    /// <summary>
    /// Gets called, cannot guarantee every frame, but whenever the player's grounded state is assured
    /// `grounded` is whether the player is currently grounded
    /// </summary>
    public static event Action<bool> PlayerGrounded;
    public static void isPlayerGrounded(bool grounded)
    {
        PlayerGrounded?.Invoke(grounded);
    }
}
