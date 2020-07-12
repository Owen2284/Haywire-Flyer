using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBossBehaviour : BaseEnemyBehaviour
{
    public Vector2 strafeVector;

    private bool up;

    protected override void Setup() {
        secondsToNextCannon = GetCannonCooldownTime();

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

            float powerFactor = GetPowerFactor();
            if (up) {
                body.velocity = strafeVector * powerFactor;
            }
            else {
                body.velocity = strafeVector * -1 * powerFactor;
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

                secondsToNextCannon = GetCannonCooldownTime();
                nextCannon += 1;
                if (nextCannon >= cannons.Count) nextCannon = 0;
            }
        }
    }

    private float GetPowerFactor() {
        float healthFactor = health / maxHealth;

        if (healthFactor <= 0.25) {
            return 2f;
        }
        else if (healthFactor <= 0.5) {
            return 1.5f;
        }

        return 1f;
    }

    private float GetCannonCooldownTime() {
        return (cannonCooldownTime + (Random.Range(0, 0.25f) * cannonCooldownTime)) / GetPowerFactor();
    }
}
