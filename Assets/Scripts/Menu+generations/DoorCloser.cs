using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorCloser : MonoBehaviour
{
    public GameObject door;
    private Text level;
    void Start()
    {
        level = GameObject.FindGameObjectWithTag("Level").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        door.SetActive(true);
        level.text = "Level: " + (Current_Maze.level=Current_Maze.level+1).ToString();
        Destroy(gameObject);
    }
}
