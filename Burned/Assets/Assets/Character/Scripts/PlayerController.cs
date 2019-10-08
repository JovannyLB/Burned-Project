using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour{
    // Components
    private Animator animator;
    private CharacterController characterController;

    // Input bools
    private bool rightMouseButton;

    // State bools
    [HideInInspector] public bool aiming;

    // UI
    private Image crossHair;
    
    // Particles
    public ParticleSystem deathParticles;

    void Start(){
        StartUp();
    }

    void Update(){
        InputManager();
        CameraControl();
        Combat();
        Animation();

        if (Input.GetKeyDown(KeyCode.F) && GameController.gameType == GameController.GameType.Testing){
            Death();
        }
    }

    // Gets components
    private void StartUp(){
        // Animator
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        crossHair = GameObject.FindGameObjectWithTag("Crosshair").GetComponent<Image>();
    }

    // Gets inputs
    private void InputManager(){
        // Mouse buttons
        rightMouseButton = Input.GetMouseButton(1);
    }

    // Corrects camera
    private void CameraControl(){
        // Makes the player rotate with the camera if aiming
        if (aiming){
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Camera.main.transform.forward), 0.3f);
            crossHair.enabled = true;
            
            // Makes the crosshair red if it's pointed at an enemy
            int layerMask = LayerMask.GetMask("Scenery", "Enemy");

            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f, layerMask) && hit.transform.name == "Enemy Body"){
                crossHair.color = Color.red;
            }
            else{
                crossHair.color = Color.white;
            }
        }
        else{
            crossHair.enabled = false;
        }
    }

    // Handles combat
    private void Combat(){
        // Checks if character is aiming
        aiming = rightMouseButton;
    }

    // Handles animation
    private void Animation(){
        
    }

    private void Death(){
        var currentDeathParticle = Instantiate(deathParticles, transform.position, Quaternion.identity);
        
        ParticleSystem.ShapeModule editableShape = currentDeathParticle.shape;
        editableShape.meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();

        ParticleSystem.VelocityOverLifetimeModule editableVelocity = currentDeathParticle.velocityOverLifetime;
        editableVelocity.x = characterController.velocity.x;
        editableVelocity.y = characterController.velocity.y;
        editableVelocity.z = characterController.velocity.z;
    }
}
