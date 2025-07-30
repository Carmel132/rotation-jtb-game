using UnityEngine;
using System;

/*
 * === HOW TO ADD EVENTS ===
 * 1. Add a `public static event Action {event name};` field to the `EventManagerProp` class
 * 2. Add an invocation function `public static void on{event name}(params...)`
 * 3. The invocation function MUST call `{event name}.Invoke()` otherwise it wont actually call the delegates
 * 5. ????
 * 4. Profit
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

}
