using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ButtonActivator : MonoBehaviour
{
    [SerializeField] private Transform objectDetect;
    [SerializeField] private float rayDist;
    [SerializeField] private GameObject button;
    [SerializeField] private GameObject alteredObject;
    private AlteredController _altered;
    private bool _pressed;
    
    void Start()
    {
        _pressed = false;
        _altered = alteredObject.GetComponent<AlteredController>();
    }
    
    void Update()
    {
        RaycastHit2D objectCheck = Physics2D.Raycast(objectDetect.position, Vector2.right * transform.localScale, rayDist);
        if (!(objectCheck.collider is null)) {
            if (!_pressed)
            {
                button.transform.position -= new Vector3(0,0.2f);
                _pressed = true;
                _altered.Disable();
            }
        }
        else
        {
            if (_pressed)
            {
                button.transform.position += new Vector3(0,0.2f);
                _pressed = false;
                _altered.Enable();
            }
        }
    }
}
