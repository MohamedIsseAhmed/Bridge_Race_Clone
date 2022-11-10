using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Threading.Tasks;

public class PickObject : MonoBehaviour
{
    public event EventHandler<CubeBase> OnPickedCube;
    [SerializeField] private Transform firstCube;
     public Transform FirstCube { get { return firstCube; } set { firstCube = value; } }
    public Color FirstCubeColor;
    [SerializeField] private Transform cubeParent;
    [SerializeField] private Vector3 offset; 
    [SerializeField] private float yOffset = 0.03f;
    [SerializeField] private float tweenTime = 0.2f;
    [SerializeField] private float jumpPower = 2f;
    [SerializeField] private int numberOfJumps = 1;
    [SerializeField] private int taskDelay = 1000;
    [SerializeField] private float distance = 0.2f;
  
    [SerializeField] private Ease easeTyep;
    [SerializeField] private CharacterBase characterBase;
    [SerializeField] private List<Transform> pickecObjects;
    [SerializeField] private List<Transform> listForWinComparism;
    public List<Transform> PickecObjects { get { return pickecObjects; } }
    public List<Transform> ListForWinComparism { get { return listForWinComparism; } }
    private List<Transform> dropedObjectsList;
    public List<Transform> DropedObjectsList => dropedObjectsList;
    [SerializeField] private bool isPicked = true;
    [SerializeField] private bool doTeween = false;

    private Transform pickedCube;
    [SerializeField] private string cubeTag;
    private Material material;
    private Tween cubePickUpTween;
    Vector3 targetPosition;
    public bool CanPlayerClipStairs { get; private set; }
    private void Awake()
    {
     
        listForWinComparism = new List<Transform>();
        pickecObjects = new List<Transform>();
        dropedObjectsList = new List<Transform>();
       
    }
    private void Start()
    {
      targetPosition = firstCube.localPosition;
    }
    private void Update()
    {
        if (!isPicked)
        {
            return;
        }
      

    }
    private async void OnTriggerEnter(Collider other)
    {

        if (other.transform.CompareTag(cubeTag))
        {
           await PickUpCube(other);
        }   
        else if (other.CompareTag("drop") && pickecObjects.Count > 0  )
        {
            OnStairsCollision(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag(cubeTag))
        {
            doTeween = false;
            cubePickUpTween.Kill();
        
        }
    }
    private bool CanPlayerGoOnStairs(Transform brick)
    {
        if (dropedObjectsList.Count > 1 )
        {
            return dropedObjectsList.Contains(brick) || pickecObjects.Count>1;
        }
        return false;
    }
    
    private void OnStairsCollision(Collider other)
    {
        if (pickecObjects.Count > 0)
        {
            if (!dropedObjectsList.Contains(other.transform) || other.gameObject.GetComponent<MeshRenderer>().material.color!= pickecObjects[pickecObjects.Count - 1].GetComponent<MeshRenderer>().sharedMaterial.color)
            {
                other.gameObject.GetComponent<MeshRenderer>().enabled = true;
               
                
                if (other.gameObject.GetComponent<MeshRenderer>().material.color != pickecObjects[pickecObjects.Count - 1].GetComponent<MeshRenderer>().sharedMaterial.color)                
                {
                    other.gameObject.GetComponent<Stair>().LerpColor(pickecObjects[pickecObjects.Count - 1].GetComponent<MeshRenderer>().sharedMaterial.color);
                    targetPosition.y -= yOffset;
                    pickecObjects[pickecObjects.Count - 1].gameObject.SetActive(false);
                    firstCube = pickecObjects[pickecObjects.Count - 1].transform;
                    FirstCubeColor = firstCube.GetComponent<MeshRenderer>().material.color;
                    Destroy(pickecObjects[pickecObjects.Count - 1].gameObject);
                    pickecObjects.Remove(pickecObjects[pickecObjects.Count - 1]);

                }
             
            }
         
        }
        CanPlayerClipStairs= CanPlayerGoOnStairs(other.transform);
        if (!dropedObjectsList.Contains(other.transform))
        {
            dropedObjectsList.Add(other.transform);

        }
        


        isPicked = true;
      
    }
    public void PlayAnimationsOnWin(bool isThisWiner)
    {
        characterBase.PlayAnimationsOnWin(isThisWiner);
    }
    private async Task PickUpCube(Collider other)
    {
        other.transform.tag = "picked";
       
        CubeBase cubeBase = other.gameObject.GetComponent<CubeBase>();
        if (cubeBase != null)
        {
            OnPickedCube?.Invoke(this, cubeBase);
        }
      
        firstCube = other.transform;
        FirstCubeColor = firstCube.GetComponent<MeshRenderer>().material.color;
        targetPosition.x = 0;
        targetPosition.y+= yOffset;
        targetPosition.z = 0;
        pickedCube = other.transform;
       
       
        other.transform.parent = cubeParent;
      
      
        await TweenBricks(other.transform, targetPosition);


        pickecObjects.Add(other.transform);
      
        listForWinComparism.Add(other.transform);
      
        doTeween = true;

        isPicked = false;

    }
  
    private async Task TweenBricks(Transform target,Vector3 targetPosition)
    {
        target.transform.localScale = new Vector3(0.55356878f, 1, 0.520956397f);
        target.transform.localEulerAngles = Vector3.zero;
     
        target.transform.DOLocalJump(targetPosition, jumpPower, numberOfJumps, tweenTime).OnComplete(() =>
       {
           target.GetComponent<CubeBase>().StopTrailRendereOnTop();
       });
        await Task.Yield();


    }
    public void ResetCollectingPosition()
    {
        targetPosition = firstCube.localPosition;
       
    }
}
