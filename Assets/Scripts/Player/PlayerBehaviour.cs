using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class PlayerBehaviour : BaseSpaceEntityBehaviour
{
    public float moveSpeed;
    public float rotateSpeed;
    public float cannonCooldownTime;

    private List<CannonBehaviour> cannons;
    private int nextCannon;
    private float secondsToNextCannon;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        nextCannon = 0;
        secondsToNextCannon = 0;
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
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        transform.up = worldPosition - new Vector2(transform.position.x, transform.position.y);
    }

    void HandleFiring() {
        if (secondsToNextCannon > 0) {
            secondsToNextCannon -= Time.deltaTime;
        }

        if (secondsToNextCannon <= 0 && (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))) {
            CannonBehaviour currentCannon = cannons[nextCannon];
            currentCannon.FireCannon();

            secondsToNextCannon = cannonCooldownTime;
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
