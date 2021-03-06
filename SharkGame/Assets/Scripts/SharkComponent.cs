using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkComponent : MonoBehaviour
{
    public Shark shark;
    public float cooldown;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        shark.AddSharkComponent(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void SetMirror(bool toMirror) {
        GetComponent<SpriteRenderer>().flipY = toMirror;
    }

    public virtual void Attack() {

    }

    protected virtual void OnDestroy() {
        if (shark != null) {
            shark.RemoveSharkComponent(this);
        }
    }
}
