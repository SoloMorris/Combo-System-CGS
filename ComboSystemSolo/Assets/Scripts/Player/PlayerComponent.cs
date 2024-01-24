using UnityEngine;

namespace Player
{
    public class PlayerComponent : CharacterComponent
    {
        [Header("Components")]
        [HideInInspector] public PlayerAttacks cAttacks;
        [HideInInspector] public PlayerMovement cMovement;
        [HideInInspector] public PlayerAnimation cAnimation;
        [HideInInspector] public CombatControls cCombatControls;
        [HideInInspector] public HitboxCheck cHitBox;
        [HideInInspector] public PlayerCombatant combatant;

    
        protected override void AssignComponents()
        {
            cAttacks = GetComponent<PlayerAttacks>();
            cMovement = GetComponent<PlayerMovement>();
            cAnimation = GetComponent<PlayerAnimation>();
            cCombatControls = GetComponent<CombatControls>();
            cHitBox = GetComponentInChildren<HitboxCheck>();
            combatant = GetComponent<PlayerCombatant>();
            state = GetComponent<CharacterState>();
            MyBody = GetComponent<Rigidbody2D>();
        }
        
        protected override void AssignComponentsInParents()
        {
            cAttacks = GetComponentInParent<PlayerAttacks>();
            cMovement = GetComponentInParent<PlayerMovement>();
            cAnimation = GetComponentInParent<PlayerAnimation>();
            cCombatControls = GetComponentInParent<CombatControls>();
            cHitBox = GetComponentInParent<HitboxCheck>();
            combatant = GetComponentInParent<PlayerCombatant>();
            state = GetComponentInParent<CharacterState>();
            MyBody = GetComponentInParent<Rigidbody2D>();
        }
        
    }
}
