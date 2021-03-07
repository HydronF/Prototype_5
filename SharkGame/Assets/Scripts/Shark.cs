using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType {
    Melee,
    Bullet,
    Electricity,
}

public class Shark : MonoBehaviour
{   
    [Header("Components")]
    public List<SharkComponent> sharkComponents;
    public Transform waterProbes;

    [Header("References")]
    public GameObject meleeHitBox;
    protected PixelManager pixelManager;
    protected UIManager uiManager;
    Transform player;

    [Header("Movement and Physics")]
    public Vector2 buoyantForce;
    public float waterPropulsion;
    public float airPropulsion;
    public float attackPropulsion;
    protected bool canFly = false;
    protected Vector2 dir = Vector2.zero;

    [Header("Health and Damage")]
    public int maxHealth;
    protected int currHealth;
    public int meleeDamage;
    public int bulletDamage;
    public int electricityDamage;
    public bool immuneToElectricity = false;
    public Color damageColor;

    [Header("Attack")]
    public float cooldown;
    public SharkComponent attackComp;
    public float cooldownTimer;

    // Start is called before the first frame update
    protected void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        uiManager = FindObjectOfType<UIManager>();
        pixelManager = FindObjectOfType<PixelManager>();
        sharkComponents = new List<SharkComponent>();
        attackComp = null;
        meleeHitBox.SetActive(false);
        cooldownTimer = 0.0f;
        currHealth = maxHealth;
    }

    // Update is called once per frame
    protected void Update()
    {
        cooldownTimer -= Time.deltaTime;
    }

    void FixedUpdate() {
        dir = GetTargetDir(player.position);
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
        if (currPixel == PixelContent.Empty) {
            GetComponent<Rigidbody2D>().drag = 0.3f;
            GetComponent<Rigidbody2D>().AddForce(airPropulsion * toTarget);
            if (canFly) GetComponent<Rigidbody2D>().AddForce(new Vector2(0.0f, 10.5f));
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
        attackComp = newAttackComp;
        if (newAttackComp != null) {
            cooldown = newAttackComp.cooldown;
        }
    }

    public void AddSharkComponent(SharkComponent comp) {
        sharkComponents.Add(comp);
    }

    public void RemoveSharkComponent(SharkComponent comp) {
        sharkComponents.Remove(comp);
    }

    public virtual void TakeDamage(AttackType attackType) {
        switch (attackType)
        {
            case AttackType.Melee:
                currHealth -= meleeDamage;
                break;
            case AttackType.Bullet:
                currHealth -= bulletDamage;
                break;   
            case AttackType.Electricity:
                currHealth -= electricityDamage;
                break;
        }
        StartCoroutine(DamageAnim());
        if (currHealth <= 0) {
            Die();
        }
        // Enemy and player shark handles ui health bar differently.
    }

    protected IEnumerator DamageAnim() {
        GetComponent<SpriteRenderer>().color = damageColor;
        yield return new WaitForSeconds(0.5f);
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.tag == "Electricity" && !immuneToElectricity) {
            TakeDamage(AttackType.Electricity);
        }
    }

    public virtual void Die() {

    }

}
