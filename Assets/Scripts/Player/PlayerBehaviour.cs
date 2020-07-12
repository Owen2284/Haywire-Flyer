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

    private HaywireCollection haywires;

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
        HandleHaywires();
    }

    void HandleMovement() {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        if (IsHaywireActive(HaywireType.ShipMovementVerticalOnly)) {
            h = 0;
        }

        float trueMoveSpeed = GetMoveSpeed();

        body.velocity = new Vector2(h * trueMoveSpeed, v * trueMoveSpeed);
    }

    void HandleRotation() {
        if (!IsHaywireActive(HaywireType.ShipSpinUncontrollable)) {
            Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

            transform.up = worldPosition - new Vector2(transform.position.x, transform.position.y);
        }
        else {
            transform.Rotate(0, 0, 1.2f);
        }
    }

    void HandleFiring() {
        if (secondsToNextCannon > 0) {
            secondsToNextCannon -= Time.deltaTime;
        }

        if (secondsToNextCannon <= 0 && (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))) {
            CannonBehaviour currentCannon = cannons[nextCannon];
            currentCannon.FireCannon();

            secondsToNextCannon = GetCannonCooldownTime();
            nextCannon += 1;
            if (nextCannon >= cannons.Count) nextCannon = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D target) {
        HandleCollision(target.gameObject);
    }

    void HandleCollision(GameObject target) {
        if (target.tag == "Enemy" || target.tag == "Boss") {
            BaseEnemyBehaviour enemy = target.GetComponent<BaseEnemyBehaviour>();
            enemy.TakeDamage(1);

            if (health <= 0) Destroy(this.gameObject);
        }
    }

    void HandleHaywires() {
        // Spin cannons
        if (IsHaywireActive(HaywireType.ShipCannonsSpin)) {
            foreach (CannonBehaviour cannon in cannons) {
                cannon.gameObject.transform.Rotate(0, 0, 0.2f);
            }
        }
        else {
            foreach (CannonBehaviour cannon in cannons) {
                cannon.gameObject.transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z);
            }
        }
    }

    public void SetHaywires(HaywireCollection h) {
        haywires = h;
    }

    private float GetCannonCooldownTime() {
        // Speed up guns for some haywires
        if (IsHaywireActive(HaywireType.ShipCannonsSpin)
            || IsHaywireActive(HaywireType.ShipProjectilesWeighted)
            || IsHaywireActive(HaywireType.ShipCannonsNonStop)) {
            return cannonCooldownTime / 2;
        }
        
        if (IsHaywireActive(HaywireType.ShipMovementVerticalOnly)
            || IsHaywireActive(HaywireType.ShipSpinUncontrollable)
            || IsHaywireActive(HaywireType.ShipMovementPong)) {
            return (2 * cannonCooldownTime) / 3;
        }

        return cannonCooldownTime;
    }

    private float GetMoveSpeed() {
        if (IsHaywireActive(HaywireType.ShipSpeedDoubled)) {
            return moveSpeed * 2;
        }

        return moveSpeed;
    }

    public bool IsHaywireActive(HaywireType type) {
        if (haywires == null || haywires.TotalHaywires == 0) {
            return false;
        }

        return haywires.IsActive(type);
    }
}
