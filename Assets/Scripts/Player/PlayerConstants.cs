using UnityEngine;

/// <summary>
/// Class meant to hold static constants for the player
/// </summary>
public static class PlayerConstants
{
    /// <summary>
    /// The downward acceleration
    /// </summary>
    public static float GRAVITY_FACTOR { get; } = 0.1f;
    /// <summary>
    /// The width of the offset maintained between the player collider and other colliders.
    /// 
    /// This is done so the vertices of the player are never absorbed by other colliders and nullify
    /// collision logic
    /// </summary>
    public static float SKIN_WIDTH { get; } = 0.01f;
    /// <summary>
    /// The force of the jump (?)
    /// </summary>
    public static float JUMP_FORCE { get; } = 20f;
    /// <summary>
    /// The Speed of dashing :thumbs up:
    /// </summary>
    public static float DASHING_SPEED { get; } = 1.25f;
}
