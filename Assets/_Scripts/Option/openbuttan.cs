using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class openbuttan : MonoBehaviour
{
    bool canClick = true;
    public List<Button> buttonList;
    public void notClick()
    {
        int buttonListMax=buttonList.Count;
        for(int i = 0; i < buttonListMax; i++)
        {
            if (canClick)
            {
                buttonList[i].interactable = true;
            }
            else
            {
                buttonList[i].interactable = false;
            }
        }
    }

    public void OnButton()
    {

        if (this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(false);
            canClick = true;
        }
        else
        {
            this.gameObject.SetActive(true);
            canClick = false;
        }
        notClick();
    }
}
