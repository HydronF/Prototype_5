using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropHandler : MonoBehaviour
{
    public GameObject pickupSprite;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(transform.parent.gameObject, 5.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            pickupSprite.SetActive(true);
            other.GetComponent<PlayerShark>().SetPotentialPickup(transform.parent.GetComponent<SharkComponent>());
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player") {
            pickupSprite.SetActive(false);
            other.GetComponent<PlayerShark>().SetPotentialPickup(transform.parent.GetComponent<SharkComponent>());
        }
    }

}
