using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class GateManager : MonoBehaviour
{
    [SerializeField] private Transform gate1;
    [SerializeField] private Transform gate2;
    [SerializeField] private float xDirectionscaleTarget;
    [SerializeField] private float tweenDuratin=0.3f;
    private bool isCollided = false;
    private BoxCollider boxCollider;
    private void Awake()
    {
       boxCollider=GetComponent<BoxCollider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
           
            if (Vector3.Dot(transform.forward, other.transform.forward) < 0 && isCollided)
            {
               
            }
          
            if (!isCollided)
            {
                gate1.DOScaleX(xDirectionscaleTarget, tweenDuratin);
                gate2.DOScaleX(xDirectionscaleTarget, tweenDuratin).OnComplete(() =>
                {

                });
            }
            isCollided = true;
            //if (Vector3.Dot(transform.forward, other.transform.forward) < 0)
            //{

            //    boxCollider.isTrigger = false;
            //}
        }
    }
}
