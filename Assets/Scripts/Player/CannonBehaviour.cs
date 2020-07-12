using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBehaviour : MonoBehaviour
{
    public GameObject projectile;
    public float fireSpeed;
    public bool facePlayer;

    private float originalRotation;
    private GameObject targetPlayer;

    void Start() {
        originalRotation = transform.rotation.z;
        targetPlayer = GameObject.FindWithTag("Player");
    }

    void Update() {
        if (facePlayer && targetPlayer != null) {
            transform.up = targetPlayer.transform.position - transform.position;
        }
        else {
            //transform.eulerAngles = new Vector3(0, 0, originalRotation);
        }
    }

    public void FireCannon(bool backwards = false) {
        GameObject spawnedProjectile = (GameObject)Instantiate(projectile, transform.position, transform.rotation);

        float trueFireSpeed = fireSpeed;
        if (backwards) {
            trueFireSpeed = trueFireSpeed * -1;
        }

        Vector2 fireVelocity = new Vector2(trueFireSpeed, trueFireSpeed) * transform.up;
        spawnedProjectile.GetComponent<BaseProjectileBehaviour>().velocity = fireVelocity;
    }
}
