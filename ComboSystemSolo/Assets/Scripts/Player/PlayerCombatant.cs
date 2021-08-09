using System.Linq.Expressions;

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

        
    }
}
