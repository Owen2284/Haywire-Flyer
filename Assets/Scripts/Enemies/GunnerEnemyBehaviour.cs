using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunnerEnemyBehaviour : BaseEnemyBehaviour
{
    protected override void Setup() {
        secondsToNextCannon = cannonCooldownTime + (Random.Range(0, 0.50f) * cannonCooldownTime);
    }

    protected override void HandleRotation() {
        if (targetPlayer != null) {
            transform.up = targetPlayer.transform.position - transform.position;
        }
        else {
            ResetRotation();
        }
    }

    protected override void HandleFiring() {
        if (targetPlayer != null) {
            if (secondsToNextCannon > 0) {
                secondsToNextCannon -= Time.deltaTime;
            }

            if (secondsToNextCannon <= 0) {
                CannonBehaviour currentCannon = cannons[nextCannon];
                currentCannon.FireCannon();

                secondsToNextCannon = cannonCooldownTime + (Random.Range(0, 0.50f) * cannonCooldownTime);
                nextCannon += 1;
                if (nextCannon >= cannons.Count) nextCannon = 0;
            }
        }
    }
}
