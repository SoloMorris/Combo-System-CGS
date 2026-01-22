using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Combat/Effect")]
public class Effect : ScriptableObject
{
    // Effects are NON-GENERIC things that happen to an actor once hit by an attack with said effect applied
    // i.e Launch effect will apply knockbackDirection (upwards Force) 

    /// <summary>
    /// // CMB = Combo or something lol
    /// </summary>
    public string effectTag; 
    /// <summary>
    /// //Length of effect. If 0, effect is instant.
    /// </summary>
    public float effectDuration; 
    [HideInInspector] public float timer;

    /// <summary>
    /// Who effect is applied to. Can be Player, Enemy, Ally, etc.
    /// </summary>
    public string targetTag;

    [Header("If target is Player")]
    /// <summary>
    /// If this is targeting the player, what state does the player need to be in for the effect to be applied?
    /// </summary>
    public CharacterState.CombatState applyCondition;

    public bool endsAttackEarly;

    /// <summary>
    /// Must the attack hit before the effect can be applied?
    /// </summary>
    public bool attackMustHit;

    /// <summary>
    /// If so, how many times?
    /// </summary>
    public int howManyTimes;
    
    [Header("Stat changes")]
    /// <summary>
    /// Damage applied by effect
    /// </summary>
    public int eDamage; 
    /// <summary>
    /// if false, enemy is damaged over the effect duration
    /// </summary>
    public bool damageInstant;

    [HideInInspector] public bool instantDamageApplied;

    /// <summary>
    /// Direction the enemy is sent.
    /// </summary>
    public Vector2 knockbackDirection;
    public float knockbackDist;
    
    /// <summary>
    /// if false, enemy is knocked back over the effect duration
    /// </summary>
    public bool knockbackInstant;

    [HideInInspector] public bool instantKnockbackApplied;
    /// <summary>
    /// State to force the target into over duration of effect.
    /// </summary>
    public bool forceState;
    public CharacterState.CombatState ForcedState;
    public bool forceMovementState;
    public CharacterState.MovementState ForcedMovementState;
    /// <summary>
    /// Return the affected combatant to neutral after effect ends?
    /// </summary>
    public bool returnAfterEnd;

    /// <summary>
    /// How many times can this effect be applied, if applicable?
    /// </summary>
    public int timesCanBeApplied;

    /// <summary>
    /// How many times this effect has already been applied.
    /// </summary>
    [HideInInspector] public int timesApplied;

    public void Reset()
    {
        timer = 0f;
        timesApplied = 0;
        instantDamageApplied = false;
        instantKnockbackApplied = false;
    }
    public void TickEffect()
    {
        timer += Time.deltaTime;
    }

    public void Apply()
    {
        if (timesApplied >= timesCanBeApplied) return;
        timesApplied++;
    }


}
public class EffectInstance 
{
    public Effect effectData;
    public float timer;
    public int timesApplied;
    public bool instantDamageApplied;
    public bool instantKnockbackApplied;

    public EffectInstance(Effect data) {
        effectData = data;
        Reset();
    }

    public void Reset() {
        timer = 0f;
        timesApplied = 0;
        instantDamageApplied = false;
        instantKnockbackApplied = false;
    }

    public void Tick() {
        timer += Time.deltaTime;
    }
}

