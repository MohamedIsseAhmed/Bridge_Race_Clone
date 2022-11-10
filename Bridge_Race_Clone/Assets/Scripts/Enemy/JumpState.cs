using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
public class JumpState : EnemyState
{
   
    public JumpState(NavMeshAgent _agent, Animator _animator, List<CubeBase> _pickecObjects, EnemyDataSO _enemyData, CharacterColorType _characterColorType, bool _isOnUpperPlane) : base(_agent, _animator, _pickecObjects, _enemyData, _characterColorType, _isOnUpperPlane)
    {
       
    }
   
    protected async  override void Enter()
    {
        agent.enabled = false;
        
        await DoFallingBack();

    }
 
    private async Task DoFallingBack()
    {
        animator.SetTrigger("FallingBack");
        await Task.Yield();
         BackToCollectingCubesStates();
    }
   
    private void  BackToCollectingCubesStates()
    {
        nextState = new CollectCubesState(agent, animator, targetCubes, enemyData, characterColor, isOnUpperPlane);
        eventStages = EventStages.Exit;
    }
    protected override void Exit()
    {
        animator.ResetTrigger("FallingBack");
        animator.ResetTrigger("StandingUp");
        base.Exit();
       
    }
}
