using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomEffect
{
    public Combatant host;
    public Combatant target;

    public void Assign(Combatant nHost, Combatant nTarget)
    {
        host = nHost;
        target = nTarget;
    }
    public virtual void ExecuteEffect()
    {
        
    }
}
