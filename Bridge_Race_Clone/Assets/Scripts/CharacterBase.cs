using System;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
  
    protected Vector3 desiredPosition;
    protected Vector3 desiredTurn;
    protected Vector3 velocity;
    protected Vector3 velocityXZ;
    protected Transform upperPlane;
    protected int  levelNumberForAWin;
    protected float gravityModifier=-9.81f;
    protected float rayDistance = 10f;
    protected bool isOnStairs = false;
    protected bool isGrounded = false;
    protected static bool isCubesSpwanedOnUpperPlane = false;


    public virtual void ChechGround()
    {

    }
    public virtual void HandleMovemnt()
    {

    }
    public abstract void PlayAnimationsOnWin(bool isThisWiner);
    public abstract void SpawnCubes(Transform GroundPlane);
    public  virtual void IncreaseNumber()
    {
        levelNumberForAWin++;
    }
}
