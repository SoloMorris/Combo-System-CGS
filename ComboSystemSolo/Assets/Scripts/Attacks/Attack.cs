using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

[CreateAssetMenu(menuName = "Combat/Attack")]
public class Attack : ScriptableObject
{

    public bool enabled;
    
    /// <summary>
    /// Whether the attack will hit an enemy who has recovered from its hitstun again.
    /// </summary>
    public int timesCanHit;
    public string name; // Used as an ID to find other attacks.
    public bool startsACombo; // True if this attack is prioritised above others with the same input.
    public int hitboxActiveFrame; // When the attack does damage.
    public int hitboxEndFrame; // When the attack stops doing damage.
    public int hitStun; // How long the enemy is stunned after being hit.
    public int playerHitLag; // How long everything is paused when the player lands a hit
    public int damage; // The amount the enemy's health is reduced by.
    public List<string> attackInputName = new List<string>(); // The single input needed for the attack to come out. Either Light or Heavy right now
    private BoxCollider2D hitbox; // Self explanatory
    public Attack prevAttack; // Used if the attack continues or ends a combo.
    public Animation attackAnimation; // Animation used by the attack.

    public List<Effect> attachedEffects; // Misc effects the attack applies such as DoT or position manipulation.

    [Header("Debugging")]
    public int frame = 0; //The current frame if the attack is in use, used in debugging.
    public bool hit;

    public delegate void AttackHitEvent();
    public delegate void AttackEndEvent(); // Set to true when attack is complete, signalling to end the move.

    public AttackHitEvent attackHitEvent;
    public AttackEndEvent attackEndEvent;
    

    // Add an effect to the attack that applies on hit.
    public void AttachEffect(Effect nEffect)   {   attachedEffects.Add(nEffect);   }

    public void Reset()
    {
        frame = 0;
    }
    public void Update()
    {
        frame++;
        if (frame >= hitboxActiveFrame && frame < hitboxEndFrame)
        {
            if (hit)
            {
                frame -= playerHitLag;
            }
        }
        else if (frame >= hitboxEndFrame)
        {
            Reset();           
            attackEndEvent?.Invoke();
        }
    }

}
