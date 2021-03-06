using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonComponent : SharkComponent
{
    public float shootingCooldown;
    public GameObject bulletPrefab;
    Transform playerTransform; 
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = shark.player.transform;
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator ShootCannon() {
        yield return new WaitForSeconds(shootingCooldown);
        Instantiate(bulletPrefab, transform.position, transform.rotation);
    }
}
