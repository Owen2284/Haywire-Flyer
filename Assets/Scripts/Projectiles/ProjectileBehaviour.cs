using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public Vector2 velocity;
    public float damage;

    private int energy;

    private Rigidbody2D body;

    // Start is called before the first frame update
    void Start()
    {
        energy = 1;

        body = GetComponent<Rigidbody2D>();
        body.velocity = velocity;

        // TODO: Audio?
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfOffScreen();
    }

    private void CheckIfOffScreen() {
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        if (screenPosition.x > Screen.width + 1 || screenPosition.x < -1 || 
            screenPosition.y > Screen.height + 1 || screenPosition.y < -1) {
            Destroy(this.gameObject);                
        }
    }

    private void OnCollisionEnter2D(Collision2D target) {
        if (target.gameObject.tag == "Enemy") {
            BaseEnemyBehaviour enemy = target.gameObject.GetComponent<BaseEnemyBehaviour>();
            enemy.TakeDamage(1);

            energy -= 1;

            if (energy <= 0) Destroy(this.gameObject);
        }
    }
}
