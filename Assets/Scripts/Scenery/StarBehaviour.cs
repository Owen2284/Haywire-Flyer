using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarBehaviour : MonoBehaviour
{
    public Vector2 moveVector;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position -= new Vector3(moveVector.x, moveVector.y, 0);

        if (transform.position.x < -10) {
            Vector3 position = new Vector3(10, transform.position.y, 100);
            Quaternion rotation = Quaternion.Euler(0, 0, 0);
            Instantiate(this, position, rotation);

            Destroy(this.gameObject);
        }
    }
}
