using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonComponent : SharkComponent
{
    public const float shootingCooldown = 0.6f;
    public const float startingForce = 10.0f;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    // Start is called before the first frame update
    protected override void Start()
    {
        shark.NewAttack(this);
        shark.cooldown = shootingCooldown;
        transform.localPosition = new Vector3(0.0f, 0.0f, -0.1f);
    }

    public override void Attack() {
        shark.cooldownTimer = shootingCooldown;
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation, transform);
        Vector2 dir = - new Vector2(transform.right.x, transform.right.y);
        bullet.GetComponent<Rigidbody2D>().AddForce(startingForce * dir, ForceMode2D.Impulse);
    }

    public override void SetMirror(bool toMirror) {
        base.SetMirror(toMirror);
        bulletSpawn.localPosition = new Vector3(bulletSpawn.localPosition.x, -bulletSpawn.localPosition.y, bulletSpawn.localPosition.z);
    }

}
