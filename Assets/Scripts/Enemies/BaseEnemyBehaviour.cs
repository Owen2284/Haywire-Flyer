using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyBehaviour : BaseSpaceEntityBehaviour
{
    public Vector2 moveVector;
    public float rotatation;

    private Vector3 DEFAULT_ROTATION = new Vector3(0, 0, 90);

    // Start is called before the first frame update
    private void Start()
    {
        base.Start();

        Setup();
    }

    // Update is called once per frame
    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleFiring();
    }

    private void OnCollisionEnter2D(Collision2D target) {
        HandleCollision(target.gameObject);
    }

    protected virtual void Setup() {

    }

    protected virtual void HandleMovement() {
        body.velocity = moveVector;
    }

    protected virtual void HandleRotation() {
        body.rotation += rotatation;
    }

    protected virtual void HandleFiring() {
        
    }

    protected virtual void HandleCollision(GameObject target) {
        if (target.tag == "Player") {
            PlayerBehaviour player = target.GetComponent<PlayerBehaviour>();
            player.TakeDamage(1);
        }
    }

    protected void ResetRotation() {
        if (transform.eulerAngles.z != 90) {
            transform.eulerAngles = DEFAULT_ROTATION;
        }
    }
}
