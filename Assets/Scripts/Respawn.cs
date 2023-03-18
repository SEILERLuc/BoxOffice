using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Respawn : MonoBehaviour
{
    [SerializeField] private GameObject crateToSpawn;
    [SerializeField] private GameObject playerObject;
    private Transform _player;
    // Start is called before the first frame update
    void Start()
    {
        _player = playerObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckCrateState();
    }

    void CheckCrateState()
    {
        if (transform.position.y < -20)
        {
            Instantiate(crateToSpawn, new Vector2(_player.transform.position.x, _player.transform.position.y + 5), Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
