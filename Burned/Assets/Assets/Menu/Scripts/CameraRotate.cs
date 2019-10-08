using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour{

    [Header("Speed at which camera rotates")]
    public float speed;
    
    void Update(){
        // Rotates the camera in the manu
        transform.Rotate(0, speed * Time.deltaTime, 0, Space.World);
    }
}
