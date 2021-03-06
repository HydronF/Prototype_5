using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoilComponent : SharkComponent
{
    public const float coilCooldown = 1.5f;
    public PixelManager pixelManager;
    // Start is called before the first frame update

    protected override void Start()
    {
        shark.NewAttack(this);
        shark.cooldown = coilCooldown;
        transform.localPosition = new Vector3(0.0f, 0.0f, -0.2f);
        pixelManager = FindObjectOfType<PixelManager>();
    }


    public override void Attack() {
        shark.cooldownTimer = coilCooldown;
        pixelManager.ActivateElectricity(transform.position);
        StartCoroutine(HandleImmunity());
    }

    IEnumerator HandleImmunity() {
        shark.immuneToElectricity = true;
        yield return new WaitForSeconds(pixelManager.electricityDuration + 0.1f);
        shark.immuneToElectricity = false;

    }
}
