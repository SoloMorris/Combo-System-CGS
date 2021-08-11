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
    [SerializeField] private AttackMovelist specialMovelist;
    [SerializeField] Attack activeAttack;

    /// <summary>
    /// To store directional inputs for an attack.
    /// </summary>
    public ActionQueueString storedInputs = new ActionQueueString();

    /// <summary>
    /// Sets to true every frame, allows delay of hitlag enumerator 
    /// </summary>
    private bool hitLagCheck =false;
    private void Start()
    {
        AssignComponents();
        storedInputs.lifetime = 1.0f;
    }


    void Update()
    {
        storedInputs.UpdateQueue();

        EnsureState();
        void EnsureState()
        {
            if (state.currentCombatState == CharacterState.CombatState.Neutral && activeAttack)
            {
                print("catch states mismatched");
            }
        }
    }

    #region Attack Delegates
    
    private void OnAttackHit()
    {
        if (!IsAttackActive()) return;
        if (activeAttack.canBeCancelled)
        {
            OnCancellableAttackHit();
            return;
        }
        SetCombatState(CharacterState.CombatState.Hitlag);
        activeAttack.hit++;
        if (activeAttack.playerHitLag > 0) StartCoroutine(FreezeHitlag());
    }
    
    private void OnCancellableAttackHit()
    {
        if (!IsAttackActive()) return;
        SetCombatState(CharacterState.CombatState.Free);
        activeAttack.hit++;
        if (activeAttack.playerHitLag > 0) StartCoroutine(FreezeCancelWindow());
    }

    public IEnumerator FreezeHitlag()
    {
        float counter = 0f;
        var storedAtk = activeAttack;
        while (counter < storedAtk.playerHitLag)
        {
            if (activeAttack == null) yield break;
            GetComponent<Animator>().speed = 0f;
            MyBody.velocity = Vector2.zero;
            counter++;
            yield return 0;
        }
        SetCombatState(CharacterState.CombatState.Attacking);
        GetComponent<Animator>().speed = 1f;
    }

    public IEnumerator FreezeCancelWindow()
    {
        float counter = 0f;
        var storedAtk = activeAttack;
        while (counter < storedAtk.playerHitLag)
        {
            if (activeAttack == null) yield break;
            GetComponent<Animator>().speed = 0f;
            MyBody.velocity = Vector2.zero;
            counter++;
            yield return 0;
        }
        GetComponent<Animator>().speed = 1f;
        SetCombatState(CharacterState.CombatState.Attacking);
    }

    #endregion

    #region Animation Events
    public void OnAttackEnd(Attack atk)
    {
        if (activeAttack == null || atk.name != activeAttack.name)
        {
            return;
        }
        
        SetCombatState(CharacterState.CombatState.Neutral);
        cHitBox.Deactivate();
        activeAttack.attackHitEvent -= OnAttackHit;
        activeAttack.active = false;
        activeAttack.hit = 0;
        activeAttack = null;
        StopCoroutine(FreezeHitlag());
        StopCoroutine(FreezeCancelWindow());
        GetComponent<Animator>().speed = 1f;
        print("Attack over");
    }

    public void TryApplySelfEffect()
    {
        if (IsAttackActive())
            combatant.ApplyEffectsFromAttack(activeAttack);
    }
    public void ActivateHitbox()
    {
        if (IsAttackActive())
            cHitBox.Activate(activeAttack);
    }

    public void DeactivateHitbox()
    {
        if (IsAttackActive())
            cHitBox.Deactivate();
    }


    public void CheckAnimationIsCleared()
    {
        if (activeAttack != null || state.currentCombatState == CharacterState.CombatState.Attacking)
            OnAttackEnd(activeAttack);
    }
    public void EnterRecovery()
    {
        if (IsAttackActive())
            SetCombatState(CharacterState.CombatState.Recovery);
    }

    #endregion
    public bool TryStartAttack(ActionInput atk)
    {
        if (!CheckStatesAllowAction()) return false;
        ExecuteAttackAction(atk);
        return true;
    }

    
    public Attack GetActiveAttack()
    {
        return activeAttack;
    }

    private bool IsAttackActive()
    {
        return activeAttack != null && activeAttack.active;
    }

    private void ExecuteAttackAction(ActionInput atk)
    {
        var attack = FindAttack();
        if (attack == null)
        {
            return;
        }

        // If there is an attack being used, end it first
        if (state.currentCombatState == CharacterState.CombatState.Recovery) OnAttackEnd(activeAttack);
        activeAttack = attack;
        activeAttack.attackHitEvent += OnAttackHit;
        SetCombatState(CharacterState.CombatState.Attacking);
        activeAttack.active = true;
        cAnimation.PlayAnimation(activeAttack.attackAnimation);

        Attack FindAttack()
        {
            if (GetMovementState() == CharacterState.MovementState.Locked) return FindSpecialAttack();
            // Loop through the movelist and find a match.
            // If there is a match, try to find if there are any input matches.

            foreach (var move in movelist.attacks)
            {
                if (!move.enabled) continue;

                //  If the player isn't in lock state, just find the matching input.
                // if the player is attacking, assume that they are trying to input a combo.
                if (state.currentCombatState == CharacterState.CombatState.Neutral)
                {
                    // Return an attack that starts a combo.
                    if (move.attackInputName.Count == 1 && move.attackInputName[0] == atk.inputContext.action.name
                                                        && move.startsACombo && move.aerialMove != cMovement.grounded)
                        return move;
                } //Return an attack that continues a combo.
                else if (move.attackInputName.Count == 1 && move.attackInputName[0] == atk.inputContext.action.name
                                                         && move.prevAttack != null
                                                         && move.prevAttack.name == activeAttack.name
                                                         && move.aerialMove != cMovement.grounded)
                    return move;

            }
            return null;
        }

        Attack FindSpecialAttack()
        {
            Attack closestMatch = null;
            // Loop through the attacks in the special movelist
            foreach (var move in specialMovelist.attacks)
            {
                if (!move.enabled) continue;
                // Loop through each input for the move
                for (int i = 0; i < move.attackInputName.Count - 1; i++)
                {
                    
                    var input = move.attackInputName[i];
                    
                    //  Loop through the stored inputs
                    for (int j = 0; j  < storedInputs.Queue.Count; j++)
                    {
                        //  If both inputs match, start another loop
                        var match = storedInputs.Queue[j].name;
                        if (input != match || j + 1 > storedInputs.Queue.Count - 1
                        || i + 1 > move.attackInputName.Count - 1) continue;
                        //  If these match again, continue through the inputs until end is reached.
                        for (int n = i + 1; n < move.attackInputName.Count - 1; n++)
                        {
                            //  If they match AGAIN, use this to loop through the rest of the move
                            for (int k = j + 1; k < storedInputs.Queue.Count; k++)
                            {
                                if (n == move.attackInputName.Count - 1)
                                {
                                    if (move.attackInputName[n] == atk.inputContext.action.name)
                                        closestMatch = move;
                                    break;
                                }

                                var jMatch = storedInputs.Queue[k].name;
                                if (move.attackInputName[n] == jMatch)
                                    if (move.attackInputName[n + 1] == atk.inputContext.action.name)
                                    {
                                        closestMatch = move;
                                        return closestMatch;
                                    }

                                break;

                            }
                        }
                    }
                }
            }
            return closestMatch;
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
               GetCombatState() != CharacterState.CombatState.Hitstun || GetCombatState() == CharacterState.CombatState.Free;
    }

}
    
