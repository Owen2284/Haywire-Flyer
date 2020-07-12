using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBossBehaviour : BaseEnemyBehaviour
{
    public Vector2 strafeVector;

    private bool up;

    protected override void Setup() {
        secondsToNextCannon = cannonCooldownTime + (Random.Range(0, 0.25f) * cannonCooldownTime);

        up = true;
    }

    protected override void HandleMovement() {
        if (transform.position.x > 6) {
            body.velocity = moveVector;
        }
        else {
            if (transform.position.y > 3.5) {
                up = false;
            }
            if (transform.position.y < -3.5) {
                up = true;
            }

            if (up) {
                body.velocity = strafeVector;
            }
            else {
                body.velocity = strafeVector * -1;
            }
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

                secondsToNextCannon = cannonCooldownTime + (Random.Range(0, 0.25f) * cannonCooldownTime);
                nextCannon += 1;
                if (nextCannon >= cannons.Count) nextCannon = 0;
            }
        }
    }
}
