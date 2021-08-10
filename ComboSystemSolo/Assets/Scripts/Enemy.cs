using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Combatant
{
    private Rigidbody2D myBody;
    private Animator animator;
    private NpcGroundDetection groundCheck;

    void Start()
    {
        myBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        groundCheck = gameObject.GetComponentInChildren<NpcGroundDetection>();
    }

    private void Update()
    {
        HandleGenericAnimation(animator);
    }

    protected override void HandleGenericAnimation(Animator animator)
    {
        base.HandleGenericAnimation(animator);
        // If grounded and not in hitstun, leave disabled state.
        {
            if (state.currentMovementState == CharacterState.MovementState.Disabled &&
                state.currentCombatState == CharacterState.CombatState.Neutral &&
                groundCheck.grounded)
                animator.SetBool("Disabled", false);
        }
    }
}
