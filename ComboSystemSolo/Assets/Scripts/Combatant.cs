using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combatant : CharacterComponent
{
    public int health;
    public Attack lastHitBy;

    // Runtime instances (DO NOT store/mutate Effect ScriptableObjects at runtime)
    public readonly List<EffectInstance> underEffects = new List<EffectInstance>();

    // Per-target application limits (replaces Effect.timesApplied which was shared global state)
    protected readonly Dictionary<Effect, int> _activeEffectCounts = new Dictionary<Effect, int>();

    protected Vector2 storedVelocity; //Velocity stores when the character is in hitstun. Applies when they exit hitstun.
    protected bool storageActive = false;
    private Coroutine _hitstunCoroutine;
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

    public virtual void ReceiveAttack(AttackInstance atk)
    {
        // victim hitstun
        ApplyHitStunSeconds(FramesToSeconds(atk.attackData.enemyHitLagFrames));

        health -= atk.attackData.damage;
        ApplyEffectsFromAttack(atk);
        lastHitBy = atk.attackData;
    }

    public virtual void ApplyEffectsFromAttack(AttackInstance atk)
    {
        foreach (Effect fx in atk.attackData.attachedEffects)
        {
            if (!CompareTag(fx.targetTag))
                continue;

            // Enforce per-target apply limit
            int currentCount = 0;
            _activeEffectCounts.TryGetValue(fx, out currentCount);

            // Treat 0 as "unlimited" to avoid accidental lockout from default int value
            var max = fx.timesCanBeApplied;
            if (max > 0 && currentCount >= max)
                continue;

            _activeEffectCounts[fx] = currentCount + 1;
            underEffects.Add(new EffectInstance(fx));
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
            foreach (var inst in underEffects)
            {
                var fx = inst.effectData;

                if (fx.eDamage > 0)
                    ApplyDamage();

                if (fx.knockbackDirection != Vector2.zero)
                    ApplyKnockback();

                if (fx.forceState)
                    SetCombatState(fx.ForcedState);
                if (fx.forceMovementState)
                    SetMovementState(fx.ForcedMovementState);

                inst.Tick();

                void ApplyDamage()
                {
                    if (fx.damageInstant)
                    {
                        if (!inst.instantDamageApplied)
                        {
                            health -= fx.eDamage;
                            inst.instantDamageApplied = true;
                        }
                    }
                    else
                    {
                        // Spread damage across duration (guard against 0 duration)
                        var denom = Mathf.Max(0.0001f, fx.effectDuration);
                        health -= (int)(fx.eDamage * (Time.deltaTime / denom));
                    }
                }

                void ApplyKnockback()
                {
                    if (fx.knockbackInstant)
                    {
                        if (!inst.instantKnockbackApplied)
                        {
                            TryApplyVelocity(fx.knockbackDirection * fx.knockbackDist);
                            inst.instantKnockbackApplied = true;
                        }
                    }
                    else
                    {
                        // Over-time knockback (guard against 0 duration)
                        var denom = Mathf.Max(0.0001f, fx.effectDuration);
                        TryApplyVelocity(fx.knockbackDirection * (fx.knockbackDist * (Time.deltaTime / denom)));
                    }
                }
            }
        }
       
    }

    protected virtual void CheckEffectDuration()
    {
        var check = true;
        do
        {
            check = true;
            foreach (var inst in underEffects)
            {
                var fx = inst.effectData;
                if (inst.timer >= fx.effectDuration)
                {
                    check = false;
                    if (fx.forceState && fx.returnAfterEnd)
                        SetCombatState(CharacterState.CombatState.Neutral);
                    if (fx.forceMovementState && fx.returnAfterEnd)
                        SetMovementState(CharacterState.MovementState.Neutral);

                    // Allow this Effect to be applied again once the current instance ends.
                    if (_activeEffectCounts.TryGetValue(fx, out var count))
                    {
                        count = Mathf.Max(0, count - 1);
                        if (count == 0) _activeEffectCounts.Remove(fx);
                        else _activeEffectCounts[fx] = count;
                    }

                    underEffects.Remove(inst);
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
            MyBody.linearVelocity = desiredVelocity;
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
            MyBody.linearVelocity = Vector2.zero;
            MyBody.gravityScale = 0;
        }
    }
    protected virtual void CheckIfDie()
    {
        if (health <= 0 && state.currentCombatState != CharacterState.CombatState.Hitstun) Die();
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
    protected void ApplyHitStunSeconds(float durationSeconds) {
        if (durationSeconds <= 0f) return;

        if (_hitstunCoroutine != null)
            StopCoroutine(_hitstunCoroutine);

        _hitstunCoroutine = StartCoroutine(hitstunCoroutine(durationSeconds));
    }

    private IEnumerator hitstunCoroutine(float durationSeconds) {
        var prevCombatState = state.currentCombatState;

        var anim = GetComponent<Animator>();
        var prevAnimSpeed = anim != null ? anim.speed : 1f;

        var prevVelocity = MyBody != null ? MyBody.linearVelocity : Vector2.zero;
        var prevGravity = MyBody != null ? MyBody.gravityScale : 1f;

        SetCombatState(CharacterState.CombatState.Hitstun);

        float t = 0f;
        while (t < durationSeconds) {
            if (anim != null) anim.speed = 1f;

            if (MyBody != null) {
                MyBody.linearVelocity = Vector2.zero;
                MyBody.gravityScale = 0f;
            }

            t += Time.unscaledDeltaTime;
            yield return null;
        }

        if (anim != null) anim.speed = prevAnimSpeed;

        if (MyBody != null) {
            MyBody.gravityScale = prevGravity;
            MyBody.linearVelocity = prevVelocity;
        }

        // Only revert if nobody else changed it during hitstun
        if (state.currentCombatState == CharacterState.CombatState.Hitstun)
            SetCombatState(prevCombatState);

        _hitstunCoroutine = null;
    }

    protected float FramesToSeconds(int frames) {
        if (frames <= 0) return 0f;
        return frames / CombatManager.COMBAT_FPS;
    }
}
