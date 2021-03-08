using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoilComponent : SharkComponent
{
    public const float coilCooldown = 1.5f;
    public GameObject shieldSprite;
    // public Color immuneColor;

    // Start is called before the first frame update

    protected override void Start()
    {
        base.Start();
        shark.NewAttack(this);
        shark.cooldown = coilCooldown;
        transform.localPosition = new Vector3(0.0f, 0.0f, -0.2f);
        pixelManager = FindObjectOfType<PixelManager>();
        shieldSprite.SetActive(false);
        if (shark is EnemyShark) GetComponent<AudioSource>().volume = GetComponent<AudioSource>().volume * 0.4f;
    }


    public override void Attack() {
        if (shark != null) {
            shark.cooldownTimer = coilCooldown;
            pixelManager.ActivateElectricity(transform.position);
            StartCoroutine(HandleImmunity());
            GetComponent<AudioSource>().Play();
        }
    }

    IEnumerator HandleImmunity() {
        if (shark != null) {
            shark.immuneToElectricity = true;
            // shark.GetComponent<SpriteRenderer>().color = immuneColor;
            shieldSprite.SetActive(true);

        }
        yield return new WaitForSeconds(pixelManager.electricityDuration + 0.1f);
        if (shark != null) {
            shark.immuneToElectricity = false;
            shieldSprite.SetActive(false);
        }
    }

    public override void SetMirror(bool toMirror) {
        base.SetMirror(toMirror);
        shieldSprite.GetComponent<SpriteRenderer>().flipY = toMirror;
    }
}
