using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public GameObject midp;
    public GameObject pause;
    private bool isMenuActive= false;
    void Start()
    {
        
    }
    public void Resume()
    {
        isMenuActive = false;
        pause.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void Restart()
    {
        isMenuActive = false;
        Current_Maze.level = 0;
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void ExitMenu()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1f;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isMenuActive)
            {
                isMenuActive = false;
                pause.SetActive(false);
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                isMenuActive = true;
                pause.SetActive(true);
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }
}
