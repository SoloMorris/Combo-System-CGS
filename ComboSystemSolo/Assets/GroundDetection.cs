using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class GroundDetection : PlayerComponent
{
    [SerializeField] private LayerMask groundMask;
    private float Timer = 0f;
    private float tick = 0.0f;
    
    private void Start()
    {
        AssignComponentsInParents();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Floor")) return;
        {
            cMovement.grounded = true;
            if (state.currentMovementState == CharacterState.MovementState.Airborne)
                SetMovementState(CharacterState.MovementState.Neutral);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Floor")) return;
        {
            cMovement.grounded = false;
            if (state.currentMovementState != CharacterState.MovementState.Free)
                SetMovementState(CharacterState.MovementState.Airborne);
        }
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
        cMovement.grounded = false;
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
            cMovement.grounded = false;
        }
    }
}
