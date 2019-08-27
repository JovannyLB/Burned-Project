using System.Collections;
using System.Collections.Generic;
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

    // Inputs, turnspeed and gravity
    private float inputX, inputZ, turnSpeed, gravity;
    
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

    // State machines
    public enum PositionState{
        onGround,
        onAir
    }
    private PositionState positionState;

    void Start(){
        StartUp();
    }

    void Update(){
        InputManager();
        InputDecider();
        MovementManager();
    }

    // Gets the components
    private void StartUp(){
        // Character contoller
        characterController = GetComponent<CharacterController>();
        // Main camera
        cam = Camera.main;
    }

    // Gets the inputs
    private void InputManager(){
        // Axis
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");

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
        // Gets the actual speed
        float endSpeed;

        // Checks if playsr can run and is running
        if (Input.GetKey(KeyCode.LeftShift) && canRun){
            endSpeed = sprintSpeed;
        } else if (aiming){
            endSpeed = aimSpeed;
        } else{
            endSpeed = moveSpeed;
        }
        
        // Applies said speed to the movement
        moveDirection = new Vector3(desiredMoveDirection.x * endSpeed, moveDirection.y, desiredMoveDirection.z * endSpeed);

        // Creates gravity
        gravity -= 9.8f * Time.deltaTime;
        gravity *= gravityMultiplier;
        // Applies said gravity to movement
        moveDirection = new Vector3(moveDirection.x, moveDirection.y + gravity, moveDirection.z);
        
        // Jump check
        if (isGrounded()){
            if (Input.GetKey(KeyCode.Space)){
                positionState = PositionState.onAir;
                moveDirection.y = jumpForce;
            }
            else{
                positionState = PositionState.onGround;
                moveDirection.y = 0;
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

}
