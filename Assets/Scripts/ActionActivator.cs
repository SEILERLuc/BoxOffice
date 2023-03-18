using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionActivator : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private GameObject alteredObject;
    [SerializeField] private bool activated;
    private AlteredController _altered;
    private PlayerController _player;

    // Start is called before the first frame update
    void Start()
    {
        _player = playerObject.GetComponent<PlayerController>();
        _altered = alteredObject.GetComponent<AlteredController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_player.GetInteracting())
        {
            if (activated)
            {
                activated = false;
                anim.SetBool("Activated", false);
                Debug.Log("Close");
                _altered.Disable();
            }
            else
            {
                activated = true;
                anim.SetBool("Activated", true);
                Debug.Log("Open");
                _altered.Enable();
            }
        }
        // if (activated)
        // {
        //     altered.SetActive(false);
        //     anim.SetBool("Activated", false);
        //     Debug.Log("Close");
        // }
        // else
        // {
        //     altered.SetActive(true);
        //     anim.SetBool("Activated", true);
        //     Debug.Log("Open");
        // }
    }

    public void Trigger()
    {
        activated = !activated;
    }
}
