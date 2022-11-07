using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
public class EnemyState:MonoBehaviour
{
    protected NavMeshAgent agent;
    protected Animator animator;
    protected  int levelNumberForAWin; 
    public List<CubeBase> targetCubes;
    protected EnemyDataSO enemyData;
    protected CharacterColorType characterColor;
    protected EnemyState nextState;
    protected bool isOnUpperPlane;
    public enum EventStages
    {
        Enter,
        Update,
        Exit
    }
    protected EventStages eventStages;
    public EnemyState(NavMeshAgent _agent, Animator _animator, List<CubeBase> _pickecObjects, EnemyDataSO  _enemyData,CharacterColorType _characterColorType,bool _isOnUpperPlane)
    {
        this.agent = _agent;
        this.animator = _animator;
        this.targetCubes = _pickecObjects;
        this.enemyData = _enemyData;
        this.characterColor = _characterColorType;
        this.isOnUpperPlane = _isOnUpperPlane;
    }

    protected  virtual void Enter()
    {
        eventStages = EventStages.Update;
    }
  
    protected virtual void Update()
    {
        eventStages = EventStages.Update;
    }
    protected virtual void Exit()
    {
        eventStages = EventStages.Exit;
    }
    public EnemyState ProcessStates()
    {
        if (eventStages == EventStages.Enter)
        {
            Enter();
        }
        if (eventStages == EventStages.Update)
        {
            Update();
        }
        if (eventStages == EventStages.Exit)
        {
            Exit();
            return nextState;
        }

        return this;
    }
}
