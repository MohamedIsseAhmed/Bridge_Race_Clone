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
    public static event EventHandler OnWinCameraPositionEvent;
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
    
     
     
    }
    private void OnTriggerEnter(Collider other)
    {
        if ((other.transform.CompareTag("Player") || other.transform.CompareTag("Enemy"))&& !IsGameOver)
        {
            IsGameOver = true;
            StartCoroutine(CollidedWithCharacter(other));
            OnWinCameraPositionEvent?.Invoke(this, EventArgs.Empty);
        }
    }

    private IEnumerator  CollidedWithCharacter(Collider other)
    {
        IsGameOver = true;


        CharacterBase winerTransform = other.transform.GetComponent<CharacterBase>();
        winerTransform.GetComponent<Rigidbody>().isKinematic = true;

        CharacterBase[] objectPickers = GameObject.FindObjectsOfType<CharacterBase>();
        List<CharacterBase> characterList = objectPickers.OrderBy(g => g.transform.childCount).ToList();
        characterList.Remove(other.GetComponent<CharacterBase>());
        if (winerTransform != null)
        {

            winerTransform.transform.position = winersPlaces[0].position;
            winerTransform.transform.rotation = winersPlaces[0].rotation;
            winerTransform.PlayAnimationsOnWin(true);


        }
        StartCoroutine(TransformCharactersToWinPositins(characterList));

        yield return null;
        StartCoroutine(PlayParticleSystem());
    }

    private IEnumerator TransformCharactersToWinPositins(List<CharacterBase> characterList)
    {
        for (int i = 0; i < characterList.Count; i++)
        {
            print(characterList[i].name);
            characterList[i].transform.position = winersPlaces[i + 1].transform.localPosition;
            characterList[i].transform.rotation = winersPlaces[i + 1].transform.rotation;
            characterList[i].GetComponent<CharacterBase>().PlayAnimationsOnWin(false);
            yield return null;
        } 
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
