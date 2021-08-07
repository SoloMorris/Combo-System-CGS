using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Sprites;
using UnityEngine;

public class Combatant : CharacterComponent
{
    public int health;
    public Attack lastHitBy;
    public List<Effect> underEffects = new List<Effect>();
    
    private Vector2 storedVelocity; //Velocity stores when the character is in hitstun. Applies when they exit hitstun.
    private bool storageActive = false;
    public CharacterState.CombatState pGetCombatState()
    {
        return state.currentCombatState;
    }

    protected virtual void Start()
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
            
        foreach (Effect fx in atk.attachedEffects)
            underEffects.Add(fx);
        lastHitBy = atk;
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
                if (fx.damageInstant)
                    health -= fx.eDamage;
                else
                    health -= (fx.eDamage / (int)fx.effectDuration);

                if (fx.knockbackInstant)
                    TryApplyVelocity(fx.knockbackDirection * fx.knockbackDist);
                else
                    TryApplyVelocity(fx.knockbackDirection * (fx.knockbackDist * fx.effectDuration));
                
                if (fx.forceState)
                    state.SetCombatState(fx.ForcedState);

                fx.TickEffect();
                print("I am under effect aaaaaa");
            }
        }
        void CheckEffectDuration()
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
                        if (effect.forceState)
                            state.SetCombatState(CharacterState.CombatState.Neutral);
                        effect.Reset();
                        underEffects.Remove(effect);
                        break;
                    }
                }
            } while (!check);
        }
    }

    /// <summary>
    /// Check if the combatant is able to have velocity applied. If not, store it to be applied later.
    /// </summary>
    /// <param name="desiredVelocity"></param>
    private void TryApplyVelocity(Vector2 desiredVelocity)
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
}
