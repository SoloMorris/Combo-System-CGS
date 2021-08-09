using System.Linq.Expressions;
using UnityEngine;

namespace Player
{
    public class PlayerCombatant : Combatant
    {

        public override void ApplyEffectsFromAttack(Attack atk)
        {
            foreach (Effect fx in atk.attachedEffects)
                if (CompareTag(fx.targetTag) && state.currentCombatState == fx.applyCondition
                                             && HasHitConditionBeenMet(fx, atk))
                {
                    fx.Apply();
                    underEffects.Add(fx);
                }

            bool HasHitConditionBeenMet(Effect fx, Attack atk)
            {
                if (!fx.attackMustHit) return true;
                return atk.hit >= fx.howManyTimes;
            }
        }

        protected override void CheckEffectDuration()
        {
            var check = true;
            do
            {
                check = true;
                foreach (var effect in underEffects)
                {
                    if (effect.timer >= effect.effectDuration)
                    {
                        check = false;
                        if (effect.forceState && effect.returnAfterEnd)
                            SetCombatState(CharacterState.CombatState.Neutral);
                        if (effect.forceMovementState && effect.returnAfterEnd)
                            SetMovementState(CharacterState.MovementState.Neutral);
                        effect.Reset();
                        if (effect.endsAttackEarly)
                            GetComponent<PlayerAttacks>().OnAttackEnd(GetComponent<PlayerAttacks>().GetActiveAttack());
                        underEffects.Remove(effect);
                        break;
                    }
                }
            } while (!check);
        }
    }

}
