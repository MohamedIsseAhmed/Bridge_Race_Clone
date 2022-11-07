using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class SpawnSytem : MonoBehaviour
{
    public static SpawnSytem Instance;
    [SerializeField] private Transform cubePrefab;
    [SerializeField] private Transform plane;
     private Transform currentPlane;
    [SerializeField] private float width;
    [SerializeField] private float minWidth;
    [SerializeField] private float height;
    [SerializeField] private float minHeight;
    [SerializeField] private float positnModifer;

    private List<Vector3> spawnPositions;
    [SerializeField] private Transform[] resoureces;
    [SerializeField] private Transform blueCubeListParent;
    [SerializeField] private Transform greenCubeListParent;
    [SerializeField] private Transform redCubeListParent;
    
   [SerializeField] private List<BlueCube> blueCubeInstaintaited;   
   public List<BlueCube> BlueCubeInstaintaited { get { return blueCubeInstaintaited; } }   
   [SerializeField] private List<GreenCube> greenCubeInstaintaited;
    public List<GreenCube> GreenCubeInstaintaited { get { return greenCubeInstaintaited; } }
    [SerializeField] private List<RedCube> redCubeInstaintaited;
    public List<RedCube> RedCubeInstaintaited { get { return redCubeInstaintaited; } }

    private CubeBase[][] cubeInstaintaited;
    public CubeBase[][] CubeInstaintaited { get { return cubeInstaintaited; } }
    public event EventHandler OnRespawndedCubes;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
        cubeInstaintaited = new CubeBase[3][];    
        blueCubeInstaintaited =new List<BlueCube>();
        greenCubeInstaintaited=new List<GreenCube>();
        redCubeInstaintaited=new List<RedCube>();

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
        currentPlane = plane; ;
    }
    void Start()
    {
       
        SpawnCubes(plane.transform);

        cubeInstaintaited[0]=  blueCubeInstaintaited.ToArray();
        cubeInstaintaited[1] = greenCubeInstaintaited.ToArray();
        cubeInstaintaited[2] = redCubeInstaintaited.ToArray();   

      
    }
  
    public void SpawnCubes(Transform plane)
    {
        minWidth = plane.GetComponent<BoxCollider>().bounds.min.x * 0.35f;
        width = plane.GetComponent<BoxCollider>().bounds.max.x * 0.35f;
        minHeight = plane.GetComponent<BoxCollider>().bounds.min.z;
        height = plane.GetComponent<BoxCollider>().bounds.max.z;

        for (float x = minWidth; x <= width; x+=0.35f)
        {
            for (float z = minHeight; z <=height; z+=0.35f)
            {
                //positnModifer = Random.Range(0.4f, 0.8f);
                int random = Random.Range(0, resoureces.Length);
                Transform newCube=Instantiate(resoureces[random], GetPosition(x,z),Quaternion.identity,transform);
                CubeBase cube=newCube.GetComponent<CubeBase>();    
                if (cube is BlueCube)
                {
                    blueCubeInstaintaited.Add(cube as BlueCube);
                    cube.transform.SetParent(blueCubeListParent);
                }
                else if(cube is GreenCube)
                {
                    greenCubeInstaintaited.Add(cube as GreenCube);
                    cube.transform.SetParent(greenCubeListParent);
                }
                else if (cube is RedCube)
                {
                    redCubeInstaintaited.Add(cube as RedCube);
                    cube.transform.SetParent(redCubeListParent);
                }
            }
        }
      
    }
    private Vector3 GetPosition(float x,float z)
    {
        float y = 0.014057f;
        return new Vector3(x, currentPlane.position.y+y, z+0.11f);
    }
    public void OnDoorOpened(Transform targetPlane)
    {
       
        if (targetPlane != null)
        {
            currentPlane = targetPlane;
           
            blueCubeInstaintaited.Clear();
            greenCubeInstaintaited.Clear();
            redCubeInstaintaited.Clear();
            SpawnCubes(currentPlane);
            
            cubeInstaintaited[0] = blueCubeInstaintaited.ToArray();
            cubeInstaintaited[1] = greenCubeInstaintaited.ToArray();
            cubeInstaintaited[2] = redCubeInstaintaited.ToArray();
            
        }
      
  
    }
  
}
