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
        if (!grounded && state.currentMovementState != CharacterState.MovementState.Free) SetMovementState(CharacterState.MovementState.Airborne);
    }


    #region Movement Checks & Update

    public void UpdateDirection(Vector2 newDirection)
    {
        MovementDirection = newDirection;
    }

    private bool CanMove()
    {
        return (state.currentCombatState == CharacterState.CombatState.Neutral && state.currentMovementState != CharacterState.MovementState.Disabled &&
                state.currentMovementState != CharacterState.MovementState.Locked && grounded || state.currentMovementState == CharacterState.MovementState.Free);
    }

    public bool CanJump()
    {
        return (CanMove() && state.currentMovementState != CharacterState.MovementState.Airborne);
    }

    private void TryMove()
    {
        if (CanMove() && MovementDirection != Vector2.zero)
            DoMovement(MovementDirection);
        else if (MovementDirection == Vector2.zero)
        {
            if (grounded)
            {
                MyBody.velocity = new Vector2(0, MyBody.velocity.y);
                cAnimation.OnStopMove();
            }
            else
            {
                MyBody.velocity = new Vector2(MyBody.velocity.x, MyBody.velocity.y);
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
        var myBody = MyBody;
        if (direction.x == 0) return;
        myBody.velocity = new Vector2(moveSpeed * direction.x * movementMod,
            myBody.velocity.y);
        cAnimation.OnMove();
    }

    private void DoJump()
    {
        SetMovementState(CharacterState.MovementState.Airborne);
        cAnimation.animator.SetTrigger("Jump");
    }

    public void AnimJumpLaunch()
    {
        groundDetection.LeaveGroundForTime(15f);
        MyBody.velocity = new Vector2(MyBody.velocity.x, jumpHeight);
    }

    public void AnimTouchGround()
    {
        state.currentMovementState = CharacterState.MovementState.Neutral;
    }

    public void ZeroMovement()
    {
        MyBody.velocity = Vector2.zero;
    }
    #endregion


}
