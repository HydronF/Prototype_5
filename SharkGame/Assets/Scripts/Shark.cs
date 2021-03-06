using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark : MonoBehaviour
{
    public GameObject player;
    public GameObject meleeHitBox;
    protected PixelManager pixelManager;
    protected UIManager uiManager;
    public List<SharkComponent> sharkComponents;
    public Transform waterProbes;
    public Vector2 buoyantForce;
    public float waterPropulsion;
    public float airPropulsion;
    public float attackPropulsion;
    public float cooldown;
    protected bool canFly = false;
    protected Vector2 dir = Vector2.zero;
    protected SharkComponent attackComp;
    protected float cooldownTimer;

    // Start is called before the first frame update
    protected void Start()
    {
        player = GameObject.FindWithTag("Player");
        uiManager = FindObjectOfType<UIManager>();
        pixelManager = FindObjectOfType<PixelManager>();
        sharkComponents = new List<SharkComponent>();
        attackComp = null;
        meleeHitBox.SetActive(false);
        cooldownTimer = 0.0f;
    }

    // Update is called once per frame
    protected void Update()
    {
        cooldownTimer -= Time.deltaTime;
        uiManager.UpdateCooldown(cooldownTimer, cooldown);
    }

    void FixedUpdate() {
        dir = GetTargetDir(player.transform.position);
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
                    comp.SetMirror(true);
                }
                meleeHitBox.transform.localPosition = new Vector3(
                    meleeHitBox.transform.localPosition.x,
                    -meleeHitBox.transform.localPosition.y,
                    meleeHitBox.transform.localPosition.z
                );
            }
        }
        else if (toTarget.x < 0.0f) {
            if (GetComponent<SpriteRenderer>().flipY) {
                GetComponent<SpriteRenderer>().flipY = false;
                foreach (SharkComponent comp in sharkComponents) {
                    comp.SetMirror(false);
                }
                meleeHitBox.transform.localPosition = new Vector3(
                    meleeHitBox.transform.localPosition.x,
                    -meleeHitBox.transform.localPosition.y,
                    meleeHitBox.transform.localPosition.z
                );
            }
        }
        float targetZ = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
        GetComponent<Rigidbody2D>().MoveRotation(Quaternion.Euler(0f, 0f, targetZ + 180.0f));

    }

    protected void MoveTowards(Vector2 toTarget) {
        PixelContent currPixel = PixelContent.Empty;
        if (pixelManager.GetContentWorld(transform.position) == PixelContent.Water) {
            currPixel = PixelContent.Water;
        }
        foreach (Transform child in waterProbes) {
            if (pixelManager.GetContentWorld(child.position) == PixelContent.Water) {
                currPixel = PixelContent.Water;
                break;
            }
        }
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
        if (cooldownTimer < 0.0f) {
            cooldownTimer = cooldown;
            if (attackComp == null) {
                StartCoroutine(MeleeAttack());
            }
            else {
                attackComp.Attack();
            }
        }
    }


    IEnumerator MeleeAttack() {
        meleeHitBox.SetActive(true);
        Vector2 forceDir = - new Vector2(transform.right.x, transform.right.y).normalized;
        GetComponent<Rigidbody2D>().AddForce(forceDir * attackPropulsion, ForceMode2D.Impulse);
        cooldownTimer = cooldown;
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
        sharkComponents.Add(newAttackComp);
        attackComp = newAttackComp;
        cooldown = newAttackComp.cooldown;
    }

    public void AddSharkComponent(SharkComponent comp) {
        sharkComponents.Add(comp);
    }

    public void RemoveSharkComponent(SharkComponent comp) {
        sharkComponents.Remove(comp);
        Destroy(attackComp.gameObject);
    }

}
