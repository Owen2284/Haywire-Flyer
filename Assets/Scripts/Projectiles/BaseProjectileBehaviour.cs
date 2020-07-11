using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProjectileBehaviour : MonoBehaviour
{
    public Vector2 velocity;
    public float damage;

    protected int energy;

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

    protected void DecreaseEnergy() {
        energy -= 1;

        if (energy <= 0) Destroy(this.gameObject);
    }
}
