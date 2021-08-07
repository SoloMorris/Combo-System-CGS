using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using Player;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAttacks : PlayerComponent
{
    [SerializeField] private BoxCollider2D myHitbox;
    
    [Header("Attack Stuff")]
    [SerializeField] private bool fightModeActive;
    [SerializeField] private AttackMovelist movelist;
    [SerializeField] Attack activeAttack;

    /// <summary>
    /// To store directional inputs for an attack.
    /// </summary>
    private ActionQueue storedInputs = new ActionQueue();
    private void Start()
    {
        AssignComponents();
    }


    void Update()
    {
        storedInputs.UpdateQueue();
        if (activeAttack != null)
        {
            activeAttack.Update();
        }
        // TODO Find out why the system is not saving the input state for the fightmode function .
    }

    private void OnAttackHit()
    {
        state.SetCombatState(CharacterState.CombatState.Hitlag);
        print("mom i'm hitting a thing");
    }
    private void OnAttackEnd()
    {
        hitboxControl.Deactivate();
        state.SetCombatState(CharacterState.CombatState.Neutral);
        activeAttack.Reset();
        activeAttack.attackHitEvent -= OnAttackHit;
        activeAttack.attackEndEvent -= OnAttackEnd;
        activeAttack = null;
    }
    public bool TryStartAttack(ActionInput atk)
    {
        if (!CheckStatesAllowAction()) return false;
        ExecuteAttackAction(atk);
        return true;
    }
    private void HandleFightMode()
    {
            
    }

    private void ExecuteAttackAction(ActionInput atk)
    {
        var attack = FindAttack();
        if (attack == null)
        {
            print("couldn't find attack ");
            return;
        }
        if (activeAttack != null) OnAttackEnd();
        activeAttack = attack;
        activeAttack.attackHitEvent += OnAttackHit;
        activeAttack.attackEndEvent += OnAttackEnd;
        hitboxControl.Activate(activeAttack);
        state.SetCombatState(CharacterState.CombatState.Attacking);
        
        Attack FindAttack()
        {
            // Loop through the movelist and find a match.
            // If there is a match, try to find if there are any input matches.

            Attack storedMove = null;
            foreach (var move in movelist.attacks)
            {            
                if (!move.enabled) continue;

                // If the player is in locked state, try to match directional inputs as well
                if (state.currentMovementState == CharacterState.MovementState.Locked)
                {
                    if (!TryFindDirectionalInputs())
                        continue;
                    return storedMove;
                }

                //  If the player isn't in lock state, just find the matching input.
                // if the player is attacking, assume that they are trying to input a combo.
                if (activeAttack == null)
                {
                    // Return an attack that starts a combo.
                    if (move.attackInputName.Count == 1 && move.attackInputName[0] == atk.inputContext.action.name
                                                        && move.startsACombo)
                        return move;
                } //Return an attack that continues a combo.
                else if (move.attackInputName.Count == 1 && move.attackInputName[0] == atk.inputContext.action.name
                                                         && move.prevAttack.name == activeAttack.name)
                    return move;

                bool TryFindDirectionalInputs()
                {
                    if (!move.attackInputName.Contains(atk.inputContext.action.name))
                        return false;

                    // If the input matches and the move only requires one input, this may be it - store it
                    //  But continue looping in case a better is found.
                    if (move.attackInputName.Count <= 1)
                    {
                        storedMove = move;
                        return false;
                    }
                    
                    print("Trying to find directional inputs");

                    bool[] checkListLength = new bool[storedInputs.Queue.Count];
                    
                    // Now loop through the checklist to find if the parts that match form a combo
                    for (int i = 0; i < checkListLength.Length - 1; i++)
                    {
                        if (move.attackInputName[i] == atk.inputContext.action.name) continue;
                        
                        //TODO: Continue this
                    }

                    return false;
                }
            }
            return storedMove;
        }
    }

    /// <summary>
    /// Check if the movement and combat states currently allow for an attack to be performed.
    /// </summary>
    /// <returns>True if player can attack, false if not.</returns>
    private bool CheckStatesAllowAction()
    {
        if (GetMovementState() == CharacterState.MovementState.Disabled) return false;
        return GetCombatState() != CharacterState.CombatState.Attacking && 
               GetCombatState() != CharacterState.CombatState.Hitstun;
    }

}
    
