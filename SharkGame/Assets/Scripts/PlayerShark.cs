using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShark : Shark
{
    public List<GameObject> componentPrefabs;
    SharkComponent potentialPickup;
    int pickupType = -1;

    new void Update() {
        base.Update();
        uiManager.UpdateCooldown(cooldownTimer, cooldown);
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dir = GetTargetDir(mousePos);
        if (Input.GetMouseButton(0)) {
            Attack();
        }
        if (Input.GetMouseButtonDown(1)) {
            if (potentialPickup != null) {
                Pickup();
            }
        }
    }
    
    void FixedUpdate()
    {
        RotateTowards(dir);
        MoveTowards(dir);
    }

    public override void TakeDamage()
    {
        StartCoroutine(DamageAnim());
    }

    IEnumerator DamageAnim() {
        GetComponent<SpriteRenderer>().color = damageColor;
        yield return new WaitForSeconds(0.5f);
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void Pickup() {
        Destroy(potentialPickup.gameObject);
        potentialPickup = null;
        SharkComponent newComp = Instantiate(componentPrefabs[pickupType], transform.position, transform.rotation, transform).GetComponent<SharkComponent>();
        if (GetComponent<SpriteRenderer>().flipY) { 
            newComp.SetMirror(true);
        }
        newComp.shark = this;
    }

    public void SetPotentialPickup(SharkComponent comp) {
        potentialPickup = comp;
        if (comp is CannonComponent) pickupType = 0;
        else if (comp is TornadoComponent) pickupType = 1;
        else if (comp is WingComponent) pickupType = 2;
        else if (comp is CoilComponent) pickupType = 3;
    }

    public void RemovePotentialPickup() {
        potentialPickup = null;
    }
}
