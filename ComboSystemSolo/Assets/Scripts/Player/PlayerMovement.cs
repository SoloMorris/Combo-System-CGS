using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class PlayerMovement : PlayerComponent
{
    public Vector2 MovementDirection;

    //  Jumping
    public bool grounded;
    [SerializeField] private float jumpHeight = 50f;
    [SerializeField] private GroundDetection groundDetection;

    //  Movement
    [SerializeField] private int moveSpeed = 3;
    [SerializeField] public float movementMod = 1;
    
    // Flags
    public bool canJump = true;

    private void Start()
    {
        AssignComponents();
    }

    void Update()
    {
        cAnimation.animator.SetBool("Grounded", grounded);
        TryMove();
        cAnimation.animator.SetBool("PrevGroundedState", grounded);
    }

    public void SetLocked()
    {
        SetMovementState(CharacterState.MovementState.Locked);
        MyBody.gravityScale = 0f;
    }

    public void SetUnlocked()
    {
        SetMovementState(CharacterState.MovementState.Neutral);
        MyBody.gravityScale = 1f;
    }

    #region Movement Checks & Update

    public void UpdateDirection(Vector2 newDirection)
    {
        MovementDirection = newDirection;
    }

    private bool CanMove()
    {
        return GetCombatState() == CharacterState.CombatState.Neutral &&
               GetMovementState() != CharacterState.MovementState.Disabled &&
               GetMovementState() != CharacterState.MovementState.Locked &&
               grounded &&
               GetMovementState() != CharacterState.MovementState.Airborne;

    }

    public bool CanJump()
    {
        return CanMove() || GetCombatState() == CharacterState.CombatState.Free;
    }

    private void TryMove()
    {
        if (CanMove()) {
            DoMovement(MovementDirection);
        }
            var currentCombatState = GetCombatState();
        if (MovementDirection == Vector2.zero || currentCombatState != CharacterState.CombatState.Neutral ) {
            if (grounded) {
                MyBody.velocity = new Vector2(0, MyBody.velocity.y);
                cAnimation.OnStopMove();
            }
        }
    }

    public void Jump()
    {
        if (CanJump())
            DoJump();
    }


    #endregion
    
    #region Movement Functions

    private void DoMovement(Vector2 direction)
    {
        MyBody.velocity = new Vector2(moveSpeed * direction.x * movementMod,
            MyBody.velocity.y);
        cAnimation.OnMove();
    }

    private void DoJump()
    {
        if (GetCombatState() == CharacterState.CombatState.Free)
        {
            DoMovement(MovementDirection);
            cAttacks.OnAttackEnd(cAttacks.GetActiveAttack());
            cAnimation.animator.SetTrigger("CancelJump");
            SetCombatState(CharacterState.CombatState.Neutral);
            return;
        }
        SetMovementState(CharacterState.MovementState.Airborne);
        cAnimation.animator.SetTrigger("Jump");
    }

    public void AnimJumpLaunch()
    {
        groundDetection.LeaveGroundForTime(15f);
        if (GetCombatState() == CharacterState.CombatState.Free)
            MyBody.velocity = new Vector2(MovementDirection.x, jumpHeight);
        else
            MyBody.velocity = new Vector2(MyBody.velocity.x, jumpHeight);
    }

    public void AnimTouchGround()
    {
        if (GetMovementState() is CharacterState.MovementState.Airborne) SetMovementState(CharacterState.MovementState.Neutral);
    }

    public void ZeroMovement()
    {
        MyBody.velocity = Vector2.zero;
    }
    #endregion


}
