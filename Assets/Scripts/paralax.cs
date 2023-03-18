using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class paralax : MonoBehaviour
{
    private Camera camera;
    private float paralaxmove;
    private Vector3 startpos;
    // Start is called before the first frame update
    void Start()
    {
        camera=GameObject.Find("Main Camera").GetComponent<Camera>();
        paralaxmove=0.07F;
        startpos=camera.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position =new Vector3(-(startpos.x+camera.transform.position.x*paralaxmove),startpos.y+camera.transform.position.y*paralaxmove,transform.position.z);
    }
}
