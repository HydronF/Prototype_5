using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    Transform player;
    AudioSource hitSound;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Explode() {
        hitSound.Play();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            Explode(); 
            Destroy(gameObject, 0.5f);
        }
    }
}
