using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkComponent : MonoBehaviour
{
    public Shark shark;
    public float cooldown;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMirror(bool toMirror) {
        GetComponent<SpriteRenderer>().flipX = toMirror;
    }

    public virtual void Attack() {

    }
}
