using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Player
{
    public class CombatControls : PlayerComponent
    {
        //GOAL create an input queue
        private ActionQueue actionQueue;
        // PlayerAttacks script to enable attacks
        
        private void Awake()
        {
            actionQueue = new ActionQueue();
        }

        private void Start()
        {
            AssignComponents();
        }

        private void Update()
        {
            UpdateActionQueue();
            TryExecuteFromQueue();
        }

        private void TryExecuteFromQueue()
        {
            if (actionQueue.Queue.Count <= 0) return;
            if (TryExecuteInput(actionQueue.Queue[0].action))
                actionQueue.Queue.Remove(actionQueue.Queue[0]);
        }
        /// <summary>
        /// Takes an input from InputController and converts it to an ActionInput.
        /// Tries to execute it. If it can't be executed, add it to the input queue.
        /// </summary>
        /// <param name="context">The CallbackContext of the input to measure it's press state</param>
        public void ConvertInputAndExecute(InputAction.CallbackContext context, bool isDirection = false)
        {
            if (isDirection)
            {
                var dir = new ActionInput(context);
                    cAttacks.storedInputs.AddInputToQueue(dir);
                return;
            }
            var action = new ActionInput(context);
            if (!TryExecuteInput(action))
                actionQueue.AddInputToQueue(action);
        }
        
        
        /// <summary>
        /// Takes an input and tries to execute it in PlayerAttacks.
        /// </summary>
        /// <param name="_input">ActionInput to execute.</param>
        /// <returns>True if successfully executes, false if it fails.</returns>
        private bool TryExecuteInput(ActionInput _input)
        {
            return cAttacks.TryStartAttack(_input);
        }

        private void UpdateActionQueue()
        {
            actionQueue?.UpdateQueue();
        }

    }
    





}