using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) {
        if (other.transform.tag != transform.parent.tag) {
            Shark shark = other.GetComponent<Shark>();
            if (shark != null) {
                shark.TakeDamage();
                Debug.Log("damage!");
            }
        }
    }
}
