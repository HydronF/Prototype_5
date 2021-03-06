using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingComponent : SharkComponent
{
    // Start is called before the first frame update
    void Start()
    {
        shark.ActivateFlying();
    }
}
