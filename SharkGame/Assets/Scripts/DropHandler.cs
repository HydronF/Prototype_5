using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropHandler : MonoBehaviour
{
    public UIManager uiManager;
    // Start is called before the first frame update
    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        Destroy(transform.parent.gameObject, 5.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            uiManager.ShowPickupPrompt(transform.parent);
            other.GetComponent<PlayerShark>().SetPotentialPickup(transform.parent.GetComponent<SharkComponent>());
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player") {
            uiManager.HidePickupPrompt(transform.parent);
            other.GetComponent<PlayerShark>().RemovePotentialPickup();
        }
    }

    void OnDestroy() {
        uiManager.RemovePickupPrompt(transform.parent);
    }

}
