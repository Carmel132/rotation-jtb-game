using System;
using UnityEngine;


/// <summary>
/// Encapsulated a fixed time timer 
/// </summary>
[Serializable]
public class FixedTimer
{
    [SerializeField]
    public float duration;
    public bool timePassed { get => Time.time >= lastTime + duration; }
    public void reset () => lastTime = Time.time;


    float lastTime;
    public FixedTimer (float duration)
    {
        this.duration = duration;
        lastTime = Time.time;
    }
}