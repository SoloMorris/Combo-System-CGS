using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class HitboxCheck : PlayerComponent
{

    public bool active;
    public Attack attack;
    private void Start()
    {
        AssignComponents();
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (!active || attack == null) return;
        if (collision.gameObject.CompareTag("Enemy"))
        {
            var enemy = collision.gameObject.GetComponent<Enemy>();
                
            // Ensure the same attack doesn't hit twice on accident -- maybe add a numberofhits to attack for multihits
            if (CannotHitEnemy(enemy))
                return;
            
            attack.attackHitEvent?.Invoke(collision.gameObject.transform.position);
            enemy.ReceiveAttack(attack);
            print("I hit " + enemy.name + " with " + attack.name + "!!!");
        }
    }

    private bool CannotHitEnemy(Enemy nme)
    {
        return nme.GetCombatState() == CharacterState.CombatState.Hitstun &&
               nme.lastHitBy.name == attack.name || attack.hit >= attack.timesCanHit;
    }
    public void Activate(Attack atk)
    {
        attack = atk;
        active = true;
    }

    public void Deactivate()
    {
        attack = null;
        active = false;
    }
}
