using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonComponent : SharkComponent
{
    public const float shootingCooldown = 0.4f;
    public const float startingForce = 12.0f;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    // Start is called before the first frame update
    void Start()
    {
        shark.cooldown = shootingCooldown;
    }

    // Update is called once per frame
    void Update()
    {

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
