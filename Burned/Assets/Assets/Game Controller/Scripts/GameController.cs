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
    public GameObject cubes, tutorial, tutorialTest, tutorialTip;
    private bool tutorialSwitch = false;

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

        if (Input.GetKeyDown(KeyCode.Return)){
            tutorialSwitch = !tutorialSwitch;
        }

        if (tutorialSwitch){
            if (gameType != GameType.Testing){
                tutorial.SetActive(true);
            }
            else{
                tutorialTest.SetActive(true);
            }
            
            tutorialTip.SetActive(false);
        }
        else{
            if (gameType != GameType.Testing){
                tutorial.SetActive(false);
            }
            else{
                tutorialTest.SetActive(false);
            }

            tutorialTip.SetActive(true);
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
                cubes.SetActive(false);
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
