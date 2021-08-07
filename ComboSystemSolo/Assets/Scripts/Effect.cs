using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Combat/Effect")]
public class Effect : ScriptableObject
{
    // Effects are NON-GENERIC things that happen to an actor once hit by an attack with said effect applied
    // i.e Launch effect will apply knockbackDirection (upwards Force) 

    public string effectTag; // CMB = Combo
    public float effectDuration; //Length of effect. If 1, effect is instant.
    [HideInInspector] public float timer;

    public int eDamage; // Damage applied by effect
    public bool damageInstant;// if false, enemy is damaged over the effect duration

    public Vector2 knockbackDirection;// Direction the enemy is sent.
    public float knockbackDist;
    public bool knockbackInstant;// if false, enemy is knocked back over the effect duration

    public bool forceState;
    public CharacterState.CombatState ForcedState;

    // public Effect(int effectDuration,
    //               int eDamage,
    //               Vector2 knockbackDirection,
    //               float knockbackDist,
    //               bool damageInstant = true,
    //               bool knockbackInstant = true,
    //               string effectTag = "CMB")
    // {
    //     this.effectDuration = effectDuration;
    //     this.eDamage = eDamage;
    //     this.damageInstant = damageInstant;
    //     this.timer = 0;
    //     this.knockbackDirection = knockbackDirection;
    //     this.knockbackDist = knockbackDist;
    //     this.knockbackInstant = knockbackInstant;
    //     this.effectTag = effectTag;
    // }

    public void Reset()
    {
        timer = 0f;
    }
    public void TickEffect()
    {
        timer += Time.deltaTime;
    }
    
}
