using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    const float HIT_RECOIL = 5.0f;
    const float HIT_IMPACT = 7.0f;
    void OnTriggerEnter2D(Collider2D other) {
        if (other.transform.tag != transform.parent.tag) {
            Shark shark = other.GetComponent<Shark>();
            if (shark != null) {
                shark.TakeDamage(AttackType.Melee);
                if (shark.GetComponent<Rigidbody2D>() != null) {
                    Vector2 forceDir = new Vector2(shark.transform.position.x - transform.position.x,
                                                   shark.transform.position.y - transform.position.y).normalized;
                    shark.GetComponent<Rigidbody2D>().AddForce(forceDir * HIT_IMPACT, ForceMode2D.Impulse);
                    GetComponentInParent<Rigidbody2D>().AddForce(- forceDir * HIT_RECOIL, ForceMode2D.Impulse);
                }
            }

        }
    }
}
