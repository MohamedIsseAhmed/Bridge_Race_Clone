using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Player : CharacterBase
{

    [SerializeField] private DynamicJoystick dynamicJoystick;
    [SerializeField] private Animator animator;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private float maxAngInRad = 1f;
    [SerializeField] private float rayOffsetDistance = 1f;
    [SerializeField] private float smoothJoystickSpeed = 0.7f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform pickedCubesParent;
    [SerializeField] private PickObject  PickUpComponent;
    [SerializeField] private Vector3 pickedCubesParentRotaion;

    [SerializeField] private float desiredBlendSpeed = 0;
    private CharacterController characterController;
    private CapsuleCollider capsuleCollider;
    private Rigidbody rigidbody;

    [SerializeField] private bool stop = false;

    private float angel = 0;
    [SerializeField] private float angelSpeed = 5;
 
    [SerializeField] private float smoothSpeed=0.1f;
    [SerializeField] private float gravitySpeed=-9.81f;
    [SerializeField] private float smoothTime = 0.3f;
    [SerializeField] private Vector3 moveVelocity;

    private float smoothInputMagnitude;
    private float smoothVelocity;
    private float yVelocityTarget;
    private void Awake()
    {
        rigidbody=GetComponent<Rigidbody>();    
        isCubesSpwanedOnUpperPlane = false;
         
    }
    private void Start()
    {
        Physics.gravity =new Vector3(0, gravitySpeed,0);
        levelNumberForAWin = 1;
        characterController = GetComponent<CharacterController>();
        
    }

 

    private void Update()
    {
        if (WinManager.Instance.IsGameOver)
        {
            return;
        }
      
        HandleMovemnt();
        ChechGround();

    }
   
    public override void HandleMovemnt()
    {
        if(stop && dynamicJoystick.Vertical > 0 && PickUpComponent.PickecObjects.Count <=0)
        {
            smoothJoystickSpeed = 0;          
             return;
        }
        else
        {
            smoothJoystickSpeed = 0.7f;
        }
      
        if(isOnStairs && PickUpComponent.PickecObjects.Count <= 1)
        {
            print("return");
        }

        if (Input.GetMouseButton(0))
        {

         
            float blendSpeedNormalized = desiredPosition.magnitude / 1;

            desiredBlendSpeed = Mathf.Lerp(animator.GetFloat("BlendSpeed"), 1, blendSpeedNormalized);
            animator.SetFloat("BlendSpeed", desiredBlendSpeed);


        }
        if (Input.GetMouseButtonUp(0))
        {
            desiredBlendSpeed = 0;
            animator.SetFloat("BlendSpeed", desiredBlendSpeed);
          

        }



        float horizontal = dynamicJoystick.Horizontal * smoothJoystickSpeed;
        float vertical = dynamicJoystick.Vertical * smoothJoystickSpeed;

        float yVelocity = Mathf.SmoothDamp(rigidbody.velocity.y, rigidbody.velocity.y, ref yVelocityTarget, smoothTime);
        desiredPosition = new Vector3(horizontal, 0, vertical) + new Vector3(0, yVelocity, 0);
        float inputMagnitude = desiredPosition.magnitude;
        //smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothVelocity, smoothSpeed);
        float ang›nDeg = Mathf.Atan2(desiredPosition.x, desiredPosition.z) * Mathf.Rad2Deg;

        angel = Mathf.LerpAngle(angel, ang›nDeg, angelSpeed * Time.deltaTime * inputMagnitude);
        transform.rotation = Quaternion.AngleAxis(angel, Vector3.up);

       

       
        moveVelocity = desiredPosition * moveSpeed * Time.deltaTime;
       
      
        transform.Translate(moveVelocity, Space.World);
       
       
    }
    private void MoveWithRay()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.transform.position.z;

        Ray ray=Camera.main.ScreenPointToRay(mousePos);
        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity,groundLayer))
        {
            Vector3 hitpoint=hit.point;
            transform.position = Vector3.Lerp(transform.position, hitpoint, moveSpeed * Time.deltaTime);
            Vector3 direction =new Vector3(hitpoint.x,transform.position.y, hitpoint.z);    
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction - transform.position).normalized, turnSpeed * Time.deltaTime);
           
        }
    }
  

    public override void ChechGround()
    {
      
        Ray ray = new Ray(transform.position+Vector3.forward*rayOffsetDistance+Vector3.up*0.1f, Vector3.down);
        RaycastHit hit;
      
        if(Physics.Raycast(ray,out hit, rayDistance, groundLayer,QueryTriggerInteraction.Collide))
        {
            
            isGrounded = true;
           
            Debug.DrawLine(ray.origin, ray.direction * 10, Color.blue);
          
            if (hit.collider.transform.CompareTag("drop"))
            {
                if(PickUpComponent.PickecObjects.Count>0 || hit.collider.gameObject.GetComponent<MeshRenderer>().material.color == PickUpComponent.FirstCubeColor)
                {
                    stop = false;
                }
                else if(PickUpComponent.PickecObjects.Count <= 0 || hit.collider.gameObject.GetComponent<MeshRenderer>().material.color != PickUpComponent.FirstCubeColor)
                {
                    stop = true;
                }

            }
            else
            {
                stop = false;
            }

          
        }
    
    }
  
    private void ClampY()
    {
        Vector3 position = transform.position;
        position.y = Mathf.Clamp(position.y,0, 1);
        transform.position = position;
    }
    private Vector3 RotateVector(Vector3 direction,float angel)
    {
        return Quaternion.Euler(angel, 0, 0) * direction;
    }
    private void ClimpLader(Vector3 turnDirection)
    {
        velocity.y += gravityModifier * Time.deltaTime;
        float ang›nDeg = Mathf.Atan2(turnDirection.x, turnDirection.z) * Mathf.Rad2Deg;
        angel = Mathf.LerpAngle(angel, ang›nDeg, angelSpeed * Time.deltaTime * turnDirection.magnitude);
        transform.rotation = Quaternion.AngleAxis(angel, Vector3.up);
        desiredTurn = turnDirection;
    }
    public override void IncreaseNumber()
    {
       levelNumberForAWin++;
    }
    public override void SpawnCubes(Transform GroundPlane)
    {
        print("Tray on Upper Plane");
        if (!isCubesSpwanedOnUpperPlane && levelNumberForAWin<=2)
        {
            isCubesSpwanedOnUpperPlane = true;
            upperPlane = GroundPlane;
            SpawnSytem.Instance.OnDoorOpened(upperPlane);
            levelNumberForAWin++;
            print(levelNumberForAWin);
            print("spawnCubes on Upper Plane");
        }
        else if (levelNumberForAWin >= 2)
        {
            WinManager.Instance.DeclareWinner();
        }
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
        for (int i = 1; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
