using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShark : Shark
{
    public SharkSpawner spawner;
    new void Update()
    {
        base.Update();
        if (cooldownTimer < - 2 * cooldown) {
            Attack();
        }
    }

     public override void TakeDamage(AttackType attackType)
    {
        base.TakeDamage(attackType);
    }


    public override void Die() {
        spawner.RemoveShark(this);
        if (sharkComponents.Count > 0 && Random.Range(0.0f, 1.0f) < 0.35f) {
            sharkComponents[Random.Range(0, sharkComponents.Count)].Drop();
        }
        Destroy(gameObject);
    }

}
