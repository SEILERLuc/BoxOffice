using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateBehaviour : MonoBehaviour
{
    private Rigidbody2D rb;
    public bool hurting=true;
    private bool holding =false;
    private AudioSource sfx;
    public AudioClip sfx_kill;
    // Start is called before the first frame update
    void Start()
    {
      rb=GetComponent<Rigidbody2D>();   
    }

    // Update is called once per frame
    void Update()
    {
         if(rb.velocity.x == 0 && rb.velocity.y == 0){
            hurting=false;
         }else{
            hurting=true;
         }
    }
    private void OnCollisionEnter2D(Collision2D collision){
        if (collision.gameObject.CompareTag("Enemy")&&hurting){
            Destroy(collision.gameObject);
            //sfx.PlayOneShot(sfx_kill);
        }
    }

}
