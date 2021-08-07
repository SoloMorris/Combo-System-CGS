using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : CharacterComponent
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
    public bool canJump { get; private set; } = true;

    private void Start()
    {
        AssignComponents();
    }

    void Update()
    {
        TryMove();
    }

    //  Sets grounded upon touching the ground
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
            grounded = true;
    }


    #region Movement Checks & Update

    public void UpdateDirection(Vector2 newDirection)
    {
        MovementDirection = newDirection;
    }

    private bool CanMove()
    {
        return (grounded);
    }

    private void TryMove()
    {
        if (CanMove() && MovementDirection != Vector2.zero)
            DoMovement(MovementDirection);
    }

    public void Jump()
    {
        if (canJump && grounded)
            DoJump();
    }


    #endregion
    
    #region Movement Functions

    private void DoMovement(Vector2 direction)
    {
        var myBody = MyBody;
        if (direction.x != 0)
            myBody.velocity = new Vector2(moveSpeed * direction.x * movementMod, 
                myBody.velocity.y);
        else
            myBody.velocity = new Vector2(0, myBody.velocity.y);
    }

    private void DoJump()
    {
        groundDetection.LeaveGroundForTime(15f);
        MyBody.velocity = new Vector3(MyBody.velocity.x, jumpHeight, 500f);
    }

    public void ZeroMovement()
    {
        MyBody.velocity = Vector2.zero;
    }
    #endregion


}
