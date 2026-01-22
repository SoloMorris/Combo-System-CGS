using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace Player
{
    public class ActionQueue
    {
        //  Queue to store inputs that could not be executed.
        //  List instead of queue because queue doesn't allow much control
        public List<ActionQueueEntry> Queue = new List<ActionQueueEntry>();
    
        /// <summary>
        /// If an input cannot be executed, convert it and add it to the input Queue.
        /// </summary>
        /// <param name="entryInput"></param>
        public void AddInputToQueue(ActionInput entryInput)
        {
            var newEntry = new ActionQueueEntry(entryInput);
            newEntry.lifeTime = PlayerAttacks.INPUTBUFFER;
            Queue.Add(newEntry);
        }

        /// <summary>
        /// Ticks down each entry in the action queue. If they've reached 0, remove them from the queue.
        /// </summary>
        private void HandleQueueTimer()
        {
            //  Tick down each entry
            foreach (var entry in Queue)
                entry.countdownComplete = entry.Countdown();

            //  Remove any inputs that are at 0
            bool cleared = false;
            do
            {
                cleared = true;
                foreach (var entry in Queue)
                {
                    if (!entry.countdownComplete) continue;
                    Queue.Remove(entry);
                    cleared = false;
                    break;
                }
            } while (!cleared);

        }

        public void UpdateQueue()
        {
            HandleQueueTimer();
        }
        
    }
    
    /// <summary>
    /// Used to store ActionInputs in a queue.
    /// Lifetime will tick down over time. When it hits 0, it will be removed from the queue.
    /// </summary>
    public class ActionQueueEntry
    {
        public ActionQueueEntry(ActionInput _action)
        {
            action = _action;
        }
        public ActionInput action;
        public float lifeTime = PlayerAttacks.INPUTBUFFER;

        /// <summary>
        /// Ticks down this entry's time in the queue. When it hits 0, clear it from the queue.
        /// </summary>
        /// <returns></returns>
        public bool Countdown()
        {
            lifeTime -= Time.deltaTime;
            return lifeTime <= 0.0f;
        }

        public bool countdownComplete;

    }
    
    public class ActionQueueEntryString
    {
        public ActionQueueEntryString(string title)
        {
            name = title;
        }
        public string name;
        public float lifeTime = PlayerAttacks.INPUTBUFFER;

        /// <summary>
        /// Ticks down this entry's time in the queue. When it hits 0, clear it from the queue.
        /// </summary>
        /// <returns></returns>
        public bool Countdown()
        {
            lifeTime -= Time.deltaTime;
            return lifeTime <= 0.0f;
        }

        public bool countdownComplete;

    }
    /// <summary>
    /// Struct to store InputAction contexts. Putting it here so it's easier to type
    /// And also to future proof it. Totally.
    /// </summary>
    public struct ActionInput
    {
        public ActionInput(InputAction.CallbackContext context)
        {
            inputContext = context;
        }
        public InputAction.CallbackContext inputContext;
    }
    
    public struct ActionInputString
    {
        public ActionInputString(string title)
        {
            name = title;
        }

        private string name;
    }
}

public class ActionQueueString
{
    public float lifetime = PlayerAttacks.SPECIALMOVEBUFFER;
    //  Queue to store inputs that could not be executed.
    //  List instead of queue because queue doesn't allow much control
    public List<ActionQueueEntryString> Queue = new List<ActionQueueEntryString>();
    
    /// <summary>
    /// If an input cannot be executed, convert it and add it to the input Queue.
    /// </summary>
    /// <param name="entryInput"></param>
    public void AddInputToQueue(string entryInput)
    {
        var newEntry = new ActionQueueEntryString(entryInput);
        newEntry.lifeTime = lifetime;
        Queue.Add(newEntry);
    }

    /// <summary>
    /// Ticks down each entry in the action queue. If they've reached 0, remove them from the queue.
    /// </summary>
    private void HandleQueueTimer()
    {
        //  Tick down each entry
        foreach (var entry in Queue)
            entry.countdownComplete = entry.Countdown();

        //  Remove any inputs that are at 0
        bool cleared = false;
        do
        {
            cleared = true;
            foreach (var entry in Queue)
            {
                if (!entry.countdownComplete) continue;
                Queue.Remove(entry);
                cleared = false;
                break;
            }
        } while (!cleared);

    }

    public void UpdateQueue()
    {
        HandleQueueTimer();
    }
        
}
