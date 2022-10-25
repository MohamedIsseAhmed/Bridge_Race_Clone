using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class WinManager : MonoBehaviour
{
 
    public static WinManager Instance;
    private Transform winner;
    public static event Action<CharacterBase,Vector3> OnWin;
    [SerializeField] private List<Transform> winnerList;
    [SerializeField] private List<Transform> winParticles;
    [SerializeField] private List<Transform> winersPlaces;
    public bool IsGameOver { get; private set; }    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
       
        Instance = this;
        
    }
  
    public void DeclareWinner()
    {

        GameObject[] objectPickers = GameObject.FindGameObjectsWithTag("picker");
      
        int maxCount=int.MinValue;
        for (int i = 0; i < objectPickers.Length; i++)
        {
            PickObject pick= objectPickers[i].GetComponent<PickObject>();
            if(pick != null)
            {
                winnerList.Add(pick.transform);
                if (pick.ListForWinComparism.Count > maxCount)
                {
                    maxCount = pick.ListForWinComparism.Count;
                    winner=pick.transform.parent;
                    
                }
            }
            
        }
        var list=
            from l in winnerList.OrderBy(x=>x.childCount)
            select l;
        foreach(Transform t in list)
        {
            print(t.parent.name);
        }
     
        print("maxlengh:" + maxCount+","+winner.parent.name);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player") && !IsGameOver)
        {
            StartCoroutine(CollidedWithCharacter(other));
        }
    }

    private IEnumerator  CollidedWithCharacter(Collider other)
    {
        IsGameOver = true;
     
      
        CharacterBase winerTransform = other.transform.GetComponent<CharacterBase>();
        winerTransform.GetComponent<Rigidbody>().isKinematic = true;

        GameObject[] objectPickers = GameObject.FindGameObjectsWithTag("Player");
        List<GameObject> characterList= objectPickers.OrderBy(g=>g.transform.childCount).ToList();
        characterList.Remove(other.gameObject);
       
        for (int i = 0; i < characterList.Count; i++)
        {
            print(characterList[i].name);
            characterList[i].transform.position=winersPlaces[i+1].transform.position;
            characterList[i].transform.rotation=winersPlaces[i+1].transform.rotation;
            characterList[i].GetComponent<CharacterBase>().PlayAnimationsOnWin(false);
        }
       
        if (winerTransform != null)
        {
         
            winerTransform.transform.position = winersPlaces[0].position;
            winerTransform.transform.rotation = winersPlaces[0].rotation;
            winerTransform.PlayAnimationsOnWin(true);
         

        }
        yield return null;
        StartCoroutine(PlayParticleSystem());
    }

    
    private IEnumerator PlayParticleSystem()
    {
        yield return null;
        for (int i = 0; i < winParticles.Count; i++)
        {
            winParticles[i].gameObject.SetActive(true);
          
        }
    }
}
