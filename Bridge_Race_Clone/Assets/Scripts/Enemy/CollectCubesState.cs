using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CollectCubesState : EnemyState
{
    private EnemyAÝ enemyAÝ;
    public CollectCubesState(NavMeshAgent _agent, Animator _animator, List<CubeBase> _pickecObjects, EnemyDataSO _enemyData, CharacterColorType _characterColorType, bool _isOnUpperPlane) : base(_agent, _animator, _pickecObjects, _enemyData, _characterColorType, _isOnUpperPlane)
    {
        enemyData.RandomNumberForStoppinCollcting = 0;
    }

    protected override void Enter()
    {
        agent.enabled = true;
        enemyAÝ=agent.GetComponent<EnemyAÝ>();
        //enemyAÝ.OnDoorOpened += EnemyAÝ_OnDoorOpened;
        Debug.Log("Enter");
        agent.isStopped = false;
        animator.SetFloat("BlendSpeed", 0.85f);
        enemyData.RandomNumberForStoppinCollcting = Random.Range(enemyData.minRandomNumber, enemyData.maxRandomNumber);
        base.Enter();
    }

    protected override void Update()
    {
        if (agent.enabled)
        {
            if (targetCubes.Count > 0)
            {
                agent.SetDestination(targetCubes[0].transform.position);

                if (enemyData.RandomNumberForStoppinCollcting > enemyData.maxRandomNumber)
                {
                    agent.isStopped = true;

                    Debug.Log("stop...................");

                    nextState = new ClimpStairsState(agent, animator, targetCubes, enemyData, characterColor, isOnUpperPlane);
                    eventStages = EventStages.Exit;
                }
            }
          
        }
       
    }
    
    protected override void Exit()
    {
        Debug.Log(nextState.GetType().Name);
        animator.SetFloat("BlendSpeed", 0f);
        base.Exit();
    }
}
