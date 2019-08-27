using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour{

    public float mouseSensitivity = 7;
    public float mouseSensitivityAim = 4;
    public Transform target;
    public float distanceFromTarget = 6;
    public float distanceFromTargetAim = 2;
    public float actualDistanceFromTarget = 6;
    public Vector2 pitchMinMax = new Vector2(-40, 85);
    public float rotationSmoothTime = 0.1f;

    private Vector3 rotationSmoothVelocity, currentRotation;
    private float yaw, pitch;

    private bool cursorLock = true;

    void LateUpdate(){
        if (cursorLock){
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        yaw += Input.GetAxis("Mouse X") * (target.transform.root.GetComponent<PlayerController>().aiming ? mouseSensitivityAim : mouseSensitivity);
        pitch -= Input.GetAxis("Mouse Y") * (target.transform.root.GetComponent<PlayerController>().aiming ? mouseSensitivityAim : mouseSensitivity);
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
        transform.eulerAngles = currentRotation;

        RaycastHit hit;

        if (Physics.Linecast(target.transform.position, target.position - transform.forward * distanceFromTarget, out hit) && !target.transform.root.GetComponent<PlayerController>().aiming){
            print(hit.distance);
            actualDistanceFromTarget = hit.distance;
        } else if (Physics.Linecast(target.transform.position, target.position - transform.forward * distanceFromTargetAim, out hit) && target.transform.root.GetComponent<PlayerController>().aiming){
            print(hit.distance);
            actualDistanceFromTarget = hit.distance;
        } else{
            if (!target.transform.root.GetComponent<PlayerController>().aiming){
                DOTween.To(() => actualDistanceFromTarget, x => actualDistanceFromTarget = x, distanceFromTarget, 0.1f).OnComplete(() => {
                    actualDistanceFromTarget = distanceFromTarget;
                });
            }
            else{
                DOTween.To(() => actualDistanceFromTarget, x => actualDistanceFromTarget = x, distanceFromTargetAim, 0.1f).OnComplete(() => {
                    actualDistanceFromTarget = distanceFromTargetAim;
                });
            }
        }
        
        transform.position = target.position - transform.forward * actualDistanceFromTarget;
    }
}
