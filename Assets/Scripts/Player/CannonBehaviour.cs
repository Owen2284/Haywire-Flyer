using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBehaviour : MonoBehaviour
{
    public GameObject projectile;
    public float fireSpeed;

    void Start() {
        
    }

    public void FireCannon() {
        GameObject spawnedProjectile = (GameObject)Instantiate(projectile, transform.position, transform.rotation);

        Vector2 fireVelocity = new Vector2(fireSpeed, fireSpeed) * transform.up;
        spawnedProjectile.GetComponent<ProjectileBehaviour>().velocity = fireVelocity;
    }
}
