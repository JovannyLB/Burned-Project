using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour{
    // Components
    private CharacterController characterController;

    // Input bools
    private bool rightMouseButton;

    // State bools
    [HideInInspector] public bool aiming;

    // UI
    private Image crossHair;
    
    // Ball catcher
    public GameObject ballField;
    public AudioClip[] forceFieldSounds;
    
    void Start(){
        StartUp();
    }

    void Update(){
        InputManager();
        CameraControl();
        Combat();
    }

    // Gets components
    private void StartUp(){
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
            var forward = Camera.main.transform.forward;
            forward.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(forward), 0.3f);
            crossHair.enabled = true;
            
            
            
            // Makes the crosshair red if it's pointed at an enemy
            int layerMask = LayerMask.GetMask("Scenery", "Enemy");

            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f, layerMask) && hit.transform.name == "Enemy@T-Pose"){
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
        
        // Ball force field
        if (Input.GetKey(KeyCode.Q) && !ballField.GetComponent<MeshRenderer>().enabled){
            ballField.GetComponent<MeshRenderer>().enabled = true;
            ballField.GetComponent<SphereCollider>().enabled = true;
            ballField.transform.localScale = Vector3.zero;
            ballField.transform.DOScale(new Vector3(7, 7, 7), 1f).OnComplete(() => ballField.GetComponent<AudioSource>().Play());
            ballField.GetComponent<AudioSource>().PlayOneShot(forceFieldSounds[0]);
        }
        else if(Input.GetKeyUp(KeyCode.Q) && ballField.GetComponent<MeshRenderer>().enabled){
            ballField.GetComponent<AudioSource>().Stop();
            ballField.GetComponent<AudioSource>().PlayOneShot(forceFieldSounds[2]);
            ballField.transform.DOScale(new Vector3(0, 0, 0), 1f).OnComplete(() => {
                ballField.GetComponent<MeshRenderer>().enabled = false;
                ballField.GetComponent<SphereCollider>().enabled = false;
            });
        }

        if (!ballField.GetComponent<MeshRenderer>().enabled){
            ballField.GetComponent<AudioSource>().Stop();
        }

    }
}
