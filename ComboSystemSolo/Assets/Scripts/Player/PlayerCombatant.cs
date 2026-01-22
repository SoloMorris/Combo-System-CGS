using System.Linq.Expressions;
using UnityEngine;

namespace Player
{
    public class PlayerCombatant : Combatant
    {

        public override void ApplyEffectsFromAttack(AttackInstance atk)
        {
            foreach (Effect fx in atk.attackData.attachedEffects)
            {
                if (!CompareTag(fx.targetTag))
                    continue;

                if (state.currentCombatState != fx.applyCondition)
                    continue;

                if (!HasHitConditionBeenMet(fx, atk))
                    continue;

                // Enforce per-target apply limit
                int currentCount = 0;
                _activeEffectCounts.TryGetValue(fx, out currentCount);

                // Treat 0 as "unlimited" to avoid accidental lockout from default int value
                var max = fx.timesCanBeApplied;
                if (max > 0 && currentCount >= max)
                    continue;

                _activeEffectCounts[fx] = currentCount + 1;
                underEffects.Add(new EffectInstance(fx));
            }

            bool HasHitConditionBeenMet(Effect fx, AttackInstance atk)
            {
                if (!fx.attackMustHit) return true;
                return atk.hitCount >= fx.howManyTimes;
            }

            // Optionally apply immediately (keeps behavior consistent with Combatant)
            HandleEffects();
        }



        protected override void CheckEffectDuration()
        {
            var check = true;
            do
            {
                check = true;
                foreach (var inst in underEffects)
                {
                    var fx = inst.effectData;
                    if (inst.timer >= fx.effectDuration)
                    {
                        check = false;
                        if (fx.forceState && fx.returnAfterEnd)
                            SetCombatState(CharacterState.CombatState.Neutral);
                        if (fx.forceMovementState && fx.returnAfterEnd)
                            SetMovementState(CharacterState.MovementState.Neutral);

                        if (fx.endsAttackEarly)
                            GetComponent<PlayerAttacks>().TryEndAttack(GetComponent<PlayerAttacks>().GetActiveAttack());

                        // Allow this Effect to be applied again once the current instance ends.
                        if (_activeEffectCounts.TryGetValue(fx, out var count))
                        {
                            count = Mathf.Max(0, count - 1);
                            if (count == 0) _activeEffectCounts.Remove(fx);
                            else _activeEffectCounts[fx] = count;
                        }

                        underEffects.Remove(inst);
                        break;
                    }
                }
            } while (!check);
        }
    }

}
