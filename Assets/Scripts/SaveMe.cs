using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveMe : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        DestroyManager();
    }

    void DestroyManager()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Music");

        if (objs.Length > 1)
        {
            Destroy(objs[0]);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
