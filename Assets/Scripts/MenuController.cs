using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void Load_Main_Menu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void Load_first_level()
    {
        SceneManager.LoadScene("Level1");
    }

    public void Exit_Game()
    {
        print("Goodbye");
       Application.Quit();
    }
}
