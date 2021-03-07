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
        if (sharkComponents.Count > 0 && Random.Range(0, 3) == 0) {
            sharkComponents[Random.Range(0, sharkComponents.Count)].Drop();
        }
        Destroy(gameObject);
    }

}
