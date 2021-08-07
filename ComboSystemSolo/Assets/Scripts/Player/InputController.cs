using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class InputController : PlayerComponent
    {
        #region Neutral State
    
        public void OnMovement(InputAction.CallbackContext context)
        {
            Movement.UpdateDirection(context.ReadValue<Vector2>());
        }
        public void OnJump(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            Movement.Jump();
        }
        public void OnLight(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            CombatControls.ConvertInputAndExecute(context);
        }
        public void OnHeavy(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            CombatControls.ConvertInputAndExecute(context);
        }
        public void OnLock(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            Movement.ZeroMovement();
        
        }
        #endregion
        #region Locked State
        public void OnLockMovement(InputAction.CallbackContext context)
        {
        
        }

        public void OnLockJump(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            Movement.Jump();
        }
        public void OnLockLight(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

        }
        public void OnLockHeavy(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
        }
        public void OnUnlock(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            Movement.ZeroMovement();
        
        }
        #endregion
    }
}
