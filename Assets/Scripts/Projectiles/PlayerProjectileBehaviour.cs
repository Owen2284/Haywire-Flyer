using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileBehaviour : BaseProjectileBehaviour
{
    private void OnCollisionEnter2D(Collision2D target) {
        if (target.gameObject.tag == "Enemy" || target.gameObject.tag == "Boss") {
            BaseEnemyBehaviour enemy = target.gameObject.GetComponent<BaseEnemyBehaviour>();
            enemy.TakeDamage(damage);

            DecreaseEnergy();
        }
    }
}
