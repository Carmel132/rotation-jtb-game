using UnityEngine;

/// <summary>
/// Class meant to hold static constants for the player
/// </summary>
public static class PlayerConstants
{
    /// <summary>
    /// The downward acceleration
    /// </summary>
    public static float GRAVITY_FACTOR { get; } = 0.7f;
    /// <summary>
    /// The width of the offset maintained between the player collider and other colliders.
    /// 
    /// This is done so the vertices of the player are never absorbed by other colliders and nullify
    /// collision logic
    /// </summary>
    public static float SKIN_WIDTH { get; } = 0.03f;
    /// <summary>
    /// The force of the jump (?)
    /// </summary>
    public static float JUMP_FORCE { get; } = 40f;
}
