using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawnSytem : MonoBehaviour
{
    [SerializeField] private Transform cubePrefab;
    [SerializeField] private Transform plane;
    [SerializeField] private float width;
    [SerializeField] private float minWidth;
    [SerializeField] private float height;
    [SerializeField] private float minHeight;
    [SerializeField] private float positnModifer;

    private List<Vector3> spawnPositions;
    [SerializeField] private Transform[] resoureces;
    private void Awake()
    {
        spawnPositions = new List<Vector3>();
        for (float x = minWidth; x <= width; x += 0.35f)
        {
            for (float z = minHeight; z <= height; z += 0.35f)
            {
                //positnModifer = Random.Range(0.4f, 0.8f);
                spawnPositions.Add(new Vector3(x,0,z));
            }
        }
     
        resoureces = Resources.LoadAll<Transform>("Cubes");
    }
    void Start()
    {
        minWidth = plane.GetComponent<BoxCollider>().bounds.min.x*0.35f;
        width = plane.GetComponent<BoxCollider>().bounds.max.x * 0.35f;
        minHeight = plane.GetComponent<BoxCollider>().bounds.min.z;
        height = plane.GetComponent<BoxCollider>().bounds.max.z ;
        SpawnCubes();
        print(plane.GetComponent<BoxCollider>().bounds.min);
        print(plane.GetComponent<BoxCollider>().bounds.extents);
        print(plane.GetComponent<BoxCollider>().bounds.max);
        print(plane.GetComponent<BoxCollider>().bounds.center);
        print(plane.GetComponent<BoxCollider>().bounds.size);
    }
    private void Update()
    {
        
    }
    public void SpawnCubes()
    {
      
        for (float x = minWidth; x <= width; x+=0.35f)
        {
            for (float z = minHeight; z <=height; z+=0.35f)
            {
                //positnModifer = Random.Range(0.4f, 0.8f);
                int random = Random.Range(0, resoureces.Length);
                Transform newCube=Instantiate(resoureces[random], GetPosition(x,z),Quaternion.identity,transform);
            }
        }
        print(transform.childCount);
    }
    private Vector3 GetPosition(float x,float z)
    {
        float y = 0.014057f;
        return new Vector3(x, plane.position.y+y, z+0.11f);
    }
}
