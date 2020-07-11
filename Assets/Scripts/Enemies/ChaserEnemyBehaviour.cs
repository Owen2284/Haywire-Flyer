using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserEnemyBehaviour : BaseEnemyBehaviour
{
    protected override void HandleMovement() {
        if (targetPlayer == null) {
            base.HandleMovement();
            return;
        }

        Vector2 relativePos = targetPlayer.transform.position - transform.position;
        body.velocity = relativePos;
    }

    protected override void HandleRotation() {
        if (targetPlayer != null) {
            transform.up = targetPlayer.transform.position - transform.position;
        }
        else {
            ResetRotation();
        }
    }
}
