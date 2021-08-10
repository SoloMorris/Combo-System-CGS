using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class CharacterComponent : MonoBehaviour
{
    protected CharacterState state;
    public Rigidbody2D MyBody;

    #region Setup
    void Awake()
    {
        AssignComponents();
    }

    protected virtual void AssignComponents()
    {
        state = GetComponent<CharacterState>();
        MyBody = GetComponent<Rigidbody2D>();
    }

    protected virtual void AssignComponentsInParents()
    {
        state = GetComponentInParent<CharacterState>();
        MyBody = GetComponentInParent<Rigidbody2D>();
    }
    
    

    #endregion
    
    public CharacterState.MovementState GetMovementState()
    {
        return state.currentMovementState;
    }
    public CharacterState.CombatState GetCombatState()
    {
        return state.currentCombatState;
    }
    
    public void SetMovementState(CharacterState.MovementState nState)
    {
        state.currentMovementState = nState;
    }
    public void SetCombatState(CharacterState.CombatState nState)
    {
        state.currentCombatState =nState ;
    }
}


