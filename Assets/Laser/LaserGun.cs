using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGun : MonoBehaviour
{

    public float reloadTime;
    public LaserBeam LaserBeamPrefab;

    private float nextFireTime = 0f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            if (Time.time > nextFireTime) Fire();
        }
    }

    public void Fire()
    {
        nextFireTime = Time.time + reloadTime;
        LaserBeam projectile = GameObject.Instantiate<LaserBeam>(LaserBeamPrefab, this.transform.position, this.transform.rotation);
        //Debug.Break();
    }
}
