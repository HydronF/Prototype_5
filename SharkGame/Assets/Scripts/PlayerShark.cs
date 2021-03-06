using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShark : Shark
{
    public GameObject CannonPrefab;

    // Start is called before the first frame update
    new void Start() {
        base.Start();
        
        SharkComponent cannon = Instantiate(CannonPrefab, transform).GetComponent<CoilComponent>();
        cannon.shark = this;
    }
    new void Update() {
        base.Update();
        uiManager.UpdateCooldown(cooldownTimer, cooldown);
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dir = GetTargetDir(mousePos);
        if (Input.GetMouseButtonDown(0)) {
            Attack();
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
}
