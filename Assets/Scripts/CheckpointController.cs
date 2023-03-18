using UnityEngine.SceneManagement;
using UnityEngine;


public class CheckpointController : MonoBehaviour
{
    private int sceneNumber;

    private void Start()
    {
        sceneNumber = SceneManager.GetActiveScene().buildIndex;
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            print("�collision");
            if (sceneNumber < 4)
            {
                
                sceneNumber++;
                SceneManager.LoadScene(sceneNumber);
            }
        }
    }
}
