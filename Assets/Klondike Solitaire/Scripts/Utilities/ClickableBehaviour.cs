using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ClickableBehaviour : MonoBehaviour, IPointerClickHandler
{
    Selectable s;

    private void Awake()
    {
        s = GetComponent<Selectable>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {       
        if (s != null && s.interactable == false)
            return;
        SoundManager.instance.PlayBtnClickSound();
    }


}
