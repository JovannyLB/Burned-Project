using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour{

    private Rigidbody enemyRigidbody;
    private GameObject enemy;

    private bool goRight;

    public enum HitState{
        NotHit,
        BeenHit
    }
    private HitState hitState;

    public enum Direction{
        GoX,
        GoY,
        GoZ,
        NoAxis
    }
    public Direction direction;

    public float speed;
    public float distance;

    public ParticleSystem deathParticles;
    public Material eyeColor;
    public GameObject pointLight;

    private Quaternion originalRotation;

    void Start(){
        enemyRigidbody = transform.GetChild(0).GetComponent<Rigidbody>();
        enemy = transform.GetChild(0).gameObject;
        enemy.transform.GetChild(0).GetComponent<MeshRenderer>().material = eyeColor;
        originalRotation = enemy.transform.rotation;
        pointLight = transform.GetChild(0).GetChild(1).gameObject;
        pointLight.GetComponent<Light>().color = eyeColor.color;
    }

    void Update(){
        switch (hitState){
            case HitState.NotHit:
                MoveDirection(direction);
                enemyRigidbody.useGravity = true;
                break;
            case HitState.BeenHit:
                enemyRigidbody.useGravity = false;
                break;
        }
    }

    private void MoveDirection(Direction goDirection){
        switch (goDirection){
            case Direction.GoX:
                if (enemy.transform.localPosition.x >= distance){
                    goRight = false;
                }
                else if (enemy.transform.localPosition.x <= -distance){
                    goRight = true;
                }
                if (goRight){
                    enemyRigidbody.velocity = new Vector3(speed, 0, 0);
                } else if (!goRight){
                    enemyRigidbody.velocity = new Vector3(-speed, 0, 0);
                }
                break;
            case Direction.GoY:
                if (enemy.transform.localPosition.x >= distance){
                    goRight = false;
                }
                else if (enemy.transform.localPosition.y <= -distance){
                    goRight = true;
                }
                if (goRight){
                    enemyRigidbody.velocity = new Vector3(0, speed, 0);
                } else if (!goRight){
                    enemyRigidbody.velocity = new Vector3(0, -speed, 0);
                }
                break;
            case Direction.GoZ:
                if (enemy.transform.localPosition.z >= distance){
                    goRight = false;
                }
                else if (enemy.transform.localPosition.z <= -distance){
                    goRight = true;
                }
                if (goRight){
                    enemyRigidbody.velocity = new Vector3(0, 0, speed);
                } else if (!goRight){
                    enemyRigidbody.velocity = new Vector3(0, 0, -speed);
                }
                break;
            case Direction.NoAxis:
                if (enemy.transform.localPosition.z >= distance){
                    goRight = false;
                }
                else if (enemy.transform.localPosition.z <= -distance){
                    goRight = false;
                }
                if (goRight){
                    enemyRigidbody.velocity = Vector3.zero;
                } else if (!goRight){
                    enemyRigidbody.velocity = Vector3.zero;
                }
                break;
        }
    }
    
    public IEnumerator Death(){
        hitState = HitState.BeenHit;
        
//        LockUnlockRotation(false);

        yield return new WaitForSeconds(0.25f);
        var currentDeathParticle = Instantiate(deathParticles, enemy.transform.position, Quaternion.identity);
        
        ParticleSystem.ShapeModule editableShape = currentDeathParticle.shape;
        editableShape.meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();

        ParticleSystem.VelocityOverLifetimeModule editableVelocity = currentDeathParticle.velocityOverLifetime;
        editableVelocity.x = enemyRigidbody.velocity.x;
        editableVelocity.y = enemyRigidbody.velocity.y;
        editableVelocity.z = enemyRigidbody.velocity.z;
        
        enemy.SetActive(false);

        yield return new WaitForSeconds(5f);
        
        enemy.transform.position = transform.position;
        enemy.transform.rotation = originalRotation;
//        LockUnlockRotation(true);

        enemy.SetActive(true);
        
        hitState = HitState.NotHit;
    }

    private void LockUnlockRotation(bool lockUnlock){
        enemyRigidbody.freezeRotation = lockUnlock;
    }

}
