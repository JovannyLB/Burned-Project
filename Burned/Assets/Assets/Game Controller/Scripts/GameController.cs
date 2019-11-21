using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour{

    public enum GameType{
        Training,
        Timing,
        Testing
    }
    public static GameType gameType;

    public int currentPoints, maxPoints;
    public GameObject cubes;
    
    public enum PlayerControl{
        ControllingPlayer,
        ControllingMenu,
        ControllingNothing
    }
    public static PlayerControl playerControl;

    void Start(){
        playerControl = PlayerControl.ControllingPlayer;
        if (Application.isEditor){
            gameType = GameType.Testing;
        }
    }

    void Update(){
        if (playerControl == PlayerControl.ControllingPlayer || playerControl == PlayerControl.ControllingNothing){
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        } else if (playerControl == PlayerControl.ControllingMenu){
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        GameplayLoop();
    }

    private void GameplayLoop(){
        switch (gameType){
            case GameType.Timing:
                if (currentPoints >= maxPoints){
                    SceneManager.LoadScene("Main Menu");
                } else if (Input.GetKey(KeyCode.Escape)){
                    SceneManager.LoadScene("Main Menu");
                }
                cubes.SetActive(false);
                break;
            case GameType.Training:
                if (Input.GetKey(KeyCode.Escape)){
                    SceneManager.LoadScene("Main Menu");
                }
                cubes.SetActive(true);
                break;
            case GameType.Testing:
                if (Input.GetKey(KeyCode.Escape)){
                    SceneManager.LoadScene("Main Menu");
                }
                cubes.SetActive(true);
                break;
        }
    }
}
