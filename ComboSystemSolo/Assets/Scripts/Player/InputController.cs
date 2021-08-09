using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class InputController : PlayerComponent
    {
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
            cMovement.ZeroMovement();
        
        }
        #endregion
        #region Locked State
        public void OnLockMovement(InputAction.CallbackContext context)
        {
        
        }

        public void OnLockJump(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
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
            cMovement.ZeroMovement();
        
        }
        #endregion
    }
}
