using UnityEngine;

namespace Player
{
    public class PlayerComponent : CharacterComponent
    {
        [Header("Components")]
        public PlayerAttacks Attacks;
        public PlayerMovement Movement;
        public PlayerAnimation Animation;
        public CombatControls CombatControls;
        public HitboxCheck hitboxControl;

    
        protected override void AssignComponents()
        {
            Attacks = GetComponent<PlayerAttacks>();
            Movement = GetComponent<PlayerMovement>();
            Animation = GetComponent<PlayerAnimation>();
            CombatControls = GetComponent<CombatControls>();
            hitboxControl = GetComponentInChildren<HitboxCheck>();
            state = GetComponent<CharacterState>();
            MyBody = GetComponent<Rigidbody2D>();
        }
        
    }
}
