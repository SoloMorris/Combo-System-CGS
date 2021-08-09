using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Player;
using UnityEngine;

public class UppercutEarlyExit : CustomEffect
{
    public override void ExecuteEffect()
    {
        var airbornecheck = host.GetMovementState() == CharacterState.MovementState.Airborne;
        var components = host.GetComponent<PlayerComponent>();
        if (airbornecheck)
        {
            components.cAttacks.OnAttackEnd(components.cAttacks.GetActiveAttack());
        }
    }
}
