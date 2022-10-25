using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
public class PickObject : MonoBehaviour
{
    public event EventHandler<CubeBase> OnPickedCube;
    [SerializeField] private Transform firstCube;
    [SerializeField] private Transform cubeParent;
    [SerializeField] private float yOffset = 0.03f;
    [SerializeField] private float tweenTime = 0.2f;
    [SerializeField] private float distance = 0.2f;
   // [SerializeField] private AnimationCurve curve;
    [SerializeField] private Ease easeTyep;
    [SerializeField] private CharacterBase characterBase;
    [SerializeField] private List<Transform> pickecObjects;
    [SerializeField] private List<Transform> listForWinComparism;
    public List<Transform> PickecObjects { get { return pickecObjects; } }
    public List<Transform> ListForWinComparism { get { return listForWinComparism; } }
    private List<Transform> dropedObjectsList;
    [SerializeField] private bool isPicked = false;
    [SerializeField] private bool doTeween = false;

    private Transform pickedCube;
    [SerializeField] private string cubeTag;
    private Material material;
    private Tween cubePickUpTween;

    private void Awake()
    {
        listForWinComparism = new List<Transform>();
        pickecObjects = new List<Transform>();
        dropedObjectsList = new List<Transform>();
       
    }
  
    private void Update()
    {
        if (!isPicked)
        {
            return;
        }
       ReArrangeListOfPicUps();
      
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.transform.CompareTag(cubeTag))
        {
            PickUpCube(other);
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
    private void OnStairsCollision(Collider other)
    {
        if (pickecObjects.Count > 1)
        {
            if (!dropedObjectsList.Contains(other.transform) || other.gameObject.GetComponent<MeshRenderer>().material.color!= pickecObjects[pickecObjects.Count - 1].GetComponent<MeshRenderer>().sharedMaterial.color)
            {
                other.gameObject.GetComponent<MeshRenderer>().enabled = true;
                other.gameObject.GetComponent<MeshRenderer>().material.color = pickecObjects[pickecObjects.Count - 1].GetComponent<MeshRenderer>().sharedMaterial.color;
                if (pickecObjects.Count >= 2)
                {

                    pickecObjects[pickecObjects.Count - 1].gameObject.SetActive(false);
                    firstCube = pickecObjects[pickecObjects.Count - 2].transform;
                    Destroy(pickecObjects[pickecObjects.Count - 1].gameObject);
                    pickecObjects.Remove(pickecObjects[pickecObjects.Count - 1]);

                }
            }
         
        }
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
    private void PickUpCube(Collider other)
    {
        CubeBase cubeBase=other.gameObject.GetComponent<CubeBase>();
        if (cubeBase != null)
        {
            OnPickedCube?.Invoke(this, cubeBase);
        }
   
        pickedCube = other.transform;
        other.transform.tag = "picked";
        other.transform.GetComponent<BoxCollider>().enabled = false;
        other.transform.parent = cubeParent;
        cubePickUpTween=other.transform.DOLocalMove(firstCube.localPosition + Vector3.up * firstCube.localScale.y * yOffset, tweenTime);
        other.transform.localScale = new Vector3(1.1093483f, 1.43232524f, 1.13810349f);
        other.transform.localEulerAngles = Vector3.zero;
        firstCube = other.transform;
        pickecObjects.Add(other.transform);
        listForWinComparism.Add(other.transform);
        isPicked = true;
        doTeween = true;
       
    }

    private void ReArrangeListOfPicUps()
    {
        
        if (pickecObjects.Count > 0)
        {
            float y = 1.7f;
            for (int i = pickecObjects.Count - 1; i < pickecObjects.Count; i++)
            {
                if (pickecObjects.Count > 2)
                {
                    if (pickecObjects[i].localPosition.y - pickecObjects[i - 1].localPosition.y > y)
                    {
                        Vector3 yVectorDjusted = pickecObjects[i - 1].localPosition + Vector3.up * pickecObjects[i - 1].localScale.y * yOffset;
                        pickecObjects[i].DOLocalMove(yVectorDjusted, tweenTime);    
                       
                    }
                }
              

            }
        }
       
    }
}
