using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark : MonoBehaviour
{
    public GameObject player;
    public GameObject meleeHitBox;
    public PixelManager pixelManager;
    protected List<SharkComponent> sharkComponents;
    public Vector2 buoyantForce;
    public float waterPropulsion;
    public float airPropulsion;
    protected bool canFly = false;
    protected SharkComponent attackComp;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        pixelManager = FindObjectOfType<PixelManager>();
        sharkComponents = new List<SharkComponent>();
        attackComp = null;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate() {
        Vector2 dir = GetTargetDir(player.transform.position);
        RotateTowards(dir);
        MoveTowards(dir);
    }

    protected Vector2 GetTargetDir(Vector3 targetPos) {
        Vector3 diff = targetPos - transform.position;
        Vector2 diff2D = new Vector2(diff.x, diff.y);
        diff2D.Normalize();
        return diff2D;
    }

    protected void RotateTowards(Vector2 toTarget) {
        if (toTarget.x > 0.0f) {
            if (!GetComponent<SpriteRenderer>().flipY) {
                GetComponent<SpriteRenderer>().flipY = true;
                foreach (SharkComponent comp in sharkComponents) {
                    GetComponent<SpriteRenderer>().flipY = true;
                    comp.SetMirror(true);
                }
            }
        }
        else if (toTarget.x < 0.0f) {
            if (GetComponent<SpriteRenderer>().flipY) {
                GetComponent<SpriteRenderer>().flipY = false;
                foreach (SharkComponent comp in sharkComponents) {
                    GetComponent<SpriteRenderer>().flipY = false;
                    comp.SetMirror(false);
                }
            }
        }
        float targetZ = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
        GetComponent<Rigidbody2D>().MoveRotation(Quaternion.Euler(0f, 0f, targetZ + 180.0f));

    }

    protected void MoveTowards(Vector2 toTarget) {
        PixelContent currPixel = pixelManager.GetContentWorld(transform.position);
        if (!canFly && currPixel == PixelContent.Empty) {
            GetComponent<Rigidbody2D>().drag = 0.5f;
        }
        else if (currPixel == PixelContent.Water) {
            GetComponent<Rigidbody2D>().drag = 3.0f;
            GetComponent<Rigidbody2D>().AddForce(buoyantForce);
            GetComponent<Rigidbody2D>().AddForce(waterPropulsion * toTarget);
        }
    }

    protected void Attack() {
        if (attackComp == null) {
            StartCoroutine(MeleeAttack());
        }
        else {
            attackComp.Attack();
        }
    }


    IEnumerator MeleeAttack() {
        meleeHitBox.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        meleeHitBox.SetActive(false);
    }

    public void ActivateFlying() {
        canFly = true;
    }

    public void NewAttack(SharkComponent newAttackComp) {
        if (attackComp != null) {
            sharkComponents.Remove(attackComp);
            Destroy(attackComp.gameObject);
        }
        attackComp = newAttackComp;
    }

}
