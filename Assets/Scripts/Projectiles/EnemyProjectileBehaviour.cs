using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileBehaviour : BaseProjectileBehaviour
{
    private void OnCollisionEnter2D(Collision2D target) {
        Debug.Log("Ooh");

        if (target.gameObject.tag == "Player") {
            PlayerBehaviour player = target.gameObject.GetComponent<PlayerBehaviour>();
            player.TakeDamage(damage);
        }

        DecreaseEnergy();
    }
}
