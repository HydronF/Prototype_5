using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShark : Shark
{
    public SharkSpawner spawner;
    new void Update()
    {
        base.Update();
        if (cooldownTimer < -cooldown) {
            Attack();
        }
    }

    public override void TakeDamage() {
        Die();
    }

    public void Die() {
        spawner.RemoveShark(this);
        Destroy(gameObject);
    }

}
