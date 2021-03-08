using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropHandler : MonoBehaviour
{
    public UIManager uiManager;
    Color glitterColor;

    // Start is called before the first frame update
    void Start()
    {
        glitterColor = new Color(0.0f, 0.0f, 0.0f, 0.4f);
        uiManager = FindObjectOfType<UIManager>();
        StartCoroutine(GlitterAnim());
        Destroy(transform.parent.gameObject, 8.0f);
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

    IEnumerator GlitterAnim() {
        yield return new WaitForSeconds(5.0f);
        while (true) { 
            transform.parent.GetComponent<SpriteRenderer>().color = glitterColor;
            yield return new WaitForSeconds(0.2f);
            transform.parent.GetComponent<SpriteRenderer>().color = Color.white;
            yield return new WaitForSeconds(0.2f);
        }
    }


}
