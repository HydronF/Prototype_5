using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShark : Shark
{
    public List<GameObject> componentPrefabs;
    // AudioLowPassFilter lowPassFilter;
    SharkComponent potentialPickup;
    PixelContent lastPixel = PixelContent.Empty;
    int pickupType = -1;
    public AudioSource splashSound;
    bool dead = false;

    new void Start() {
        base.Start();
        uiManager.UpdatePlayerHealth(currHealth, maxHealth);
        // lowPassFilter = FindObjectOfType<AudioLowPassFilter>();
        Instantiate(componentPrefabs[3], transform).GetComponent<SharkComponent>().shark = this;
        Instantiate(componentPrefabs[2], transform).GetComponent<SharkComponent>().shark = this;
        Instantiate(componentPrefabs[1], transform).GetComponent<SharkComponent>().shark = this;


    }

    new void Update() {
        if (!dead) {
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
            if (Input.GetMouseButtonDown(2)) {
                foreach (SharkComponent comp in sharkComponents) {
                    comp.Drop();
                }
                sharkComponents.Clear();
                attackComp = null;
            }
        }
    }
    
    void FixedUpdate()
    {
        if (!dead) {
            RotateTowards(dir);
            MoveTowards(dir);
            // if (currPixel == PixelContent.Empty) { lowPassFilter.enabled = false; }
            // else if (currPixel == PixelContent.Water) { lowPassFilter.enabled = true; }
            if (lastPixel != currPixel && !splashSound.isPlaying) {
                splashSound.Play();
            }
            lastPixel = currPixel;
        }
    }

    public override void TakeDamage(AttackType attackType)
    {
        base.TakeDamage(attackType);
        uiManager.UpdatePlayerHealth(currHealth, maxHealth);
        if (currHealth <= 0) {
            Die();
        }
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

    public override void Die() {
        dead = true;
        uiManager.DeathScreen();
    }

}
