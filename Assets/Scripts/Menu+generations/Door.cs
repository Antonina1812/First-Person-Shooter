using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private GameObject maze;
    public GameObject prefab;
    void Start()
    {
        maze = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameObject.FindGameObjectsWithTag("Enemy").Length==0)
        {
            Instantiate(prefab,new Vector3(maze.transform.position.x+40,maze.transform.position.y,maze.transform.position.z), Quaternion.Euler(0,0,0));
            Destroy(gameObject);
        }
    }
}
