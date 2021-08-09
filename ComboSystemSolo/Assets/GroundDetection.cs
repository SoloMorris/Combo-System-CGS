using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class GroundDetection : PlayerComponent
{
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private PlayerMovement playerMovement;
    private float Timer = 0.0f;
    private float tick = 0.0f;
    
    private void Start()
    {
        AssignComponents();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor")) return;
            cMovement.grounded = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Floor")) return;
        cMovement.grounded = false;
    }

    private void Update()
    {
        if (!Timer.Equals(0f))
            Uptick();
    }

    /// <summary>
    /// Set grounded to false for at least X amount of time.
    /// </summary>
    /// <param name="duration"></param>
    public void LeaveGroundForTime(float duration)
    {
        playerMovement.grounded = false;
        print("Trying to jump");
        Timer = duration;
    }

    /// <summary>
    /// Increase tick until it surpasses Timer
    /// </summary>
    private void Uptick()
    {
        if (tick >= Timer)
        {
            Timer = 0.0f;
            tick = 0.0f;
        }
        else
        {
            tick++;
            playerMovement.grounded = false;
        }
    }
}
