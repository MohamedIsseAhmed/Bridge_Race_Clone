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
    [SerializeField] private float rayOffsetDistance = 0.1f;
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
    [SerializeField] private EnemyStateEnum enemyState;

    [SerializeField] private bool onStairs=false;
    private NavMeshAgent agent;
    private Vector3 hitRight;
    private bool isDoorOpened = false;
    [SerializeField] private int minWinLevelNumber = 1;
    [SerializeField] private int maxWinLevelNumber = 2;
    [SerializeField] private EnemyDataSO enemyData;
    private EnemyState enemyStateMachine;
    public event EventHandler OnDoorOpened;
    private void Awake()
    {
        enemyState = EnemyStateEnum.CollectingCubeState;
        agent = GetComponent<NavMeshAgent>();
       
    }
    private void  Start()
    {
        levelNumberForAWin = 1;
      
        cubePicker.OnPickedCube += RemovePickedCubeFromLits;

        if (characterColorType == CharacterColorType.Red)
        {
            targetCubes = new BlueCube[SpawnSytem.Instance.CubeInstaintaited[2].Length];
            targetCubes = SpawnSytem.Instance.CubeInstaintaited[2];
            targetCubeList = new List<CubeBase>();
            targetCubeList = targetCubes.ToList();
        }
        else if (characterColorType == CharacterColorType.Blue)
        {
            targetCubes = new RedCube[SpawnSytem.Instance.CubeInstaintaited[0].Length];
            targetCubes = SpawnSytem.Instance.CubeInstaintaited[0];
            targetCubeList = new List<CubeBase>();
            targetCubeList= targetCubes.ToList();
        }
        else if(characterColorType == CharacterColorType.Green)
        {
            targetCubes = new GreenCube[SpawnSytem.Instance.CubeInstaintaited[1].Length];
            targetCubes = SpawnSytem.Instance.CubeInstaintaited[1];
            targetCubeList = new List<CubeBase>();
            targetCubeList = targetCubes.ToList();
        }


     
        enemyStateMachine = new CollectCubesState(agent, animator, targetCubeList, enemyData,characterColorType, isDoorOpened);

        
    }

  

    private void Update()
    {
       enemyStateMachine=enemyStateMachine.ProcessStates();
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
            case EnemyStateEnum.CollectingCubeState:
                StartCoroutine(CollectCubeCoroutine());
                break;
            case EnemyStateEnum.GoToTarget:
                StartCoroutine(GoToTarget1Coroutine());
                break;
            case EnemyStateEnum.GoToUpState:
                StartCoroutine(GoToPath2Coroutine());
                break;
            case EnemyStateEnum.GoToDownState:
                StartCoroutine(GoToTarget1Coroutine());
                break;
            case EnemyStateEnum.WaitingState:
                StartCoroutine(WaitingCoroutine());
                break;
            case EnemyStateEnum.WinState:

                break;
            case EnemyStateEnum.GameOverState:
                break;
            default:
                break;
        }
    }

    private void RemovePickedCubeFromLits(object s,CubeBase cubeBase)
    {
        if(enemyStateMachine is CollectCubesState || enemyStateMachine is ClimpStairsState || enemyStateMachine is GoDownFromStairs)
        {
            if (enemyStateMachine.targetCubes.Contains(cubeBase))
            {
                enemyData.RandomNumberForStoppinCollcting++;
                enemyStateMachine.targetCubes.Remove(cubeBase);
            }
        }
      
    }
    
  
    public override void ChechGround()
    {
        Ray ray = new Ray(transform.position + Vector3.forward * rayOffsetDistance + Vector3.up * 0.1f, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, rayDistance, groundLayer))
        {
            if (hit.collider.transform.CompareTag("drop") && !hit.collider.gameObject.GetComponent<MeshRenderer>().enabled)
            {
                Debug.DrawLine(ray.origin, desiredPosition * 10, Color.black);
                print("stop");

                onStairs = true;
            }
            else if (hit.collider.transform.CompareTag("drop") && hit.collider.gameObject.GetComponent<MeshRenderer>().enabled)
            {
                print("go on");
                //desiredPosition.y += gravityModifier * Time.deltaTime;

                onStairs = false;
            }
            
            //if (hit.collider.CompareTag("Slope"))
            //{
            //    isOnStairs = true;

            //    desiredTurn = hit.collider.transform.right;
            //    Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
            //}

            //else
            //{
            //    isOnStairs = false;
            //}
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
        }
      
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("door"))
        {
            enemyState = EnemyStateEnum.WaitingState;
            isDoorOpened = true;


        }
    }
    private IEnumerator WaitingCoroutine()
    {
        if (agent.enabled)
        {
            agent.isStopped = true;
        }
        yield return new WaitForSeconds(1);
        if (isDoorOpened || isCubesSpwanedOnUpperPlane)
        {
            yield return null;
            enemyState = EnemyStateEnum.CollectingCubeState;
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
            if (agent.enabled)
            {
                agent.isStopped = false;
            }
            agent.SetDestination(firstStagepaths[0].position);
          
            if (Vector3.Distance(firstStagepaths[0].position, transform.position) <= distanceToCube )
            {
                if (!onStairs && cubePicker.PickecObjects.Count > 1)
                {
                    enemyState = EnemyStateEnum.GoToUpState;
                    yield break;
                }
                else if(cubePicker.PickecObjects.Count <= 1)
                {
                   
                 
                    randomNuber = UnityEngine.Random.Range(minNumber, maxNumber);
                    enemyState = EnemyStateEnum.CollectingCubeState;
                    yield break;
                }
            
            }
          
            yield return null;
        }
        else if(levelNumberForAWin == 2)
        {
            if (agent.enabled)
            {
                agent.isStopped = false;
            }
            agent.SetDestination(secondtStagepaths[0].position);

            if (Vector3.Distance(secondtStagepaths[0].position, transform.position) <= distanceToCube)
            {
                Debug.DrawRay(transform.position,-transform.up * 10, Color.blue);

                if (!onStairs && cubePicker.PickecObjects.Count > 1)
                {
                    enemyState = EnemyStateEnum.GoToUpState;
                    yield break;
                }
                else if (cubePicker.PickecObjects.Count <= 1)
                {
                  
                    randomNuber = UnityEngine.Random.Range(minNumber, maxNumber);
                    enemyState = EnemyStateEnum.CollectingCubeState;
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

            if (onStairs && cubePicker.PickecObjects.Count <= 1 )
            {
               
                if (isDoorOpened && isCubesSpwanedOnUpperPlane)
                {
                    
                    if (agent.enabled)
                    {
                        agent.isStopped = true;
                    }
                    ClampWinLevelNumneer();
                    OnRespawndedCubesUpdateTargetListAndEnemyState();
                     yield break;
                }
                else
                {
                    yield return null;
                    if (agent.enabled)
                    {
                        agent.isStopped = true;
                    }
                
                    enemyState = EnemyStateEnum.GoToDownState;
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
            if (onStairs && cubePicker.PickecObjects.Count <= 1)
            {
                print("Go Down.....");
                yield return new WaitForSeconds(waitTimeToGoBack);
                enemyState = EnemyStateEnum.GoToDownState;
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
            agent.enabled = true;
            agent.SetDestination(targetCubeList[0].transform.position);
            //Vector3 targetPosition = targetCubeList[0].transform.position;
            //transform.position = Vector3.MoveTowards(transform.position, targetCubeList[0].transform.position, moveSpeed * Time.deltaTime);
            //Quaternion lookDirection = Quaternion.LookRotation(targetPosition - transform.position).normalized;
            //transform.rotation = Quaternion.Slerp(transform.rotation, lookDirection, turnSpeed * Time.deltaTime);
           
            yield return null;
            if (randomNuber > maxNumber)
            {
                
                enemyState = EnemyStateEnum.GoToTarget;
              
                agent.enabled = true;

                yield break;
            }
        }
        yield return null;

    }
  
    public override void SpawnCubes(Transform GroundPlane)
    {
        OnDoorOpened?.Invoke(this, EventArgs.Empty);
        if (!isCubesSpwanedOnUpperPlane)
        {
            upperPlane = GroundPlane;
            SpawnSytem.Instance.OnDoorOpened(upperPlane);
           
            print(levelNumberForAWin);
            OnRespawndedCubesUpdateTargetListAndEnemyState();
            print("spawnCubes on Upper Plane");
            isDoorOpened = true;
            isCubesSpwanedOnUpperPlane = true;
        }
        else
        {
            //OnRespawndedCubesUpdateTargetListAndEnemyState();
            print("cubes already spawned");
            // enemyStateMachine = new CollectCubesState(agent, animator, targetCubeList, enemyData, characterColorType, isDoorOpened);
            OnRespawndedCubesUpdateTargetListAndEnemyState();
        }
        //else if (levelNumberForAWin >= 2)
        //{
        //    WinManager.Instance.DeclareWinner();
        //}
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
      yield return new WaitForSeconds(0.25f);
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
      yield return new WaitForSeconds(0.2f);
     
     
       levelNumberForAWin++;
       ClampWinLevelNumneer();
       randomNuber = UnityEngine.Random.Range(minNumber, maxNumber);
       enemyStateMachine = new CollectCubesState(agent, animator, targetCubeList, enemyData, characterColorType, isDoorOpened);

    }

    public override void PlayAnimationsOnWin(bool isThisWiner)
    {
        agent.enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
        if (isThisWiner)
        {
            animator.SetTrigger("Win");
        }
        else
        {
            animator.SetTrigger("Sad");
        }
        for (int i = 1; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
public enum CharacterColorType
{
    Red,
    Blue,
    Green,
}
public enum EnemyStateEnum
{
    CollectingCubeState,
    GoToUpState,
    GoToTarget,
    GoToDownState,
    WinState,
    GameOverState,
    WaitingState
}