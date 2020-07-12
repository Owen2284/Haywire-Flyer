using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarGeneratorScript : MonoBehaviour
{
    public GameObject star;
    public int starCount;

    // Start is called before the first frame update
    void Start()
    {
        Quaternion rotation = Quaternion.Euler(0, 0, 0);

        for (int i = 0; i < starCount; i++) {
            float x = Random.Range(-10f, 10f);
            float y = Random.Range(-5f, 5f);
            Vector3 position = new Vector3(x, y, 100);

            Instantiate(star, position, rotation);
        }
    }
}
