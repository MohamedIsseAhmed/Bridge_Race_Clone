using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoDownFromStairs : EnemyState
{
    private EnemyAÝ enemyAÝ;

    public GoDownFromStairs(NavMeshAgent _agent, Animator _animator, List<CubeBase> _pickecObjects, EnemyDataSO _enemyData, CharacterColorType _characterColorType, bool _isOnUpperPlane) : base(_agent, _animator, _pickecObjects, _enemyData, _characterColorType, _isOnUpperPlane)
    {

    }

    protected override void Enter()
    {
        enemyAÝ=agent.GetComponent<EnemyAÝ>();  
        enemyAÝ.OnDoorOpened += EnemyAÝ_OnDoorOpened;
        agent.isStopped = false;
        base.Enter();
    }
    private void EnemyAÝ_OnDoorOpened(object sender, System.EventArgs e)
    {
        isOnUpperPlane = true;
    }
    protected override void Update()
    {
        if (isOnUpperPlane)
        {
            GoDown(PathsSingelton.instance.GetPathsList()[4]);
        }
        else
        {
            switch (characterColor)
            {
                case CharacterColorType.Green:
                    GoDown(PathsSingelton.instance.GetPathsList()[0]);
                    break;

                case CharacterColorType.Red:
                    GoDown(PathsSingelton.instance.GetPathsList()[1]);
                    break;
            }
        }
    
    }
    protected override void Exit()
    {
        base.Exit();
    }
    private void GoDown(Transform targetDestination)
    {
        if (agent.enabled)
        {
            agent.SetDestination(targetDestination.position);
            if (Vector3.Distance(targetDestination.position, agent.transform.position) <= enemyData.distanceToCube)
            {
                nextState = new CollectCubesState(agent, animator, targetCubes, enemyData, characterColor, isOnUpperPlane);
                eventStages = EventStages.Exit;
            }
        }
      
    }
}
