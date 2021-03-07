using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkComponent : MonoBehaviour
{
    public Shark shark;
    public float cooldown;
    public GameObject dropHandler;
    protected PixelManager pixelManager;
    bool dropped = false;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        shark.AddSharkComponent(this);
        pixelManager = FindObjectOfType<PixelManager>();
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Collider2D>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate() {
        if (dropped) {
            if (pixelManager.GetContentWorld(transform.position) == PixelContent.Empty) {
                GetComponent<Rigidbody2D>().drag = 0.3f;
            }
            else {
                GetComponent<Rigidbody2D>().drag = 3.0f;
            }
        }
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

    public void Drop() {
        dropped = true;
        shark = null;
        var newDrop = Instantiate(dropHandler, transform.position, transform.rotation, transform).GetComponent<DropHandler>();
        transform.SetParent(transform.parent.parent);
        GetComponent<Rigidbody2D>().isKinematic = false;
        GetComponent<Collider2D>().enabled = true;

    }
}
