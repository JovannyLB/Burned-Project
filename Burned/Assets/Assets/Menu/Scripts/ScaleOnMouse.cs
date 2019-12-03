using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScaleOnMouse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{

    public MenuController menuController;
    
    public void OnPointerEnter(PointerEventData eventData){
        if (GetComponent<Button>().interactable){
            GetComponent<RectTransform>().DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.1f);
            menuController.PlaySound();
        }
    }

    public void OnPointerExit(PointerEventData eventData){
        if (GetComponent<Button>().interactable){
            GetComponent<RectTransform>().DOScale(new Vector3(1f, 1, 1f), 0.1f);
        }
    }
}
