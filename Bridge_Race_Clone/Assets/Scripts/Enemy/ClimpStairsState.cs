using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ClimpStairsState : EnemyState
{
    private EnemyAÝ enemyAÝ;
    private bool isOnStairs;
    private bool shouldClimpToStairs;
    private Transform taregtDestination;
 
    private PickObject cubePicker;

    public ClimpStairsState(NavMeshAgent _agent, Animator _animator, List<CubeBase> _pickecObjects, EnemyDataSO _enemyData, CharacterColorType _characterColorType, bool _isOnUpperPlane) : base(_agent, _animator, _pickecObjects, _enemyData, _characterColorType, _isOnUpperPlane)
    {
        cubePicker = agent.transform.GetChild(2).GetComponent<PickObject>();
        agent.isStopped = false;
    }

    protected override void Enter()
    {
        enemyAÝ = agent.GetComponent<EnemyAÝ>();
        enemyAÝ.OnDoorOpened += EnemyAÝ_OnDoorOpened;
        shouldClimpToStairs =isOnStairs = false;
         levelNumberForAWin = 1;
        Debug.Log("Entered to clipState...................");
        animator.SetFloat("BlendSpeed", 0.85f);
        base.Enter();
    }
    protected override void Update()
    {
        ChechGround();
        GoToTarget1();
    }
    private void EnemyAÝ_OnDoorOpened(object sender, System.EventArgs e)
    {      
        isOnUpperPlane = true;
    }
    protected override void Exit()
    {
        base.Exit();
    }
    private void GoToTarget1()
    {
        if (isOnUpperPlane)
        {
            Debug.Log("Enemy is On Upper Plane");
            if (agent.enabled)
            {
                agent.isStopped = false;
            }
            taregtDestination = PathsSingelton.instance.GetPathsList()[4];
            GoToFirstTarget(taregtDestination);
            if (shouldClimpToStairs)
            {
                GoToPath2Coroutine();
            }
        }
        else
        {
            if (agent.enabled)
            {
                agent.isStopped = false;
            }
            switch (characterColor)
            {
                case CharacterColorType.Green:
                    taregtDestination = PathsSingelton.instance.GetPathsList()[0];
                    GoToFirstTarget(taregtDestination);
                    break;

                case CharacterColorType.Red:
                    taregtDestination = PathsSingelton.instance.GetPathsList()[1];
                    GoToFirstTarget(taregtDestination);
                    break;
            }
            if (shouldClimpToStairs)
            {
                GoToPath2Coroutine();
            }
        }
    
       
    }
    public  void ChechGround()
    {
        Ray ray = new Ray(agent.transform.position + Vector3.forward * enemyData.rayOffsetDistance + Vector3.up * 0.1f, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit,enemyData.rayDistance,enemyData.groundLayer))
        {
            if (hit.collider.transform.CompareTag("drop") || !hit.collider.gameObject.GetComponent<MeshRenderer>().enabled)
            {

                Debug.DrawRay(ray.origin, ray.direction * 10, Color.green);

                isOnStairs = true;
            }
            else if (hit.collider.transform.CompareTag("drop") || hit.collider.gameObject.GetComponent<MeshRenderer>().enabled)
            {
                
                isOnStairs = false;
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
          
        }

    }
    private void GoToPath2Coroutine()
    {
      
        switch (characterColor)
        {
            case CharacterColorType.Green:
                taregtDestination = PathsSingelton.instance.GetPathsList()[2];
                GoToSecondTarget(taregtDestination);
                
                break;

            case CharacterColorType.Red:
                taregtDestination = PathsSingelton.instance.GetPathsList()[3];
                GoToSecondTarget(taregtDestination);

                break;
        }
        if (isOnUpperPlane)
        {
            taregtDestination = PathsSingelton.instance.GetPathsList()[5];
            GoToSecondTarget(taregtDestination);
        }
      
       
    }

    private void GoToFirstTarget(Transform targetDestination)
    {
        if (agent.enabled)
        {
            agent.SetDestination(targetDestination.position);
            if (Vector3.Distance(targetDestination.position, agent.transform.position) <= enemyData.distanceToCube)
            {

                if (!isOnStairs && cubePicker.PickecObjects.Count > 1 && !shouldClimpToStairs)
                {
                    shouldClimpToStairs = true;
                    Debug.Log(isOnStairs + "," + cubePicker.PickecObjects.Count);
                    GoToPath2Coroutine();

                }
                //else if (cubePicker.PickecObjects.Count <= 1)
                //{

                //    enemyData.RandomNumberForStoppinCollcting = Random.Range(enemyData.maxRandomNumber, enemyData.maxRandomNumber);
                //    nextState = new CollectCubesState(agent, animator, targetCubes, enemyData, characterColor);

                //}


            }
        }
      
       
      
    }
    
    private void GoToSecondTarget(Transform targetDestination)
    {
        if (agent.enabled)
        {
            agent.SetDestination(targetDestination.position);

            if (cubePicker.PickecObjects.Count < 1 && isOnStairs)
            {
                shouldClimpToStairs = false;
                Debug.Log("down");

                enemyData.RandomNumberForStoppinCollcting = Random.Range(enemyData.maxRandomNumber, enemyData.maxRandomNumber);
                nextState = new GoDownFromStairs(agent, animator, targetCubes, enemyData, characterColor, isOnUpperPlane);
                eventStages = EventStages.Exit;
                agent.isStopped = true;
            }
        }
     
      
    }


}
