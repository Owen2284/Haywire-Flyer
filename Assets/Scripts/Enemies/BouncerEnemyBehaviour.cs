using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncerEnemyBehaviour : BaseEnemyBehaviour
{
    protected override void Setup() {
        secondsToNextCannon = cannonCooldownTime + (Random.Range(0, 0.50f) * cannonCooldownTime);
    }

    protected override void HandleFiring() {
        if (targetPlayer != null) {
            if (secondsToNextCannon > 0) {
                secondsToNextCannon -= Time.deltaTime;
            }

            if (secondsToNextCannon <= 0) {
                CannonBehaviour currentCannon = cannons[nextCannon];
                currentCannon.FireCannon(true);

                secondsToNextCannon = cannonCooldownTime + (Random.Range(0, 0.50f) * cannonCooldownTime);
                nextCannon += 1;
                if (nextCannon >= cannons.Count) nextCannon = 0;
            }
        }
    }

    protected override void HandleCollision(GameObject target) {
        base.HandleCollision(target);

        if (target.tag == "Boundary" || target.tag == "Enemy") {
            moveVector = new Vector2(moveVector.x, moveVector.y * -1);
        }
    }
}
