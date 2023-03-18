using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flying_ennemy : MonoBehaviour
{
    public int limit_top = 5;
    public float limit_bottom = -5;
    public float vitesse = 3;
    private Rigidbody2D rb;
    Vector2 direction;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        direction = new Vector2(0,vitesse);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y > limit_top) {
            transform.position = new Vector2(transform.position.x,limit_top-0.1f);
            direction = new Vector2(0,direction.y*-1);
        }
        if (transform.position.y < limit_bottom) {
            transform.position = new Vector2(transform.position.x,limit_bottom+0.1f);
            direction = new Vector2(0,direction.y*-1);
        }
        rb.velocity = direction;
    }
}
