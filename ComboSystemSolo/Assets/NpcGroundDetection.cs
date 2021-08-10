using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcGroundDetection : CharacterComponent
{
    private BoxCollider2D groundCheck;
    public bool grounded;

    // Start is called before the first frame update
    void Start()
    {
        AssignComponentsInParents();
        groundCheck = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Floor")) return;
        {
            grounded = true;
            if (state.currentMovementState == CharacterState.MovementState.Airborne)
                SetMovementState(CharacterState.MovementState.Neutral);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Floor")) return;
        grounded = false;
        SetMovementState(CharacterState.MovementState.Airborne);
    }
}
