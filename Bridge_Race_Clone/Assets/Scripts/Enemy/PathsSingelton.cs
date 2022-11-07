using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathsSingelton : MonoBehaviour
{
    public static PathsSingelton instance;
    [SerializeField] private List<Transform> paths;
  
    private void Awake()
    {
        instance = this;
    }

    public List<Transform> GetPathsList()
    {
        return paths;
    }
}
