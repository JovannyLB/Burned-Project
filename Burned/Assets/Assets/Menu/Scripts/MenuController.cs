using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour{

    // Lists all the possible menus
    public enum CurrentMenu{
        First,
        Second
    }
    private CurrentMenu currentMenu;

    // Holds all the existing menus
    public GameObject[] menus;

    void Start(){
        StartUp();
        MenuSetup();
    }

    void Update(){
        MenuSetup();
    }

    private void StartUp(){
        // Makes sure the menu is aways first
        currentMenu = CurrentMenu.First;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void MenuSetup(){
        // Changes menus
        switch (currentMenu){
            case CurrentMenu.First:
                SetMenuActive(menus, 0);
                break;
            case CurrentMenu.Second:
                SetMenuActive(menus, 1);
                break;
        }
    }

    private void SetMenuActive(GameObject[] list, int i){
        // Activates current menu and deactivates the rest
        for (int j = 0; j < list.Length; j++){
            if (j == i){
                list[j].SetActive(true);
            }
            else{
                list[j].SetActive(false);
            }
        }
    }

    // Buttons
    public void ButtonPlay(){
        currentMenu = CurrentMenu.Second;
    }

    public void ButtonQuit(){
        Application.Quit();
    }

    public void ButtonBack(){
        currentMenu = CurrentMenu.First;
    }

    public void ButtonTraining(){
        GameController.gameType = GameController.GameType.Training;
        SceneManager.LoadScene("Arena");
    }
    
    public void ButtonTiming(){
        GameController.gameType = GameController.GameType.Timing;
        SceneManager.LoadScene("Arena");
    }
    
    public void ButtonTesting(){
        GameController.gameType = GameController.GameType.Testing;
        SceneManager.LoadScene("Arena");
    }

}
