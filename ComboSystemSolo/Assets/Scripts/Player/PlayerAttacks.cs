using Player;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAttacks : PlayerComponent
{
    [Header("Attack Stuff")]
    [SerializeField] private AttackMovelist movelist;
    [SerializeField] private AttackMovelist specialMovelist;
    private AttackInstance activeAttackInstance;
    private Attack activeAttack => activeAttackInstance?.attackData;
    
    // Input buffer constants
    public static float INPUTBUFFER = 0.5f;
    public static float SPECIALMOVEBUFFER = 0.75f;
    
    // Coroutine references for proper management
    private Coroutine hitlagCoroutine;
    private Coroutine cancelWindowCoroutine;

    /// <summary>
    /// To store directional inputs for an attack.
    /// </summary>
    public ActionQueueString specialInputs = new ActionQueueString();
    
    private void Start()
    {
        AssignComponents();
        specialInputs.lifetime = 1.0f;
    }


    void Update()
    {
        specialInputs.UpdateQueue();
       
        EnsureState();
        void EnsureState()
        {
            if (state.currentCombatState == CharacterState.CombatState.Neutral && activeAttack)
            {
                print("catch states mismatched");
            }
        }
    }

    private void OnDestroy() {
        
        if (activeAttackInstance != null) TryEndAttack(activeAttackInstance);
    }

    #region Attack Delegates

    private void OnAttackHit(Vector3 contactLocation)
    {
        if (!IsAttackActive()) return;
        if (activeAttack.playVfxOnHit) {
            var vfxInstance = Instantiate(activeAttack.vfx, contactLocation, Quaternion.identity);
        }
        if (activeAttack.canBeCancelled)
        {
            OnCancellableAttackHit();
            return;
        }
        SetCombatState(CharacterState.CombatState.Hitlag);
        activeAttackInstance.hitCount++;
        if (activeAttack.playerHitLag > 0)
        {
            if (hitlagCoroutine != null) StopCoroutine(hitlagCoroutine);
            hitlagCoroutine = StartCoroutine(FreezeHitlag());
        }
    }
    
    private void OnCancellableAttackHit()
    {
        if (!IsAttackActive()) return;
        SetCombatState(CharacterState.CombatState.Free);
        activeAttackInstance.hitCount++;
        if (activeAttack.playerHitLag > 0)
        {
            if (cancelWindowCoroutine != null) StopCoroutine(cancelWindowCoroutine);
            cancelWindowCoroutine = StartCoroutine(FreezeCancelWindow());
        }
    }

    public IEnumerator FreezeHitlag()
    {
        float counter = 0f;
        var storedAtk = activeAttack;
        while (counter < storedAtk.playerHitLag)
        {
            if (activeAttack == null) yield break;
            GetComponent<Animator>().speed = 0f;
            MyBody.linearVelocity = Vector2.zero;
            counter++;
            yield return 0;
        }
        SetCombatState(CharacterState.CombatState.Attacking);
        GetComponent<Animator>().speed = 1f;
    }

    public IEnumerator FreezeCancelWindow()
    {
        float counter = 0f;
        var storedAtk = activeAttack;
        while (counter < storedAtk.playerHitLag)
        {
            if (activeAttack == null) yield break;
            GetComponent<Animator>().speed = 0f;
            MyBody.linearVelocity = Vector2.zero;
            counter++;
            yield return 0;
        }
        GetComponent<Animator>().speed = 1f;
        SetCombatState(CharacterState.CombatState.Attacking);
    }

    #endregion

    public void EndAttackAnimation(Attack attack) {
        if (activeAttackInstance!= null && activeAttackInstance.attackData == attack)
            TryEndAttack(activeAttackInstance);
    }
    public void TryEndAttack(AttackInstance atk)
    {
        if (atk == null || activeAttack == null || atk.attackData.attackName != activeAttack.attackName)
        {
            return;
        }
        
        SetCombatState(CharacterState.CombatState.Neutral);
        cHitBox.Deactivate();
        activeAttackInstance.attackHitEvent -= OnAttackHit;
        
        // Stop any running coroutines
        if (hitlagCoroutine != null)
        {
            StopCoroutine(hitlagCoroutine);
            hitlagCoroutine = null;
        }
        if (cancelWindowCoroutine != null)
        {
            StopCoroutine(cancelWindowCoroutine);
            cancelWindowCoroutine = null;
        }
        
        GetComponent<Animator>().speed = 1f;
        activeAttackInstance = null;
        print("Attack over");
    }

    #region Animation Events
    public void TryApplySelfEffect()
    {
        if (IsAttackActive())
            combatant.ApplyEffectsFromAttack(activeAttackInstance);
    }
    public void ActivateHitbox()
    {
        if (IsAttackActive())
            cHitBox.Activate(activeAttackInstance);
    }

    public void DeactivateHitbox()
    {
        if (IsAttackActive())
            cHitBox.Deactivate();
    }


    public void CheckAnimationIsCleared()
    {
        if (activeAttackInstance != null || state.currentCombatState == CharacterState.CombatState.Attacking)
            TryEndAttack(activeAttackInstance);
    }
    public void EnterRecovery()
    {
        if (IsAttackActive())
            SetCombatState(CharacterState.CombatState.Recovery);
    }

    #endregion
    public bool TryStartAttack(ActionInput atk)
    {
        if (!CheckStatesAllowAction()) return false;
        ExecuteAttackAction(atk);
        return true;
    }
    public void TryStartSpecial()
    {
        if (!CheckStatesAllowAction()) return;
        ExecuteAttackAction(new ActionInput());
    }
    
    public AttackInstance GetActiveAttack()
    {
        return activeAttackInstance;
    }

    private bool IsAttackActive()
    {
        return activeAttackInstance != null && activeAttackInstance.isActive;
    }

    /// <summary>
    /// Attempt to execute an attack action based on the given ActionInput.
    /// </summary>
    private void ExecuteAttackAction(ActionInput atk) {
        var attack = FindAttack(atk);
        if (attack == null) {
            Debug.Log("No valid attack found for input");
            return;
        }

        // If there is an attack being used, end it first
        if (state.currentCombatState == CharacterState.CombatState.Recovery) 
            TryEndAttack(activeAttackInstance);
        if (activeAttack != null) 
            activeAttackInstance.attackHitEvent -= OnAttackHit;

        // Start the new attack instance
        activeAttackInstance = new AttackInstance(attack);
        activeAttackInstance.isActive = true;
        activeAttackInstance.attackHitEvent += OnAttackHit;
        SetCombatState(CharacterState.CombatState.Attacking);
        cAnimation.PlayAnimation(activeAttack.attackAnimation);
    }
    private Attack FindAttack(ActionInput atk) {
        if (GetMovementState() == CharacterState.MovementState.Locked) return FindSpecialAttack();

        if (GetCombatState() == CharacterState.CombatState.Neutral) return FindComboStarter(atk);

        return FindComboAttack(atk);
    }
    private Attack FindComboStarter(ActionInput atk) {
        return movelist.attacks.Find(move =>
            move.enabled &&
            move.startsACombo &&
            move.aerialMove == !cMovement.grounded &&
            move.attackInputName.Count == 1 &&
            move.attackInputName[0] == atk.inputContext.action.name
        );
    }

    private Attack FindComboAttack(ActionInput atk) {
        if (activeAttack == null) return null;

        return movelist.attacks.Find(move =>
            move.enabled &&
            move.prevAttack != null &&
            move.prevAttack.name == activeAttack.name &&
            move.aerialMove == !cMovement.grounded &&
            move.attackInputName.Count == 1 &&
            move.attackInputName[0] == atk.inputContext.action.name
        );
    }

    private Attack FindSpecialAttack() {
        if (specialInputs.Queue.Count == 0) return null;

        // Try to match the longest sequence first
        var orderedMoves = specialMovelist.attacks
            .Where(m => m.enabled)
            .OrderByDescending(m => m.attackInputName.Count);

        foreach (var move in orderedMoves) {
            int matchLength = TryMatchInputSequence(move.attackInputName);

            if (matchLength == move.attackInputName.Count) {
                // Remove matched inputs from queue
                specialInputs.Queue.RemoveRange(0, matchLength);
                return move;
            }
        }

        return null;
    }
    private int TryMatchInputSequence(List<string> requiredInputs) {
        int matchCount = 0;

        for (int i = 0; i < requiredInputs.Count && i < specialInputs.Queue.Count; i++) {
            if (requiredInputs[i] == specialInputs.Queue[i].name)
                matchCount++;
            else
                break;
        }

        return matchCount;
    }


    /// <summary>
    /// Check if the movement and combat states currently allow for an attack to be performed.
    /// </summary>
    /// <returns>True if player can attack, false if not.</returns>
    private bool CheckStatesAllowAction()
    {
        if (GetMovementState() == CharacterState.MovementState.Disabled) return false;
        return GetCombatState() != CharacterState.CombatState.Attacking && 
               GetCombatState() != CharacterState.CombatState.Hitstun || GetCombatState() == CharacterState.CombatState.Free;
    }

}

