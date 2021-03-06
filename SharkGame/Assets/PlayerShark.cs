using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShark : Shark
{
    public GameObject CannonPrefab;

    // Start is called before the first frame update
    new void Start() {
        base.Start();
        
        SharkComponent cannon = Instantiate(CannonPrefab, transform).GetComponent<CannonComponent>();
        cannon.shark = this;
        NewAttack(cannon);
    }
    new void Update() {
        base.Update();
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
}
