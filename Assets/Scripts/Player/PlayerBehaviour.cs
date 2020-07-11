using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : BaseSpaceEntityBehaviour
{
    public float moveSpeed;
    public float rotateSpeed;

    private List<CannonBehaviour> cannons;
    private int nextCannon;
    private int cannonCooldown;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        nextCannon = 0;
        cannonCooldown = 0;
        cannons = new List<CannonBehaviour> {
            transform.Find("Cannon L").gameObject.GetComponent<CannonBehaviour>(),
            transform.Find("Cannon R").gameObject.GetComponent<CannonBehaviour>()
        };
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleFiring();
    }

    void HandleMovement() {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        body.velocity = new Vector2(h * moveSpeed, v * moveSpeed);
    }

    void HandleRotation() {
        bool clockwise = Input.GetKey(KeyCode.E);
        bool antiClockwise = Input.GetKey(KeyCode.Q);

        int rotationFactor = 0;

        if (clockwise && !antiClockwise) {
            rotationFactor = 1;
        }
        else if (antiClockwise && !clockwise) {
            rotationFactor = -1;
        }

        body.rotation = body.rotation + (rotationFactor * rotateSpeed);
    }

    void HandleFiring() {
        if (cannonCooldown > 0) {
            cannonCooldown -= 1;
        }

        if (cannonCooldown == 0 && Input.GetKey(KeyCode.Space)) {
            CannonBehaviour currentCannon = cannons[nextCannon];
            currentCannon.FireCannon();

            cannonCooldown = 20;
            nextCannon += 1;
            if (nextCannon >= cannons.Count) nextCannon = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D target) {
        HandleCollision(target.gameObject);
    }

    void HandleCollision(GameObject target) {
        if (target.tag == "Enemy") {
            BaseEnemyBehaviour enemy = target.GetComponent<BaseEnemyBehaviour>();
            enemy.TakeDamage(1);

            if (health <= 0) Destroy(this.gameObject);
        }
    }
}
