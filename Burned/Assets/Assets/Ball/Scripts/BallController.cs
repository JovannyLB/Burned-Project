using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour{

    private Rigidbody ballRB;
    
    public float throwPower;
    public float minSpeed;

    public GameObject player;

    public enum BallState{
        BeingHeld,
        Active,
        Inactive
    }
    public BallState ballState;

    void Start(){
        ballRB = GetComponent<Rigidbody>();
    }

    void Update(){
        BallControl();
    }

    private void BallControl(){
        switch (ballState){
            case BallState.BeingHeld:
                
                ballRB.useGravity = false;
                ballRB.isKinematic = true;
                ballRB.collisionDetectionMode = CollisionDetectionMode.Discrete;
                ballRB.drag = 0;

                if (player.GetComponent<PlayerController>().aiming){
                    int layerMask = LayerMask.GetMask("Scenery", "Enemy");

                    Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 1000f, layerMask)){
                        transform.LookAt(hit.point);
                    }
                }

                if (Input.GetMouseButtonDown(0)){
                    BallThrow();
                }
                break;
            case BallState.Active:
                if (ballRB.velocity.magnitude <= minSpeed){
                    ballState = BallState.Inactive;
                }
                
                ballRB.useGravity = false;
                ballRB.isKinematic = false;
                ballRB.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                ballRB.drag = 0;
                
                if (Input.GetKeyDown(KeyCode.E)){
                    ballState = BallState.Inactive;
                }
                break;
            case BallState.Inactive:
                
                ballRB.useGravity = true;
                ballRB.isKinematic = false;
                ballRB.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                ballRB.drag = 0.1f;
                
                if (Input.GetMouseButtonDown(1)){
                    BallBack();
                }
                break;
        }
    }

    private void BallThrow(){
        ballState = BallState.Active;
        
        ballRB.useGravity = false;
        ballRB.isKinematic = false;
        ballRB.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        ballRB.drag = 0;
        
        transform.parent = null;
        ballRB.AddForce(transform.forward * throwPower, ForceMode.Impulse);
    }

    private void BallBack(){
        ballState = BallState.BeingHeld;

        ballRB.useGravity = false;
        ballRB.collisionDetectionMode = CollisionDetectionMode.Discrete;
        ballRB.isKinematic = true;
        ballRB.drag = 0;
        
        transform.parent = player.transform;
        transform.position = player.transform.GetChild(2).transform.position;
        transform.rotation = player.transform.GetChild(2).transform.rotation;
    }

    private void OnCollisionEnter(Collision other){
        if (other.transform.name == "Enemy Body" && ballState == BallState.Active){
            ballState = BallState.Inactive;
            StartCoroutine(other.transform.root.GetComponent<Enemy>().Death());
        } else if (other.transform.CompareTag("Player")){
            BallBack();
        }
    }

    private void OnTriggerEnter(Collider other){
        if (other.transform.name == "Enemy Body"){
            ballState = BallState.Inactive;
            StartCoroutine(other.transform.root.GetComponent<Enemy>().Death());
        }
    }
}
