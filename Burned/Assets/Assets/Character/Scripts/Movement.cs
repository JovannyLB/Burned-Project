using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour{

    [Header("Movement data")]
    [SerializeField] private float rotationSpeed = 0.3f;
    [SerializeField] private float allowRotation = 0.1f;
    [SerializeField] public float moveSpeed;
    [SerializeField] public float sprintSpeed;
    [SerializeField] public float aimSpeed;
    [SerializeField] public float jumpForce;
    [SerializeField][Range(0, 1)] public float gravityMultiplier;
    [SerializeField][Range(0, 1)] public float startUpModifier;
    [SerializeField] [Range(0, 1)] public float timeMultiplier;

    // Inputs, turnspeed and gravity
    private float inputX, inputZ, turnSpeed, gravity, targerSpeed, endSpeed;
    
    // Inputs
    private bool shiftLeft, controlLeft, spaceBar;
    
    // Is able to run
    private bool canRun;
    // Is aiming
    public bool aiming;

    // Camera based movemment
    private Vector3 desiredMoveDirection;
    // Character movement
    private Vector3 moveDirection;
    
    [Header("Cameras")]
    // Main camera
    private Camera cam;
    
    // Components
        // Character controller
    private CharacterController characterController;
    
    // Sound clips
    public AudioClip jumpA, fallA;
    private AudioSource charAudioSource;

    // State machines
    public enum PositionState{
        onGround,
        onAir
    }
    private PositionState positionState;

    private Animator animator;

    void Start(){
        StartUp();
    }

    void Update(){
        InputManager();
        InputDecider();
        MovementManager();
        AnimationHandler();
    }

    // Gets the components
    private void StartUp(){
        // Character contoller
        characterController = GetComponent<CharacterController>();
        // Main camera
        cam = Camera.main;
        // Animator
        animator = transform.GetChild(0).GetComponent<Animator>();
        // Audio source
        charAudioSource = GetComponent<AudioSource>();
    }

    // Gets the inputs
    private void InputManager(){
        // Axis
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");
        
        // Keys
        shiftLeft = Input.GetKey(KeyCode.LeftShift);
        spaceBar = Input.GetKeyDown(KeyCode.Space);
        controlLeft = Input.GetKey(KeyCode.LeftControl);

        aiming = GetComponent<PlayerController>().aiming;
    }

    // Rotates the character
    private void InputDecider(){
        turnSpeed = new Vector2(inputX, inputZ).sqrMagnitude;

        if (turnSpeed > allowRotation){
            RotationManager();
        }
        else{
            desiredMoveDirection = Vector3.zero;
        }
    }

    // Gets the vectors of the camera
    private void RotationManager(){
        var forward = cam.transform.forward;
        var right = cam.transform.right;

        forward.y = 0;
        right.y = 0;
        
        forward.Normalize();
        right.Normalize();

        desiredMoveDirection = forward * inputZ + right * inputX;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(forward), rotationSpeed);
    }

    // Controls the movement
    private void MovementManager(){
        // Checks if playsr can run and is running
        if (shiftLeft && canRun){
            endSpeed = sprintSpeed; 
        }
        else{
            endSpeed = moveSpeed;
        }
        
        // Slows down time
        if (aiming){
            Time.timeScale = timeMultiplier;
        }
        else if (Time.timeScale != 1){
            Time.timeScale = 1;
        }

        // Creates gravity
        gravity -= 9.8f * Time.deltaTime;
        gravity *= gravityMultiplier;
        // Applies said gravity to movement
        moveDirection = new Vector3(moveDirection.x, moveDirection.y + gravity, moveDirection.z);
        
        // Jump check
        if (isGrounded()){
            if (spaceBar){
                // Adds vertical force
                positionState = PositionState.onAir;
                moveDirection.y = jumpForce;
                PlaySound(jumpA);
            }
            else{
                positionState = PositionState.onGround;
                moveDirection.y = 0;
                // Applies said speed to the movement if on ground (lets jump carry over speed)
//                if (controlLeft && !aiming){
//                    // Applies movement when sliding if sliding is possible (speed-wise)
//                    if (moveDirection.magnitude >= 2){
//                        moveDirection *= endSpeed >= 15 ? 0.985f : 0.970f;
//                    }
//                    else{
//                        moveDirection = Vector3.zero;
//                    }
//                }
//                else{
                    // Applies movement when on ground and standing (Smooths out the start)
                    DOTween.To(()=> targerSpeed, x=> targerSpeed = x, endSpeed, 0.5f);
//                    moveDirection = new Vector3(desiredMoveDirection.x * endSpeed, moveDirection.y, desiredMoveDirection.z * endSpeed);
                    moveDirection = new Vector3(desiredMoveDirection.x * targerSpeed, moveDirection.y, desiredMoveDirection.z * targerSpeed);
//                }
            }
            gravity = 0f;
        }

        // Checks is character can run
        if (inputZ > 0 && inputX < 0.2f && inputX > -0.2f && !aiming){
            canRun = true;
        }
        else{
            canRun = false;
        }
        
        // Applies the movement
        characterController.Move(moveDirection * Time.deltaTime);
    }

    // More extensive ground check
    private bool isGrounded(){
        // Only checks if the state machine is grounded
        if (positionState != PositionState.onAir){
            // If normal ground check is true then returns true
            if (characterController.isGrounded) return true;

            // Gets the bottom of the character
            Vector3 bottom = characterController.transform.position - new Vector3(0, characterController.height / 2, 0);

            // Creates a raycast
            RaycastHit hit;
            // Sends raycast downward
            if (Physics.Raycast(bottom, new Vector3(0, -1, 0), out hit, 0.2f)){
                // If raycast hits ground in 0.2 units, then pull character down said distance (prevents skipping when going down slopes)
                characterController.Move(new Vector3(0, -hit.distance, 0));
                return true;
            }

            return false;
        }

        // If state machine is on air, then return normal ground check
        return characterController.isGrounded;
    }

    private void AnimationHandler(){
        animator.SetFloat("X direction", inputX);
        animator.SetFloat("Y direction", inputZ);
        animator.SetFloat("Speed", moveDirection.magnitude > 0.5f ? moveDirection.magnitude : 0f);
        animator.SetBool("Grounded", positionState == PositionState.onAir);
    }

    private void PlaySound(AudioClip audioClip){
        charAudioSource.pitch = 1;
        charAudioSource.PlayOneShot(audioClip);
    }

}
