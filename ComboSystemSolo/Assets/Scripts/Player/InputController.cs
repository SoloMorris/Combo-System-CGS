using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class InputController : PlayerComponent
    {
        private PlayerInput map;

        private void Awake()
        {
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
            map.SwitchCurrentActionMap("Locked");

        }
        #endregion
        #region Locked State
        
        public void OnLockUp(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            print("Up");
            cCombatControls.ConvertInputAndExecute(context, true);
        }
        public void OnLockDown(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            print("Down");
            cCombatControls.ConvertInputAndExecute(context, true);
        }
        public void OnLockLeft(InputAction.CallbackContext context)
        {
            print("Left");
            cCombatControls.ConvertInputAndExecute(context, true);
        }
        public void OnLockRight(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            print("Right");
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
            if (!context.canceled) return;
            map.SwitchCurrentActionMap("Neutral");
        
        }
        #endregion
    }
}
