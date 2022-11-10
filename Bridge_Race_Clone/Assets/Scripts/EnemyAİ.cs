using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using System.Threading.Tasks;
using System.Linq;
using DG.Tweening;
public class EnemyAİ : CharacterBase
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Vector3 jumpOffset; 
    [SerializeField] private float sphereRadius = 2f;
    [SerializeField] private float jumpPower = 2f;
    [SerializeField] private int numberOfJumps = 2;
    [SerializeField] private float jumpDuration = 2f;
    [SerializeField] private float distanceToCube = 2f;
    [SerializeField] private float turnSpeed = 5f;
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float waitTime = 0.1f;
    [SerializeField] private float rayOffsetDistance = 0.1f;
    [SerializeField] private float waitTimeToGoBack = 0.1f;
    [SerializeField] private float angel = 90f;
    [SerializeField] Collider[] resultHits;
    [SerializeField] private Animator animator;
    [SerializeField] private Animation[] animationClips;
   
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform[]  firstStagepaths;
    [SerializeField] private Transform[]  secondtStagepaths;
    [SerializeField] private PickObject cubePicker;
    [SerializeField] private Transform doorTrigger;
    [SerializeField] private Transform cubeParent;
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
    private bool hasJumped = false;
    private Rigidbody rigidbody;
    private void Awake()
    {
        enemyState = EnemyStateEnum.CollectingCubeState;
        agent = GetComponent<NavMeshAgent>();
        rigidbody = GetComponent<Rigidbody>();


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
        if (hasJumped)
        {
            return;
        }

       enemyStateMachine = enemyStateMachine.ProcessStates();
   
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
    
    public void BackToCollectingCubesStateAnimationEvenet()
    {
        
        enemyStateMachine = new CollectCubesState(agent, animator, targetCubeList, enemyData, characterColorType, isDoorOpened);
        enemyStateMachine = enemyStateMachine.ProcessStates();
        agent.enabled = true;
        hasJumped = false;
    }
 
    private  void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("door"))
        {
            enemyState = EnemyStateEnum.WaitingState;
            isDoorOpened = true;


        }
        else if (other.transform.CompareTag("Player") && !hasJumped)
        {
           StartCoroutine(Jump(other.transform));
        }
    }
    private IEnumerator  Jump(Transform other)
    {
       
        hasJumped = true;
     
        enemyStateMachine = new JumpState(agent, animator, targetCubeList, enemyData, characterColorType, isDoorOpened);
        enemyStateMachine = enemyStateMachine.ProcessStates();
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
      
        for (int i = 1; i < cubeParent.childCount; i++)
        {
        
            GameObject toRemove = cubeParent.GetChild(i).gameObject;
            cubePicker.PickecObjects.Remove(toRemove.transform);
          
            Destroy(toRemove);

        }
        cubePicker.FirstCube = cubeParent.GetChild(0).transform;
        cubePicker.ResetCollectingPosition();
       
        transform.DOLocalJump(transform.position + jumpOffset, jumpPower, numberOfJumps, jumpDuration).OnComplete(() =>
        {
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
           

        });
        yield return null;
        
        
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
           
            OnRespawndedCubesUpdateTargetListAndEnemyState();
        }
       
    }
    public override void IncreaseNumber()
    {
        levelNumberForAWin++;
       
        OnRespawndedCubesUpdateTargetListAndEnemyState();
       
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