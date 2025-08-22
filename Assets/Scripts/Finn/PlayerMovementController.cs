using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class PlayerMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float rotationSpeed = 10f;
    [SerializeField]private float currentSpeed;
    bool isRunning;
    bool isFollowDestination;
    bool isOnRaft;
    [Header("Jump & Gravity")]
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    public float groundedOffset = 0.1f;  
    public float jumpCooldown = 0.5f; 
    public LayerMask groundMask;
    public bool canMove=true;
    [Header("Animation")]
    private CharacterController cc;
    [SerializeField]private Animator animator;
    private Vector3 velocity;


    private bool isGrounded;
    private bool canJump=true;
    private bool canRoll;
    private bool wasGrounded;
    private bool isFalling;
   
    private int hashSpeed;
    private int hashDirection;
    private int hashJump;
    private int hashIsGrounded;
    private int hashRoll = Animator.StringToHash("Roll");
    [Header("Roll")]
    public float rollDistance = 2f; 
    public float rollDuration = 0.5f;
    public float rollCooldown = 1f;
    private bool isRolling;
    private Ease rollEase;
    public static PlayerMovementController Instance { get; private set; }

    private void Awake()
    {
        Instance=this;
    }
    void Start()
    {
        cc = GetComponent<CharacterController>();


        hashSpeed = Animator.StringToHash("Speed");
        hashDirection = Animator.StringToHash("Direction");
        hashJump = Animator.StringToHash("Jump");
        hashIsGrounded = Animator.StringToHash("IsGrounded");
        Raft.OnDestinationReached += Raft_OnDestinationReached;
       
    }

    private void Raft_OnDestinationReached()
    {
        isOnRaft = false;
        transform.parent = null;
        animator.SetTrigger("Jump");
    }

    void Update()
    {
        CheckAltitudeThreshold();

        if (isFollowDestination||isOnRaft||!canMove) return;

        HandleMovement();
        HandleGravityAndJump();
    
    }

    private void HandleMovement()
    {
        if(isRolling||isFalling ) return;
        float h = Input.GetAxis("Horizontal"); 
        float v = Input.GetAxis("Vertical");   

        Vector3 moveInput = new Vector3(h, 0, v);
        float inputMagnitude = Mathf.Clamp01(moveInput.magnitude);

        
        Vector3 move = transform.forward * inputMagnitude;

      
        bool isJumped = !isGrounded;
        if (!isJumped&&!isRolling)
        {
            isRunning = Input.GetKey(KeyCode.LeftShift);
            currentSpeed = isRunning ? runSpeed : walkSpeed;
        }



        

      
        if (moveInput.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveInput);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

      
        animator.SetFloat(hashSpeed, inputMagnitude * (isRunning ? 1f : 0.5f), 0.1f, Time.deltaTime);
        animator.SetFloat(hashDirection, h, 0.1f, Time.deltaTime);



        cc.Move(move * currentSpeed * Time.deltaTime);
    }

    private void HandleGravityAndJump()
    {
       
        Vector3 spherePos = transform.position + Vector3.down * groundedOffset;
        isGrounded = Physics.CheckSphere(spherePos, cc.radius * 0.9f, groundMask);

        animator.SetBool(hashIsGrounded, isGrounded);


        if (isGrounded && !wasGrounded)
        {

            StartCoroutine(JumpCoolDown());
        }

        if (isGrounded && Input.GetButtonDown("Jump")&&canJump)
        {
            
            float jumpVel = Mathf.Sqrt(jumpHeight * -2f * gravity);
            velocity.y = jumpVel;
            animator.SetTrigger(hashJump);
            StartCoroutine(JumpCoolDown());
           

        }

        if (isGrounded && Input.GetKeyDown(KeyCode.LeftControl) && canRoll)
        {
            isRolling = true;
            
            animator.SetTrigger(hashRoll);
            StartCoroutine(PerformRoll());
        }


        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }

        cc.Move(velocity * Time.deltaTime);
        
        wasGrounded = isGrounded;
        
    }

    
    public IEnumerator MoveToDestination(Vector3 destination,Action onStartedToMove=null,Action onCompleted=null)
    {
        onStartedToMove?.Invoke();
        
        isFollowDestination = true;
        Vector3 startPos = transform.position;
        float distance = Vector3.Distance(transform.position, destination);
        float duration = distance / walkSpeed;
        float elapsedTime = 0f;
        animator.SetFloat("Speed", 0f);
        while (elapsedTime < duration)
        {
            
            float t = Mathf.Clamp01(elapsedTime / duration);
            Vector3 newPos = Vector3.Lerp(startPos, destination, t);
            transform.SetPositionAndRotation(newPos,
                Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(newPos - transform.position), rotationSpeed * Time.deltaTime));

            animator.SetFloat("Speed", 0.5f, 0.1f, Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        isFollowDestination = false;
        onCompleted?.Invoke();
    }

    public IEnumerator SetSail(Transform raftTransform,Action onStartedToSail=null)
    {

        yield return MoveToDestination(raftTransform.transform.position, onStartedToMove: () =>{ isOnRaft = true; });
        onStartedToSail?.Invoke();  
        animator.SetFloat("Speed", 0f);
        transform.parent = raftTransform;
       
       
    }
 
    public IEnumerator Elevate(StoneHandPuzzle rockHandTransform)
    {
        
        canMove = false;
        yield return MoveToDestination(rockHandTransform.targetPlayerPos.position);
        animator.SetFloat("Speed", 0f);
        transform.parent = rockHandTransform.targetPlayerPos.transform;

    }
    
    void OnDrawGizmosSelected()
    {
        Color color = isGrounded ? Color.green : Color.red;
        Gizmos.color = color;
        Vector3 spherePos = transform.position + Vector3.down * groundedOffset;
        Gizmos.DrawWireSphere(spherePos, cc != null ? cc.radius * 0.9f : 0.5f);
        
    }


    public void SetAnimation(String name,bool asd)
    {
        animator.SetBool(name,asd);
    }
    public IEnumerator PushOverTime(Action OnCompleted=null)
    {
        isFalling = true;
        canMove = true;
        float duration = 2f;
        float elapsedTime = 0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + transform.forward * 2f;
         float startGravity = gravity;
        gravity = -20;
        while (!isGrounded)
        {
            float t = elapsedTime / duration;
           
            cc.Move(transform.forward * Time.deltaTime * 25f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        OnCompleted?.Invoke();
        gravity = startGravity;
        yield return new WaitForSeconds(0.5f);
        isFalling = false;
        CameraController.Instance.SwitchToCamera(CameraType.PlayerCam);

    }

    public void SetMove(bool canmove)
    {
        canMove=canmove;
        animator.SetFloat(hashSpeed, 0f);
        
        
    }
    

    private IEnumerator JumpCoolDown()
    {
        float speed= animator.GetFloat(hashSpeed);
        float targetCoolDown= speed>0 ? 1f : jumpCooldown;
        canJump = false;
        canRoll = false;
        yield return new WaitForSeconds(targetCoolDown);
        canJump = true;
        canRoll = true;
      
    }

    private IEnumerator DoRollMovement(float duration, float distance)
    {


        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + transform.forward * distance;

        yield return new WaitForSeconds(0.1f);
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            Vector3 newPos = Vector3.Lerp(startPos, endPos, t);
            cc.Move(newPos - transform.position);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cc.Move(endPos - transform.position);

    }

    private IEnumerator PerformRoll()
    {
        isRolling = true;
        canRoll = false;
        canJump = false;
        animator.SetTrigger(hashRoll);
        animator.SetBool("isRolling", true);

        yield return DoRollMovement(rollDuration, rollDistance); 

        isRolling = false;
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("isRolling", false);

        yield return new WaitForSeconds(rollCooldown); 
        canRoll = true;
        canJump = true;
    }

    private void CheckAltitudeThreshold()
    {
        if (transform.position.y < -60f) 
        {
            Vector3 checkpointPos = CheckpointController.Instance.GetCheckpointPos();

            cc.enabled = false;
            transform.position = checkpointPos;
            cc.enabled = true;

            velocity = Vector3.zero; 
        }
    }
}
