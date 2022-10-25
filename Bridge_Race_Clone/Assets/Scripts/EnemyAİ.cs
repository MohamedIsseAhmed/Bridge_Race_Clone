using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using System.Threading.Tasks;
using System.Linq;

public class EnemyAİ : CharacterBase
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float sphereRadius = 2f;
    [SerializeField] private float distanceToCube = 2f;
    [SerializeField] private float turnSpeed = 5f;
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float waitTime = 0.1f;
    [SerializeField] private float waitTimeToGoBack = 0.1f;
    [SerializeField] private float angel = 90f;
    [SerializeField] Collider[] resultHits;
    [SerializeField] private Animator animator;
 
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform[]  firstStagepaths;
    [SerializeField] private Transform[]  secondtStagepaths;
    [SerializeField] private PickObject cubePicker;
    [SerializeField] private Transform doorTrigger;
    [SerializeField] private CharacterColorType characterColorType;
    [SerializeField] private  CubeBase[] targetCubes;
    [SerializeField] private List<CubeBase> targetCubeList;
  
    [SerializeField] private int randomNuber;
    [SerializeField] private int maxNumber=25;
    [SerializeField] private int minNumber=5;
 
   
    [SerializeField] private Transform testCube;
    [SerializeField] private EnemyState enemyState;
   
    private bool shouldGoBack=false;
    private NavMeshAgent agent;
    private Vector3 hitRight;
    private bool isDoorOpened = false;
    [SerializeField] private int minWinLevelNumber = 1;
    [SerializeField] private int maxWinLevelNumber = 2;
    private void Awake()
    {
        enemyState = EnemyState.CollectingCubeState;
        agent = GetComponent<NavMeshAgent>();
       
    }
    private IEnumerator  Start()
    {
        levelNumberForAWin = 1;
      
        cubePicker.OnPickedCube += RemovePickedCubeFromLits;
       
       
        yield return new WaitForSeconds(1);
      

        if (characterColorType == CharacterColorType.Red)
        {
            targetCubes = new BlueCube[SpawnSytem.Instance.CubeInstaintaited[2].Length];
            targetCubes = SpawnSytem.Instance.CubeInstaintaited[2];
            targetCubeList = new List<CubeBase>();
            targetCubeList = targetCubes.ToList();
        }
        else if (characterColorType == CharacterColorType.Blue)
        {
            targetCubes = new BlueCube[SpawnSytem.Instance.CubeInstaintaited[0].Length];
            targetCubes = SpawnSytem.Instance.CubeInstaintaited[0];
            targetCubeList = new List<CubeBase>();
            targetCubeList= targetCubes.ToList();
        }
        else if(characterColorType == CharacterColorType.Green)
        {
            targetCubes = new BlueCube[SpawnSytem.Instance.CubeInstaintaited[1].Length];
            targetCubes = SpawnSytem.Instance.CubeInstaintaited[1];
            targetCubeList = new List<CubeBase>();
            targetCubeList = targetCubes.ToList();
        }
   
     
        animator.SetFloat("BlendSpeed", 0.85f);
        randomNuber = UnityEngine.Random.Range(minNumber, maxNumber);
      
    }

  

    private void Update()
    {
        
        ProseccAIStates();
        levelNumberForAWin = Mathf.Clamp(levelNumberForAWin, minWinLevelNumber, maxWinLevelNumber);
        ChechGround();
    }

    private void ProseccAIStates()
    {
        if (WinManager.Instance.IsGameOver)
        {
            if (agent.enabled)
            {
                agent.isStopped = true;
            }
         
            return;
        }
        switch (enemyState)
        {
            case EnemyState.CollectingCubeState:
                StartCoroutine(CollectCubeCoroutine());
                break;
            case EnemyState.GoToTarget:
                StartCoroutine(GoToTarget1Coroutine());
                break;
            case EnemyState.GoToUpState:
                StartCoroutine(GoToPath2Coroutine());
                break;
            case EnemyState.GoToDownState:
                StartCoroutine(GoToTarget1Coroutine());
                break;
            case EnemyState.WaitingState:
                StartCoroutine(WaitingCoroutine());
                break;
            case EnemyState.WinState:

                break;
            case EnemyState.GameOverState:
                break;
            default:
                break;
        }
    }

    private void RemovePickedCubeFromLits(object s,CubeBase cubeBase)
    {
        if (targetCubeList.Contains(cubeBase))
        {
            randomNuber++;
            targetCubeList.Remove(cubeBase);    
        }
    }
    
  
    public override void ChechGround()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, rayDistance, groundLayer))
        {
        
            if (hit.collider.CompareTag("Slope"))
            {
                isOnStairs = true;
              
                desiredTurn = hit.collider.transform.right;
                Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
            }

            else
            {
                isOnStairs = false;
            }

        }
      
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("door"))
        {
            enemyState = EnemyState.WaitingState;
           
          
        }
    }
    private IEnumerator WaitingCoroutine()
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(1);
        if (isDoorOpened || isCubesSpwanedOnUpperPlane)
        {
            yield return null;
            enemyState = EnemyState.CollectingCubeState;
        }
        
    }
    public override void HandleMovemnt()
    {
        throw new NotImplementedException();
    }
    private IEnumerator GoToTarget1Coroutine()
    {
        if (levelNumberForAWin == 1)
        {
            if (agent.isStopped)
            {
                agent.isStopped = false;
            }
            agent.SetDestination(firstStagepaths[0].position);
          
            if (Vector3.Distance(firstStagepaths[0].position, transform.position) <= distanceToCube )
            {
                if (!isOnStairs && cubePicker.PickecObjects.Count > 1)
                {
                    enemyState = EnemyState.GoToUpState;
                    yield break;
                }
                else if(cubePicker.PickecObjects.Count <= 1)
                {
                    yield return null;
                    agent.enabled = false;
                    randomNuber = UnityEngine.Random.Range(minNumber, maxNumber);
                    enemyState = EnemyState.CollectingCubeState;
                    yield break;
                }
            
            }
          
            yield return null;
        }
        else if(levelNumberForAWin == 2)
        {
            if (agent.isStopped)
            {
                agent.isStopped = false;
            }
            agent.SetDestination(secondtStagepaths[0].position);

            if (Vector3.Distance(secondtStagepaths[0].position, transform.position) <= distanceToCube)
            {
                if (!isOnStairs && cubePicker.PickecObjects.Count > 1)
                {
                    enemyState = EnemyState.GoToUpState;
                    yield break;
                }
                else if (cubePicker.PickecObjects.Count <= 1)
                {
                    yield return null;
                    agent.enabled = false;
                    randomNuber = UnityEngine.Random.Range(minNumber, maxNumber);
                    enemyState = EnemyState.CollectingCubeState;
                    yield break;
                }

            }
           
            yield return null;
        }
      
    }
    private void ClampWinLevelNumneer()
    {
        levelNumberForAWin = Mathf.Clamp(levelNumberForAWin, minWinLevelNumber, maxWinLevelNumber);
    }
    private IEnumerator GoToPath2Coroutine()
    {
        
        if (levelNumberForAWin == 1)
        {

           
            agent.SetDestination(firstStagepaths[1].position);

           
            if (isOnStairs && cubePicker.PickecObjects.Count <= 1 )
            {
                yield return null;
                if (isDoorOpened && isCubesSpwanedOnUpperPlane)
                {
                    yield return null;
                    if (agent.enabled)
                    {
                        agent.isStopped = true;
                    }
                    ClampWinLevelNumneer();
                    OnRespawndedCubesUpdateTargetListAndEnemyState();
                }
                else
                {
                    yield return null;
                    if (agent.enabled)
                    {
                        agent.isStopped = true;
                    }
                
                    enemyState = EnemyState.GoToDownState;
                    print("Go Down.....");
                    yield break;
                }
               
              
              
            }
            print("levelNumberForAWin:" + levelNumberForAWin);
            yield return null;
        }
        else if (levelNumberForAWin == 2)
        {
            if (agent.isStopped)
            {
                agent.isStopped = false;
            }
            print("current staga" + levelNumberForAWin);
            agent.SetDestination(secondtStagepaths[1].position);
            if (isOnStairs && cubePicker.PickecObjects.Count <= 1)
            {
                print("Go Down.....");
                yield return new WaitForSeconds(waitTimeToGoBack);
                enemyState = EnemyState.GoToDownState;
                yield break;
            }
            yield return null;
        }
        yield return null;
    }
  
    private IEnumerator CollectCubeCoroutine()
    {
      
        if (targetCubeList.Count >= 1)
        {
           
            Vector3 targetPosition = targetCubeList[0].transform.position;
            transform.position = Vector3.MoveTowards(transform.position, targetCubeList[0].transform.position, moveSpeed * Time.deltaTime);
            Quaternion lookDirection = Quaternion.LookRotation(targetPosition - transform.position).normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, lookDirection, turnSpeed * Time.deltaTime);
           
            yield return null;
            if (randomNuber > maxNumber)
            {
                
                enemyState = EnemyState.GoToTarget;
             
                agent.enabled = true;
                yield break;
            }
        }
        yield return null;

    }
  
    public override void SpawnCubes(Transform GroundPlane)
    {
        
        if (!isCubesSpwanedOnUpperPlane)
        {
            isCubesSpwanedOnUpperPlane = true;
            upperPlane = GroundPlane;
            SpawnSytem.Instance.OnDoorOpened(upperPlane);
           
            print(levelNumberForAWin);
            OnRespawndedCubesUpdateTargetListAndEnemyState();
            print("spawnCubes on Upper Plane");
            isDoorOpened = true;
        }
        else if (levelNumberForAWin >= 2)
        {
            WinManager.Instance.DeclareWinner();
        }
    }
    public override void IncreaseNumber()
    {
        levelNumberForAWin++;
       
        OnRespawndedCubesUpdateTargetListAndEnemyState();
        ClampWinLevelNumneer();
    }
    private void OnRespawndedCubesUpdateTargetListAndEnemyState()
    {
        StartCoroutine(RespawnEventCoroutine());
    }
    private IEnumerator RespawnEventCoroutine()
    {
        
        yield return new WaitForSeconds(1);
        targetCubeList.Clear();
        if (characterColorType == CharacterColorType.Red)
        {
            targetCubes = new BlueCube[SpawnSytem.Instance.CubeInstaintaited[2].Length];
            targetCubes = SpawnSytem.Instance.CubeInstaintaited[2];
            targetCubeList = new List<CubeBase>();
            targetCubeList = targetCubes.ToList();
        }
        else if (characterColorType == CharacterColorType.Blue)
        {
            targetCubes = new BlueCube[SpawnSytem.Instance.CubeInstaintaited[0].Length];
            targetCubes = SpawnSytem.Instance.CubeInstaintaited[0];
            targetCubeList = new List<CubeBase>();
            targetCubeList = targetCubes.ToList();
        }
        else if (characterColorType == CharacterColorType.Green)
        {
            targetCubes = new BlueCube[SpawnSytem.Instance.CubeInstaintaited[1].Length];
            targetCubes = SpawnSytem.Instance.CubeInstaintaited[1];
            targetCubeList = new List<CubeBase>();
            targetCubeList = targetCubes.ToList();
        }
        yield return new WaitForSeconds(1);
        agent.enabled = false;
      
        levelNumberForAWin++;
        ClampWinLevelNumneer();
        randomNuber = UnityEngine.Random.Range(minNumber, maxNumber); 
        enemyState = EnemyState.CollectingCubeState;
       
    }

    public override void PlayAnimationsOnWin(bool isThisWiner)
    {
        if (isThisWiner)
        {
            animator.SetTrigger("Win");
        }
        else
        {
            animator.SetTrigger("Sad");
        }
    }
}
public enum CharacterColorType
{
    Red,
    Blue,
    Green,
}
public enum EnemyState
{
    CollectingCubeState,
    GoToUpState,
    GoToTarget,
    GoToDownState,
    WinState,
    GameOverState,
    WaitingState
}