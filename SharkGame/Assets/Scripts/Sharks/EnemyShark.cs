using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShark : Shark
{
    public SharkSpawner spawner;
    bool dead = false;

    new void Start() {
        base.Start();
        player = GameObject.FindWithTag("Player").transform;
    }
    new void Update()
    {
        base.Update();
        if (cooldownTimer < - 2.0f * cooldown && !dead) {
            Attack();
        }
    }

    void FixedUpdate() {
        if (player != null && !dead) {
            dir = GetTargetDir(player.position);
            RotateTowards(dir);
            MoveTowards(dir);
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
        uiManager.TrackKill();
        dead = true;
        Destroy(gameObject, 0.5f);
    }

}
