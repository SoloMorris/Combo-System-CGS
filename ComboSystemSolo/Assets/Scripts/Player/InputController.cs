using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class InputController : PlayerComponent
    {
        private PlayerInput map;

        private void Awake()
        {
            AssignComponents();
            map = GetComponent<PlayerInput>();
        }
        
        #region Neutral State
    
        public void OnMovement(InputAction.CallbackContext context)
        {
            cMovement.UpdateDirection(context.ReadValue<Vector2>());
        }
        public void OnJump(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            cMovement.Jump();
        }
        public void OnLight(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            cCombatControls.ConvertInputAndExecute(context);
        }
        public void OnHeavy(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            cCombatControls.ConvertInputAndExecute(context);
        }
        public void OnLock(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            cMovement.SetLocked();
            map.SwitchCurrentActionMap("Locked");

        }
        #endregion
        #region Locked State
        
        public void OnLockDirection(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            cCombatControls.ConvertInputAndExecute(context, true);
        }

        public void OnLockJump(InputAction.CallbackContext context)
        {
            
            if (!context.performed) return;
        }
        public void OnLockLight(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            cCombatControls.ConvertInputAndExecute(context);

        }
        public void OnLockHeavy(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            cCombatControls.ConvertInputAndExecute(context);
        }
        public void OnUnlock(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            cMovement.SetUnlocked();
            map.SwitchCurrentActionMap("Neutral");
        
        }

        public void OnExecute(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            cAttacks.TryStartSpecial();
        }

        public void OnDebug(InputAction.CallbackContext context) {
            if (!context.performed) return;
            CombatManager.Instance.SpawnEnemy();
        }
        #endregion
    }
}
