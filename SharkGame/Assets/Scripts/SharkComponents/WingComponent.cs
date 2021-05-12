using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingComponent : SharkComponent
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        shark.ActivateFlying();
        transform.localPosition = new Vector3(0.0f, 0.0f, -0.3f);
    }

    public override void Drop()
    {
        if (shark != null) {
            shark.DeactivateFlying();
        }
        base.Drop();
    }
}
