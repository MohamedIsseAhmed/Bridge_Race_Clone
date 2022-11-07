using UnityEngine;
using DG.Tweening;
using System;
public class GateManager : MonoBehaviour
{
    public static GateManager Instance;
    [SerializeField] private Transform gate1;
    [SerializeField] private Transform gate2;
    [SerializeField] private Transform upperPlane;
    [SerializeField] private float xDirectionscaleTarget;
    [SerializeField] private float tweenDuratin=0.3f;
    [SerializeField] private bool isThisGateFinalGate=false;
    private bool isOpned = false;
    private BoxCollider boxCollider;
    public event EventHandler<Transform> OnDoorOpened;
    
    private void Awake()
    {
        Instance = this;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isThisGateFinalGate)
        {
            if (other.transform.CompareTag("Player"))
            {
                CharacterBase character = other.gameObject.GetComponent<CharacterBase>();

                if (!isOpned)
                {
                    gate1.DOScaleX(xDirectionscaleTarget, tweenDuratin);
                    gate2.DOScaleX(xDirectionscaleTarget, tweenDuratin);

                    if (character != null)
                    {
                        character.SpawnCubes(upperPlane);
                        print("call spawnFun");

                    }
                    isOpned = true;
                }
                else
                {
                    character.IncreaseNumber();
                }


            }
        }
        else
        {
            CharacterBase character = other.gameObject.GetComponent<CharacterBase>();
          
            gate1.DOScaleX(xDirectionscaleTarget, tweenDuratin);
            gate2.DOScaleX(xDirectionscaleTarget, tweenDuratin);
          
            if (!isOpned)
            {
               
                
            }
        }
      
    }
}
