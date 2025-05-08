using System;
using System.Collections.Generic;
using UnityEngine;

public enum EPopupType
{
    Popup_Option,
    Popup_Credit,
}

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    public List<UI_Popup> PopupList;
    public Stack<UI_Popup> PopupStack;
    private void Awake()
    {
        Instance = this;
        PopupStack = new Stack<UI_Popup>();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (PopupStack.TryPop(out var popup))
            {
                bool opened = popup.isActiveAndEnabled;
                if (opened)
                {
                    popup.Close();
                }
            }
            else
            {
                GameManager.Instance.Pause();
            }
        }
    }

    
    public bool TryOpen(EPopupType popupName, Action closeCallback=null)
    {
        return OpenPopup(popupName.ToString(), closeCallback);
    }

    private bool OpenPopup(string popupName, Action closeCallback)
    {
        foreach (var popup in PopupList)
        {
            if (popup.name == popupName)
            {
                popup.Open(closeCallback);
                PopupStack.Push(popup);
                return true;
            }
        }
        return false;
    }
    public void RemoveFromStack(UI_Popup target)
    {
        var temp = new Stack<UI_Popup>();

        while (PopupStack.Count > 0)
        {
            var popup = PopupStack.Pop();
            if (popup == target) break;
            temp.Push(popup);
        }

        while (temp.Count > 0)
        {
            PopupStack.Push(temp.Pop());
        }
    }
    public bool TryClose(EPopupType popupName)
    {
        var closed = (ClosePopup(popupName.ToString()));

        if (closed == null) return false;
        RemoveFromStack(closed);
        return true;
    }

    private UI_Popup ClosePopup(string popupName)
    {
        foreach (var popup in PopupList)
        {
            if (popup.name == popupName)
            {
                popup.Close();
                return popup;
            }
        }
        return null;
    }
}
