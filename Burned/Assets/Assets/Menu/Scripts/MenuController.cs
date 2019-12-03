using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class MenuController : MonoBehaviour{

    // Lists all the possible menus
    public enum CurrentMenu{
        First,
        Second
    }
    private CurrentMenu currentMenu;
    public Image fade;

    // Holds all the existing menus
    public GameObject[] menus;
    public AudioClip audioMenu;

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
        FadeIn();
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
        FadeOutQuit();
    }

    public void ButtonBack(){
        currentMenu = CurrentMenu.First;
    }

    public void ButtonTraining(){
        GameController.gameType = GameController.GameType.Training;
        FadeOutScene("Arena");
    }
    
    public void ButtonTiming(){
        GameController.gameType = GameController.GameType.Timing;
        FadeOutScene("Arena");
    }
    
    public void ButtonTesting(){
        GameController.gameType = GameController.GameType.Testing;
        FadeOutScene("Arena");
    }

    private void FadeIn(){
        fade.color = Color.black;
        fade.DOColor(Color.clear, 2.5f);
    }

    private void FadeOutScene(String sceneName){
        fade.color = Color.clear;
        fade.DOColor(Color.black, 1f).OnComplete(() => { SceneManager.LoadScene(sceneName); });
    }

    private void FadeOutQuit(){
        fade.color = Color.clear;
        fade.DOColor(Color.black, 1f).OnComplete(Application.Quit);
    }

    public void PlaySound(){
        GetComponent<AudioSource>().PlayOneShot(audioMenu);
    }

}
