﻿using System.Collections;
using System.Collections.Generic;
using System.Timers;
using System.Linq;
using UnityEngine;

public class PlayerBehaviour : BaseSpaceEntityBehaviour
{
    public float moveSpeed;
    public float rotateSpeed;
    public float cannonCooldownTime;
    public float invulnerabilityTime;

    private List<CannonBehaviour> cannons;
    private int nextCannon;
    private float secondsToNextCannon;
    private float invulnerabilityTimeRemaining;

    private HaywireCollection haywires;
    private float haywireVerticalMovementUncontrollableFactor;

    private List<SpriteRenderer> spriteRenderers;

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

        spriteRenderers = new List<SpriteRenderer>() { GetComponent<SpriteRenderer>() };
        spriteRenderers.AddRange(cannons.Select(x => x.gameObject.GetComponent<SpriteRenderer>()));

        invulnerabilityTimeRemaining = 0;

        haywireVerticalMovementUncontrollableFactor = 1;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleFiring();
        HandleInvulnerability();
        HandleHaywires();
    }

    void HandleMovement() {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        if (IsHaywireActive(HaywireType.ShipMovementVerticalOnly)) {
            h = 0;
        }
        if (IsHaywireActive(HaywireType.ShipMovementVerticalUncontrollable)) {
            if (transform.position.y > 4) haywireVerticalMovementUncontrollableFactor = -1;
            if (transform.position.y < -4) haywireVerticalMovementUncontrollableFactor = 1;

            v = haywireVerticalMovementUncontrollableFactor;
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

        if (secondsToNextCannon <= 0 
        && (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0) || IsHaywireActive(HaywireType.ShipCannonsNonStop))) {
            CannonBehaviour currentCannon = cannons[nextCannon];

            bool backwards = IsHaywireActive(HaywireType.ShipCannonsBackwards);
            bool weighted = IsHaywireActive(HaywireType.ShipProjectilesWeighted);

            currentCannon.FireCannon(backwards, weighted);

            secondsToNextCannon = GetCannonCooldownTime();
            nextCannon += 1;
            if (nextCannon >= cannons.Count) nextCannon = 0;
        }
    }

    private void HandleInvulnerability() {
        if (invulnerabilityTimeRemaining > 0) {
            invulnerabilityTimeRemaining -= Time.deltaTime;

            float invulnerabilityFactor = Mathf.Ceil((invulnerabilityTimeRemaining / invulnerabilityTime) * (invulnerabilityTime * 10));
            bool visibleThisFrame = invulnerabilityFactor % 2 == 1;
            foreach (SpriteRenderer rend in spriteRenderers) {
                rend.enabled = visibleThisFrame;
            }
        }
        else {
            foreach (SpriteRenderer rend in spriteRenderers) {
                rend.enabled = true;
            }
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
            || IsHaywireActive(HaywireType.ShipCannonsNonStop)) {
            return cannonCooldownTime / 2;
        }
        
        if (IsHaywireActive(HaywireType.ShipMovementVerticalOnly)
            || IsHaywireActive(HaywireType.ShipSpinUncontrollable)
            || IsHaywireActive(HaywireType.ShipMovementVerticalUncontrollable)) {
            return (2 * cannonCooldownTime) / 3;
        }

        return cannonCooldownTime;
    }

    private float GetMoveSpeed() {
        float trueMoveSpeed = moveSpeed;

        if (IsHaywireActive(HaywireType.ShipSpeedDoubled)) {
            trueMoveSpeed = trueMoveSpeed * 2;
        }
        if (IsHaywireActive(HaywireType.ShipArmorWeightIncreased)) {
            trueMoveSpeed = trueMoveSpeed / 2;
        }

        return trueMoveSpeed;
    }

    public bool IsHaywireActive(HaywireType type) {
        if (haywires == null || haywires.TotalHaywires == 0) {
            return false;
        }

        return haywires.IsActive(type);
    }

    public void TakeDamage(float damage) {
        if (damage <= 0 || invulnerabilityTimeRemaining > 0) {
            return;
        }

        float trueDamage = damage;
        if (IsHaywireActive(HaywireType.ShipArmorWeightIncreased)) {
            trueDamage = damage / 2;
        }

        health -= trueDamage;

        if (health <= 0) {
            Destroy(this.gameObject);
        }

        invulnerabilityTimeRemaining = invulnerabilityTime;
    }
}
