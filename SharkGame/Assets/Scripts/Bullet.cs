using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    PixelManager pixelManager;
    AudioSource hitSound;
    // Start is called before the first frame update
    void Start()
    {
        pixelManager = FindObjectOfType<PixelManager>();
        hitSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {
        PixelContent currPixel = pixelManager.GetContentWorld(transform.position);

        if (currPixel == PixelContent.Empty) {
            GetComponent<Rigidbody2D>().drag = 0.5f;
        }
        else if (currPixel == PixelContent.Water) {
            GetComponent<Rigidbody2D>().drag = 3.0f;
        }

        if (Mathf.Abs(GetComponent<Rigidbody2D>().velocity.magnitude) < 0.4f) {
            Destroy(gameObject);
        }
    }

    void Explode() {
        hitSound.Play();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player" || other.tag == "Enemy") {
            Explode(); 
            other.GetComponent<Shark>().TakeDamage();
            Destroy(gameObject);
        }
        else if (other.tag == "Border") {
            Destroy(gameObject);
        }
    }
}
