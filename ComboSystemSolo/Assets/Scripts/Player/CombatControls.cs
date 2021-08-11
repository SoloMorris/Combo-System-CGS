using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

namespace Player
{
    public class CombatControls : PlayerComponent
    {
        //GOAL create an input queue
        private ActionQueue actionQueue;

        /// <summary>
        /// Prevents more than one direction input being sent per frame.
        /// </summary>
        private string prevDirection = "";
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
                var xy = dir.inputContext.ReadValue<Vector2>();
                CreateInputDirections(xy, dir);
                if (dir.inputContext.action.name == prevDirection || dir.inputContext.action.name == "Movement") return;
                prevDirection = dir.inputContext.action.name;
                cAttacks.storedInputs.AddInputToQueue(dir.inputContext.action.name);
                return;
            }
            var action = new ActionInput(context);
            if (!TryExecuteInput(action))
                actionQueue.AddInputToQueue(action);

            void CreateInputDirections(Vector2 xy, ActionInput inp)
            {
                var deadZone = 0.45f;
                    // Create Right-side inputs
                    if (xy.x >= deadZone)
                    {
                        if (xy.y >= deadZone)
                            inp.inputContext.action.Rename("Up-Right");
                        else if (xy.y <= -deadZone)
                            inp.inputContext.action.Rename("Down-Right");
                        else
                            inp.inputContext.action.Rename("Right");

                    }
                    //Create Left-side inputs
                    else if (xy.x <= -deadZone)
                    {
                        if (xy.y >= deadZone)
                            inp.inputContext.action.Rename("Up-Left");
                        else if (xy.y <= -deadZone)
                            inp.inputContext.action.Rename("Down-Left");
                        else
                            inp.inputContext.action.Rename("Left");

                    }
                    //  Create vertical inputs
                    else if (xy.y >= deadZone)
                        inp.inputContext.action.Rename("Up");
                    else if (xy.y <= -deadZone)
                        inp.inputContext.action.Rename("Down");
            }
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