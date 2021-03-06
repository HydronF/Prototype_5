using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoComponent : SharkComponent
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        FindObjectOfType<PixelManager>().StartTornado(transform);
        transform.localPosition = new Vector3(0.0f, 0.0f, 0.1f);
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        FindObjectOfType<PixelManager>().StopTornado(transform);
    }
}
