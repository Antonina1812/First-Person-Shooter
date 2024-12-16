using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
public class Door : MonoBehaviour
{
    [SerializeField] private NavMeshSurface navMeshSurface;
    public GameObject maze;
    public GameObject[] prefab;
    
    void Start()
    {
    }
    private int Number(int number)
    {
        int newNum = Random.Range(0, 4);
        while(number==newNum)
        {
            newNum = Random.Range(0, 4);
        }
        return newNum;
    }
    // Update is called once per frame
    void Update()
    {
        if(GameObject.FindGameObjectsWithTag("Enemy").Length==0)
        {
            Destroy(gameObject);
            GameObject newMaze = Instantiate(prefab[Current_Maze.number=Number(Current_Maze.number)],new Vector3(maze.transform.position.x+123,maze.transform.position.y,maze.transform.position.z), Quaternion.Euler(0,0,0));
            navMeshSurface = newMaze.GetComponent<NavMeshSurface>();
            if (navMeshSurface != null) 
            { 
                navMeshSurface.BuildNavMesh();
            }
        }
    }
}
