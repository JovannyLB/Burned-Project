using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;
using Random = UnityEngine.Random;

public class BallController : MonoBehaviour{

    private Rigidbody ballRB;
    
    public float throwPower;
    public float minSpeed;

    public ParticleSystem impactParticle;

    public GameObject player, hand;

    public AudioClip[] ballAudio;
    private AudioSource ballAudioSource;

    private bool canBePickedUp;

    public enum BallState{
        BeingHeld,
        Active,
        Inactive
    }
    public BallState ballState;

    private TweenerCore<UnityEngine.Vector3, UnityEngine.Vector3, DG.Tweening.Plugins.Options.VectorOptions> moveTween;

    void Start(){
        ballRB = GetComponent<Rigidbody>();
        ballAudioSource = GetComponent<AudioSource>();
    }

    void Update(){
        BallControl();
    }

    private void BallControl(){
        switch (ballState){
            case BallState.BeingHeld:
                GetComponent<TrailRenderer>().emitting = true;
                
                ballRB.useGravity = false;
                ballRB.isKinematic = true;
                ballRB.collisionDetectionMode = CollisionDetectionMode.Discrete;
                ballRB.drag = 0;
                
                if (transform.position != hand.transform.position){
                    moveTween = transform.DOMove(hand.transform.position, 0.25f);
                }
                
                if (Input.GetMouseButtonDown(0)){
                    if (player.GetComponent<PlayerController>().aiming){
                        int layerMask = LayerMask.GetMask("Scenery", "Enemy");

                        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit, 1000f, layerMask)){
                            transform.LookAt(hit.point);
                        }
                    }
                    
                    BallThrow();
                }

                break;
            case BallState.Active:
                GetComponent<TrailRenderer>().emitting = true;

                ballRB.useGravity = false;
                ballRB.isKinematic = false;
                ballRB.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                ballRB.drag = 0;
                
                if (ballRB.velocity.magnitude <= minSpeed && ballRB.velocity.magnitude > 0){
                    ballState = BallState.Inactive;
                }

                if (Input.GetKeyDown(KeyCode.E) && GameController.gameType == GameController.GameType.Testing){
                    ballState = BallState.Inactive;
                }
                break;
            case BallState.Inactive:
                GetComponent<TrailRenderer>().emitting = true;
                
                ballRB.useGravity = true;
                ballRB.isKinematic = false;
                ballRB.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                ballRB.drag = 0.1f;
                
                if (Input.GetMouseButtonDown(1) && GameController.gameType == GameController.GameType.Testing){
                    BallBack();
                }
                break;
        }
    }

    private void BallThrow(){
        ballRB.useGravity = false;
        ballRB.isKinematic = false;
        ballRB.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        ballRB.drag = 0;
        
        transform.parent = null;
        ballRB.AddForce(transform.forward * throwPower, ForceMode.Impulse);
        ballState = BallState.Active;

        StartCoroutine(BallBackTimer());
    }

    private void BallBack(){
        ballState = BallState.BeingHeld;

        ballRB.useGravity = false;
        ballRB.collisionDetectionMode = CollisionDetectionMode.Discrete;
        ballRB.isKinematic = true;
        ballRB.drag = 0;
        
//        transform.parent = hand.transform;
//        transform.position = hand.transform.position;
        transform.rotation = hand.transform.rotation;
    }

    private void OnCollisionEnter(Collision other){
        if (other.transform.gameObject.layer == 10 && ballState == BallState.Active){
            ballState = BallState.Inactive;
            StartCoroutine(other.transform.root.GetComponent<Enemy>().Death());
        }

        if (ballState == BallState.Active){ 
            SpawnImpact();
        }
        
        PlayBallSound();
    }

    private void OnTriggerEnter(Collider other){
        if (other.transform.CompareTag("Player") && canBePickedUp || other.transform.CompareTag("ForceField") && canBePickedUp){
            BallBack();
        }
    }

    private void SpawnImpact(){
        var currentImpactParticle = Instantiate(impactParticle, transform.position, Quaternion.identity);
        
        ParticleSystem.VelocityOverLifetimeModule editableVelocity = currentImpactParticle.velocityOverLifetime;
        editableVelocity.x = ballRB.velocity.x;
        editableVelocity.y = ballRB.velocity.y;
        editableVelocity.z = ballRB.velocity.z;
    }

    private void PlayBallSound(){
        ballAudioSource.pitch = Random.Range(0.75f, 1.25f);
        ballAudioSource.volume = ballRB.velocity.magnitude / 60;
        ballAudioSource.PlayOneShot(ballAudio[Random.Range(0, ballAudio.Length)]);
    }

    private IEnumerator BallBackTimer(){
        canBePickedUp = false;
        yield return new WaitForSeconds(0.5f);
        canBePickedUp = true;
    }
    
}
