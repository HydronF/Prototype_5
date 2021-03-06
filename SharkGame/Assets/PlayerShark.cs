using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShark : Shark
{
    // Start is called before the first frame update
    void Start()
    {
        player = gameObject;
        pixelManager = FindObjectOfType<PixelManager>();
        sharkComponents = new List<SharkComponent>();
    }

    void FixedUpdate()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = GetTargetDir(mousePos);
        RotateTowards(dir);
        MoveTowards(dir);
        if (Input.GetMouseButtonUp(0)) {
            Attack();
        }
    }
}
