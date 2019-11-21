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
    
    [Header("Particles")]
    public bool particleInheritVelocity;
    public float particleMultiplier;
    public float particleTimedExplosion;

    public ParticleSystem deathParticles;
    public Material eyeColor;
    
    private GameObject pointLight;
    private Quaternion originalRotation;

    void Start(){
        enemyRigidbody = transform.GetChild(0).GetComponent<Rigidbody>();
        enemy = transform.GetChild(0).gameObject;
        enemy.transform.GetChild(3).GetComponent<SkinnedMeshRenderer>().material = eyeColor;
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

        yield return new WaitForSeconds(particleTimedExplosion);
        var currentDeathParticle = Instantiate(deathParticles, enemy.transform.position, Quaternion.identity);
        
        ParticleSystem.ShapeModule editableShape = currentDeathParticle.shape;
        editableShape.skinnedMeshRenderer = enemy.transform.GetChild(3).GetComponent<SkinnedMeshRenderer>();

        if (particleInheritVelocity){
            ParticleSystem.VelocityOverLifetimeModule editableVelocity = currentDeathParticle.velocityOverLifetime;
            editableVelocity.x = enemyRigidbody.velocity.x / particleMultiplier;
            editableVelocity.y = enemyRigidbody.velocity.y / particleMultiplier;
            editableVelocity.z = enemyRigidbody.velocity.z / particleMultiplier;
        }

        enemy.SetActive(false);

        // Checks the type of game mode to determine if the enemy will respawn
        if (GameController.gameType == GameController.GameType.Training || GameController.gameType == GameController.GameType.Testing){
            yield return new WaitForSeconds(5f);

            enemy.transform.position = transform.position + new Vector3(0, -1.3f, 0);
            enemy.transform.rotation = originalRotation;
            enemyRigidbody.useGravity = false;
//        LockUnlockRotation(true);

            enemy.SetActive(true);

            hitState = HitState.NotHit;
        } else if (GameController.gameType == GameController.GameType.Timing){
            FindObjectOfType<GameController>().currentPoints++;
        }
    }

    private void LockUnlockRotation(bool lockUnlock){
        enemyRigidbody.freezeRotation = lockUnlock;
    }

}
