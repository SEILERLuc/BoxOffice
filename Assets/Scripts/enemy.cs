using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    public float limit_left=7;
    public float limit_right=15;
    public float vitesse=3.5F;
    private Rigidbody2D rb;
    Vector2 direction;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        direction = new Vector2(vitesse,0);
    }

    // Update is called once per frame
    void Update()
    {
       if (transform.position.x > limit_right)
       {
           transform.position = new Vector2(limit_right - 0.1f, transform.position.y);
           direction = new Vector2(direction.x * -1,0);
           transform.localScale = new Vector2(1,1);
       }
       if (transform.position.x < limit_left){
           transform.position = new Vector2(limit_left + 0.1f, transform.position.y);
           direction = new Vector2(direction.x * -1,0);
           transform.localScale = new Vector2(-1,1);
       }
       rb.velocity=direction;
    }
}
