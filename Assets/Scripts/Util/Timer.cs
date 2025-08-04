using System;
using UnityEngine;


/// <summary>
/// Encapsulated a fixed time timer 
/// </summary>
public class FixedTimer
{
    public float duration { get; set; }
    public bool timePassed { get => Time.time >= lastTime + duration; }
    public void reset () => lastTime = Time.time;


    float lastTime;
    public FixedTimer (float duration)
    {
        this.duration = duration;
        lastTime = Time.time;
    }
}