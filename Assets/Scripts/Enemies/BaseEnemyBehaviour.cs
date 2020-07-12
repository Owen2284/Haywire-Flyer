using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseEnemyBehaviour : BaseSpaceEntityBehaviour
{
    public Vector2 moveVector;
    public float rotatation;

    protected GameObject targetPlayer;

    private Vector3 DEFAULT_ROTATION = new Vector3(0, 0, 90);

    protected List<CannonBehaviour> cannons;
    protected int nextCannon;
    protected float secondsToNextCannon;

    public float cannonCooldownTime;

    // Start is called before the first frame update
    private new void Start()
    {
        base.Start();

        targetPlayer = GameObject.FindWithTag("Player");

        cannons = GetComponentsInChildren<Transform>()
            .Where(x => x.gameObject.name.StartsWith("Cannon"))
            .Select(x => x.GetComponent<CannonBehaviour>())
            .ToList();
        secondsToNextCannon = cannonCooldownTime;

        Setup();
    }

    // Update is called once per frame
    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleFiring();
        HandleDisposal();
    }

    private void OnCollisionEnter2D(Collision2D target) {
        HandleCollision(target.gameObject);
    }

    protected virtual void Setup() {

    }

    public string GetEnemyType() {
        return name.Replace("Enemy - ", "");
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

            // If spinning wildly, don't deal damage
            if (!player.IsHaywireActive(HaywireType.ShipSpinUncontrollable)) {
                player.TakeDamage(1);
            }
            else {
                // transform.up = targetPlayer.transform.position - transform.position;
                // body.AddForce(-transform.up * 5);
            }
        }
    }

    protected virtual void HandleDisposal() {
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        if (screenPosition.x < -3 || screenPosition.y > Screen.height + 1 || screenPosition.y < -1) {
            Destroy(this.gameObject);
        }
    }

    public void ResetRotation() {
        if (transform.eulerAngles.z != 90) {
            transform.eulerAngles = DEFAULT_ROTATION;
        }
    }
}
