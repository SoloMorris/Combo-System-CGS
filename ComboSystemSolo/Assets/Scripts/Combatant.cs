using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combatant : CharacterComponent
{
    public int health;
    public Attack lastHitBy;
    public List<Effect> underEffects = new List<Effect>();
    
    protected Vector2 storedVelocity; //Velocity stores when the character is in hitstun. Applies when they exit hitstun.
    protected bool storageActive = false;
    public CharacterState.CombatState pGetCombatState()
    {
        return state.currentCombatState;
    }

    protected void Start()
    {
        AssignComponents();
    }

    protected virtual void FixedUpdate()
    {
        UpdateState();
    }

    public virtual void ReceiveAttack(Attack atk)
    {
        health -= atk.damage;
        ApplyEffectsFromAttack(atk);
        lastHitBy = atk;
    }

    public virtual void ApplyEffectsFromAttack(Attack atk)
    {
        foreach (Effect fx in atk.attachedEffects)
            if (CompareTag(fx.targetTag))
            {
                fx.Apply();
                underEffects.Add(fx);
            }
        HandleEffects();
    }
    protected virtual void UpdateState()
    {
        CheckIfDie();
        HandleEffects();
    }
    protected virtual void HandleEffects()
    {
        if (storageActive)
            TryApplyVelocity(storedVelocity);
        
        HandleHitstun();

        
        if (underEffects.Count <= 0) return;
        ApplyEffects();
        CheckEffectDuration();
        
        void ApplyEffects()
        {
            foreach (var fx in underEffects)
            {
                if (fx.eDamage > 0)
                    ApplyDamage();

                if (fx.knockbackDirection != Vector2.zero)
                    ApplyKnockback();
                
                if (fx.forceState)
                    SetCombatState(fx.ForcedState);
                if (fx.forceMovementState)
                    SetMovementState(fx.ForcedMovementState);

                fx.TickEffect();

                void ApplyDamage()
                {
                    if (fx.damageInstant && !fx.instantDamageApplied)
                    {
                        health -= fx.eDamage;
                        fx.instantDamageApplied = true;
                    }
                    else 
                        health -= (fx.eDamage / (int)fx.effectDuration);
                }
                void ApplyKnockback()
                {
                    if (fx.knockbackInstant && !fx.instantKnockbackApplied)
                    {
                        TryApplyVelocity(fx.knockbackDirection * fx.knockbackDist);
                        fx.instantKnockbackApplied = true;
                    }
                    else
                        TryApplyVelocity(fx.knockbackDirection * (fx.knockbackDist * fx.effectDuration));
                }
            }
        }
       
    }

    protected virtual  void CheckEffectDuration()
    {
        var check = true;
        do
        {
            check = true;
            foreach (var effect in underEffects)
            {
                if (effect.timer >= effect.effectDuration)
                {
                    check = false;
                    if (effect.forceState && effect.returnAfterEnd)
                        SetCombatState(CharacterState.CombatState.Neutral);
                    if (effect.forceMovementState && effect.returnAfterEnd)
                        SetMovementState(CharacterState.MovementState.Neutral);
                    effect.Reset();
                    underEffects.Remove(effect);
                    break;
                }
            }
        } while (!check);
    }
    /// <summary>
    /// Check if the combatant is able to have velocity applied. If not, store it to be applied later.
    /// </summary>
    /// <param name="desiredVelocity"></param>
    protected void TryApplyVelocity(Vector2 desiredVelocity)
    {
        if (state.currentCombatState == CharacterState.CombatState.Hitstun)
        {
            storedVelocity = desiredVelocity;
            storageActive = true;
        }
        else
        {
            MyBody.velocity = desiredVelocity;
            storedVelocity = Vector2.zero;
            storageActive = false;
        }
    }
    protected virtual void HandleHitstun()
    {
        if (state.currentCombatState != CharacterState.CombatState.Hitstun)
            MyBody.gravityScale = 1f;
        else
        {
            MyBody.velocity = Vector2.zero;
            MyBody.gravityScale = 0;
        }
    }
    protected virtual void CheckIfDie()
    {
        if (health <= 0) Die();
    }
    protected virtual void Die()
    {
        Destroy(gameObject);
    }
    
    protected virtual void HandleGenericAnimation(Animator animator)
    {
        //  Handle hitstun
        {
            animator.SetBool("InHitstun", state.currentCombatState == CharacterState.CombatState.Hitstun);
        }

        //  Handle disabled state
        {
            if (state.currentMovementState == CharacterState.MovementState.Airborne &&
                state.currentCombatState == CharacterState.CombatState.Hitstun)
            {
                state.currentMovementState = CharacterState.MovementState.Disabled;
                print("Oof");
            }
        }

        //  If disabled, handle it in the animator
        {
            if (state.currentMovementState == CharacterState.MovementState.Disabled)
                animator.SetBool("Disabled", true);
        }

    }
}
