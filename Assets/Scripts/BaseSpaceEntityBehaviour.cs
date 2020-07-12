using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSpaceEntityBehaviour : MonoBehaviour
{
    public float maxHealth;

    protected float health;

    protected Rigidbody2D body;

    // Start is called before the first frame update
    protected void Start()
    {
        body = GetComponent<Rigidbody2D>();
        if (body == null) {
            throw new System.Exception("No Rigidbody2D found for space entity");
        }

        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetCurrentHealth() {
        return Mathf.Max(health, 0);
    }

    public virtual void TakeDamage(float damage) {
        if (damage <= 0) {
            return;
        }

        health -= damage;

        if (health <= 0) Destroy(this.gameObject);
    }
}
