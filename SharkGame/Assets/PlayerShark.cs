using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShark : Shark
{
    // Start is called before the first frame update

    void FixedUpdate()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = GetTargetDir(mousePos);
        RotateTowards(dir);
        MoveTowards(dir);
        if (Input.GetMouseButtonDown(0)) {
            Attack();
        }
    }
}
