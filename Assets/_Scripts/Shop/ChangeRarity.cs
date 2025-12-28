using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeRarity : MonoBehaviour
{
    public Image shopBackGround;

    [HideInInspector]
    public int nowRarity = 1;//1:normal 2:rare 3:epic
    
    public List<BayItemStack> bayItemStacks = new List<BayItemStack>();
    
    public void OnclickChangeRarity(int rarity)//1:normal 2:rare 3:epic
    {
        nowRarity = rarity;
        StackReset();
        switch (rarity)
        {
            case 1:
                shopBackGround.color = new Color32(255, 255, 255, 150);
                break;
            case 2:
                shopBackGround.color = new Color32(0, 255, 255, 150);
                break;
            case 3:
                shopBackGround.color = new Color32(255, 36, 255, 150);
                break;
        }

    }

    public void StackReset()
    {
        for (int i = 0; i < bayItemStacks.Count; i++)
        {
            bayItemStacks[i].RarityStackText();
            bayItemStacks[i].StackPriceText();
        }
    }
}
