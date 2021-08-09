using UnityEngine;

namespace Player
{
    public class PlayerComponent : CharacterComponent
    {
        [Header("Components")]
        public PlayerAttacks cAttacks;
        public PlayerMovement cMovement;
        public PlayerAnimation cAnimation;
        public CombatControls cCombatControls;
        public HitboxCheck cHitBox;

    
        protected override void AssignComponents()
        {
            cAttacks = GetComponent<PlayerAttacks>();
            cMovement = GetComponent<PlayerMovement>();
            cAnimation = GetComponent<PlayerAnimation>();
            cCombatControls = GetComponent<CombatControls>();
            cHitBox = GetComponentInChildren<HitboxCheck>();
            state = GetComponent<CharacterState>();
            MyBody = GetComponent<Rigidbody2D>();
        }
        
    }
}
