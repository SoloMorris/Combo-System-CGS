using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class HitboxCheck : PlayerComponent
{

    public bool active;
    public Attack attack;
    private int timesHit = 0;
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
            if (enemy.GetCombatState() == CharacterState.CombatState.Hitstun &&
                enemy.lastHitBy.name == attack.name ||
                timesHit >= attack.timesCanHit)
                return;
            
            attack.attackHitEvent?.Invoke();
            enemy.ReceiveAttack(attack);
            timesHit++;
            print("I hit " + enemy.name + " with " + attack.name + "!!!");
        }
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
        timesHit = 0;
    }
}
